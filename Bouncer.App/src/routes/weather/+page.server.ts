import type { PageServerLoad } from './$types';
import { API_BASE_URL } from '$env/static/private';

export interface WeatherForecast {
	date: string;
	temperatureC: number;
	temperatureF: number;
	summary: string;
}

export const load: PageServerLoad = async ({ fetch }) => {
	try {
		const response = await fetch(`${API_BASE_URL}/weatherforecast`, {
			method: 'GET',
			headers: {
				'Accept': 'application/json'
			}
		});

		if (!response.ok) {
			throw new Error(`HTTP error! status: ${response.status}`);
		}

		const weatherData: WeatherForecast[] = await response.json();

		return {
			weather: weatherData
		};
	} catch (error) {
		console.error('Failed to fetch weather data:', error);

		// Return mock data as fallback
		return {
			weather: [
				{
					date: new Date().toISOString().split('T')[0],
					temperatureC: 20,
					temperatureF: 68,
					summary: 'API Unavailable'
				}
			] as WeatherForecast[]
		};
	}
};