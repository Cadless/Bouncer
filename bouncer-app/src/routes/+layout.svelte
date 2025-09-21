<script lang="ts">
	import favicon from '$lib/assets/favicon.svg';
	import AuthGuard from '$lib/components/AuthGuard.svelte';
	import Header from '$lib/components/Header.svelte';

	let { children } = $props();
</script>

<svelte:head>
	<link rel="icon" href={favicon} />
</svelte:head>

<AuthGuard>
	<Header />
	<main>
		{@render children?.()}
	</main>
</AuthGuard>

<style>
	:global(body) {
		margin: 0;
		padding: 0;
		font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
		background: #f8f9fa;
	}

	main {
		min-height: calc(100vh - 60px);
	}
</style>
