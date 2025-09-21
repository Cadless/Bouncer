import type { Configuration, RedirectRequest } from '@azure/msal-browser';
import { PUBLIC_MSAL_CLIENT_ID, PUBLIC_MSAL_TENANT_ID } from '$env/static/public';
import { browser } from '$app/environment';

// Get redirect URI based on environment
const getRedirectUri = (): string => {
	if (!browser) return '';
	return window.location.origin;
};

export const msalConfig: Configuration = {
	auth: {
		clientId: PUBLIC_MSAL_CLIENT_ID,
		authority: `https://login.microsoftonline.com/${PUBLIC_MSAL_TENANT_ID}`,
		redirectUri: getRedirectUri(),
		postLogoutRedirectUri: getRedirectUri(),
		navigateToLoginRequestUrl: false
	},
	cache: {
		cacheLocation: 'sessionStorage',
		storeAuthStateInCookie: false
	}
};

export const loginRequest: RedirectRequest = {
	scopes: ['User.Read'],
	prompt: 'select_account'
};

export const graphConfig = {
	graphMeEndpoint: 'https://graph.microsoft.com/v1.0/me'
};