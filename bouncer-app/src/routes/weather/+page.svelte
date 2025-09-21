<script lang="ts">
	import type { PageData } from './$types';

	export let data: PageData;

	function formatDate(dateString: string): string {
		return new Date(dateString).toLocaleDateString('en-US', {
			weekday: 'long',
			year: 'numeric',
			month: 'long',
			day: 'numeric'
		});
	}
</script>

<svelte:head>
	<title>Weather Forecast - Bouncer</title>
</svelte:head>

<div class="container">
	<h1>Weather Forecast</h1>

	<div class="weather-grid">
		{#each data.weather as forecast}
			<div class="weather-card">
				<h3>{formatDate(forecast.date)}</h3>
				<div class="temperature">
					<span class="temp-c">{forecast.temperatureC}°C</span>
					<span class="temp-f">({forecast.temperatureF}°F)</span>
				</div>
				<p class="summary">{forecast.summary}</p>
			</div>
		{/each}
	</div>
</div>

<style>
	.container {
		max-width: 1200px;
		margin: 0 auto;
		padding: 2rem;
	}

	h1 {
		text-align: center;
		color: #333;
		margin-bottom: 2rem;
	}

	.weather-grid {
		display: grid;
		grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
		gap: 1.5rem;
	}

	.weather-card {
		background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
		color: white;
		padding: 1.5rem;
		border-radius: 12px;
		box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
		transition: transform 0.2s ease;
	}

	.weather-card:hover {
		transform: translateY(-2px);
	}

	.weather-card h3 {
		margin: 0 0 1rem 0;
		font-size: 1.1rem;
		font-weight: 600;
	}

	.temperature {
		display: flex;
		align-items: baseline;
		gap: 0.5rem;
		margin-bottom: 1rem;
	}

	.temp-c {
		font-size: 2rem;
		font-weight: bold;
	}

	.temp-f {
		font-size: 1rem;
		opacity: 0.8;
	}

	.summary {
		margin: 0;
		font-size: 1.1rem;
		font-weight: 500;
		text-transform: capitalize;
	}

	@media (max-width: 768px) {
		.container {
			padding: 1rem;
		}

		.weather-grid {
			grid-template-columns: 1fr;
		}
	}
</style>