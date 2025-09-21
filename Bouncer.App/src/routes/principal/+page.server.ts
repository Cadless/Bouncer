import type { PageServerLoad, Actions } from './$types';
import { API_BASE_URL } from '$env/static/private';
import { fail, redirect } from '@sveltejs/kit';

export interface Principal {
	id: number;
	externalId: string;
	createdAt: string;
	updatedAt?: string;
}

export const load: PageServerLoad = async ({ fetch }) => {
	try {
		const response = await fetch(`${API_BASE_URL}/api/principal`, {
			method: 'GET',
			headers: {
				'Accept': 'application/json'
			}
		});

		if (!response.ok) {
			throw new Error(`HTTP error! status: ${response.status}`);
		}

		const principals: Principal[] = await response.json();

		return {
			principals
		};
	} catch (error) {
		console.error('Failed to fetch principals:', error);
		return {
			principals: [] as Principal[]
		};
	}
};

export const actions: Actions = {
	create: async ({ request, fetch }) => {
		const data = await request.formData();
		const externalId = data.get('externalId')?.toString();

		if (!externalId) {
			return fail(400, { error: 'External ID is required' });
		}

		try {
			const response = await fetch(`${API_BASE_URL}/api/principal`, {
				method: 'POST',
				headers: {
					'Content-Type': 'application/json'
				},
				body: JSON.stringify({ externalId })
			});

			if (!response.ok) {
				const errorText = await response.text();
				return fail(response.status, { error: errorText });
			}

			return { success: true };
		} catch (error) {
			console.error('Failed to create principal:', error);
			return fail(500, { error: 'Failed to create principal' });
		}
	},

	update: async ({ request, fetch }) => {
		const data = await request.formData();
		const id = data.get('id')?.toString();
		const externalId = data.get('externalId')?.toString();

		if (!id || !externalId) {
			return fail(400, { error: 'ID and External ID are required' });
		}

		try {
			const response = await fetch(`${API_BASE_URL}/api/principal/${id}`, {
				method: 'PUT',
				headers: {
					'Content-Type': 'application/json'
				},
				body: JSON.stringify({ externalId })
			});

			if (!response.ok) {
				const errorText = await response.text();
				return fail(response.status, { error: errorText });
			}

			return { success: true };
		} catch (error) {
			console.error('Failed to update principal:', error);
			return fail(500, { error: 'Failed to update principal' });
		}
	},

	delete: async ({ request, fetch }) => {
		const data = await request.formData();
		const id = data.get('id')?.toString();

		if (!id) {
			return fail(400, { error: 'ID is required' });
		}

		try {
			const response = await fetch(`${API_BASE_URL}/api/principal/${id}`, {
				method: 'DELETE'
			});

			if (!response.ok) {
				const errorText = await response.text();
				return fail(response.status, { error: errorText });
			}

			return { success: true };
		} catch (error) {
			console.error('Failed to delete principal:', error);
			return fail(500, { error: 'Failed to delete principal' });
		}
	}
};