<script lang="ts">
	import { onMount } from 'svelte';
	import { isAuthenticated, isLoading, authService } from '$lib/auth/authService';
	import { browser } from '$app/environment';

	export let requireAuth = true;

	let showLogin = false;

	onMount(() => {
		if (browser && requireAuth) {
			const unsubscribe = isAuthenticated.subscribe((authenticated) => {
				if (!$isLoading && !authenticated) {
					showLogin = true;
				} else {
					showLogin = false;
				}
			});

			return unsubscribe;
		}
	});

	async function handleLogin() {
		try {
			await authService.login();
		} catch (error) {
			console.error('Login failed:', error);
		}
	}
</script>

{#if $isLoading}
	<div class="loading-container">
		<div class="loading-spinner"></div>
		<p>Loading...</p>
	</div>
{:else if showLogin && requireAuth}
	<div class="login-container">
		<div class="login-card">
			<h1>Bouncer Admin</h1>
			<p>Please sign in with your Microsoft account to access the admin portal.</p>
			<button class="login-btn" on:click={handleLogin}>
				<svg width="21" height="21" viewBox="0 0 21 21" fill="none" xmlns="http://www.w3.org/2000/svg">
					<path fill="#f25022" d="M1 1h9v9H1z"/>
					<path fill="#00a4ef" d="M11 1h9v9h-9z"/>
					<path fill="#7fba00" d="M1 11h9v9H1z"/>
					<path fill="#ffb900" d="M11 11h9v9h-9z"/>
				</svg>
				Sign in with Microsoft
			</button>
		</div>
	</div>
{:else}
	<slot />
{/if}

<style>
	.loading-container {
		display: flex;
		flex-direction: column;
		align-items: center;
		justify-content: center;
		min-height: 100vh;
		gap: 1rem;
	}

	.loading-spinner {
		width: 40px;
		height: 40px;
		border: 4px solid #f3f3f3;
		border-top: 4px solid #007bff;
		border-radius: 50%;
		animation: spin 1s linear infinite;
	}

	@keyframes spin {
		0% { transform: rotate(0deg); }
		100% { transform: rotate(360deg); }
	}

	.login-container {
		display: flex;
		align-items: center;
		justify-content: center;
		min-height: 100vh;
		background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
		padding: 2rem;
	}

	.login-card {
		background: white;
		padding: 3rem;
		border-radius: 12px;
		box-shadow: 0 8px 32px rgba(0, 0, 0, 0.1);
		text-align: center;
		max-width: 400px;
		width: 100%;
	}

	.login-card h1 {
		margin: 0 0 1rem 0;
		color: #333;
		font-size: 2rem;
	}

	.login-card p {
		margin: 0 0 2rem 0;
		color: #666;
		line-height: 1.5;
	}

	.login-btn {
		display: flex;
		align-items: center;
		justify-content: center;
		gap: 0.75rem;
		width: 100%;
		padding: 1rem 1.5rem;
		background: #0078d4;
		color: white;
		border: none;
		border-radius: 6px;
		font-size: 1rem;
		font-weight: 500;
		cursor: pointer;
		transition: background-color 0.2s;
	}

	.login-btn:hover {
		background: #106ebe;
	}

	.login-btn:active {
		transform: translateY(1px);
	}

	.login-btn svg {
		flex-shrink: 0;
	}
</style>