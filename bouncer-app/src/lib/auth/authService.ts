import { PublicClientApplication, type AccountInfo, type AuthenticationResult } from '@azure/msal-browser';
import { msalConfig, loginRequest } from './msalConfig';
import { browser } from '$app/environment';
import { writable } from 'svelte/store';

export interface User {
	id: string;
	email: string;
	name: string;
	tenantId: string;
}

export const isAuthenticated = writable<boolean>(false);
export const user = writable<User | null>(null);
export const isLoading = writable<boolean>(true);

class AuthService {
	private msalInstance: PublicClientApplication | null = null;
	private account: AccountInfo | null = null;

	async initialize(): Promise<void> {
		if (!browser) return;

		try {
			this.msalInstance = new PublicClientApplication(msalConfig);
			await this.msalInstance.initialize();

			// Handle redirect response
			const response = await this.msalInstance.handleRedirectPromise();
			if (response) {
				this.handleLoginSuccess(response);
			} else {
				// Check if user is already logged in
				const accounts = this.msalInstance.getAllAccounts();
				if (accounts.length > 0) {
					this.account = accounts[0];
					this.setAuthenticatedState(true);
					await this.setUserFromAccount(this.account);
				}
			}
		} catch (error) {
			console.error('Failed to initialize MSAL:', error);
		} finally {
			isLoading.set(false);
		}
	}

	async login(): Promise<void> {
		if (!this.msalInstance) {
			throw new Error('MSAL not initialized');
		}

		try {
			isLoading.set(true);
			await this.msalInstance.loginRedirect(loginRequest);
		} catch (error) {
			console.error('Login failed:', error);
			isLoading.set(false);
			throw error;
		}
	}

	async logout(): Promise<void> {
		if (!this.msalInstance || !this.account) {
			return;
		}

		try {
			await this.msalInstance.logoutRedirect({
				account: this.account
			});
		} catch (error) {
			console.error('Logout failed:', error);
			throw error;
		}
	}

	async getAccessToken(): Promise<string | null> {
		if (!this.msalInstance || !this.account) {
			return null;
		}

		try {
			const response = await this.msalInstance.acquireTokenSilent({
				...loginRequest,
				account: this.account
			});
			return response.accessToken;
		} catch (error) {
			console.error('Failed to acquire token silently:', error);
			// If silent acquisition fails, try interactive
			try {
				const response = await this.msalInstance.acquireTokenRedirect({
					...loginRequest,
					account: this.account
				});
				return response?.accessToken || null;
			} catch (interactiveError) {
				console.error('Failed to acquire token interactively:', interactiveError);
				return null;
			}
		}
	}

	private handleLoginSuccess(response: AuthenticationResult): void {
		this.account = response.account;
		this.setAuthenticatedState(true);
		this.setUserFromAccount(response.account);
	}

	private setAuthenticatedState(authenticated: boolean): void {
		isAuthenticated.set(authenticated);
		if (!authenticated) {
			user.set(null);
			this.account = null;
		}
	}

	private async setUserFromAccount(account: AccountInfo): Promise<void> {
		const userInfo: User = {
			id: account.localAccountId,
			email: account.username,
			name: account.name || account.username,
			tenantId: account.tenantId || ''
		};
		user.set(userInfo);
	}

	getAccount(): AccountInfo | null {
		return this.account;
	}

	isUserAuthenticated(): boolean {
		return this.account !== null;
	}
}

// Create singleton instance
export const authService = new AuthService();

// Initialize auth service when in browser
if (browser) {
	authService.initialize();
}