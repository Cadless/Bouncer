<script lang="ts">
	import { isAuthenticated, user, authService } from '$lib/auth/authService';
	import { page } from '$app/stores';

	async function handleLogout() {
		try {
			await authService.logout();
		} catch (error) {
			console.error('Logout failed:', error);
		}
	}
</script>

{#if $isAuthenticated}
	<header class="header">
		<div class="container">
			<div class="brand">
				<h1><a href="/">Bouncer Admin</a></h1>
			</div>

			<nav class="nav">
				<a href="/principal" class:active={$page.url.pathname === '/principal'}>
					Principals
				</a>
				<a href="/weather" class:active={$page.url.pathname === '/weather'}>
					Weather
				</a>
			</nav>

			<div class="user-menu">
				<div class="user-info">
					<span class="user-name">{$user?.name || $user?.email}</span>
					<button class="logout-btn" on:click={handleLogout}>
						Sign Out
					</button>
				</div>
			</div>
		</div>
	</header>
{/if}

<style>
	.header {
		background: white;
		border-bottom: 1px solid #e5e5e5;
		box-shadow: 0 2px 4px rgba(0, 0, 0, 0.05);
		position: sticky;
		top: 0;
		z-index: 100;
	}

	.container {
		max-width: 1200px;
		margin: 0 auto;
		padding: 0 2rem;
		display: flex;
		align-items: center;
		justify-content: space-between;
		height: 60px;
	}

	.brand h1 {
		margin: 0;
		font-size: 1.5rem;
	}

	.brand a {
		color: #333;
		text-decoration: none;
	}

	.brand a:hover {
		color: #007bff;
	}

	.nav {
		display: flex;
		gap: 2rem;
		align-items: center;
	}

	.nav a {
		color: #666;
		text-decoration: none;
		font-weight: 500;
		padding: 0.5rem 1rem;
		border-radius: 6px;
		transition: all 0.2s;
	}

	.nav a:hover {
		color: #007bff;
		background: #f8f9fa;
	}

	.nav a.active {
		color: #007bff;
		background: #e7f3ff;
	}

	.user-menu {
		display: flex;
		align-items: center;
	}

	.user-info {
		display: flex;
		align-items: center;
		gap: 1rem;
	}

	.user-name {
		color: #333;
		font-weight: 500;
	}

	.logout-btn {
		padding: 0.5rem 1rem;
		background: #6c757d;
		color: white;
		border: none;
		border-radius: 4px;
		font-size: 0.9rem;
		cursor: pointer;
		transition: background-color 0.2s;
	}

	.logout-btn:hover {
		background: #5a6268;
	}

	@media (max-width: 768px) {
		.container {
			padding: 0 1rem;
		}

		.nav {
			gap: 1rem;
		}

		.nav a {
			padding: 0.5rem;
			font-size: 0.9rem;
		}

		.user-name {
			display: none;
		}
	}
</style>