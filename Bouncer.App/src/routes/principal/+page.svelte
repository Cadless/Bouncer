<script lang="ts">
	import type { PageData, ActionData } from './$types';
	import { enhance } from '$app/forms';
	import { invalidateAll } from '$app/navigation';

	export let data: PageData;
	export let form: ActionData;

	let showSidebar = false;
	let editingPrincipal: typeof data.principals[0] | null = null;
	let isEditing = false;

	function openCreateForm() {
		editingPrincipal = null;
		isEditing = false;
		showSidebar = true;
	}

	function openEditForm(principal: typeof data.principals[0]) {
		editingPrincipal = principal;
		isEditing = true;
		showSidebar = true;
	}

	function closeSidebar() {
		showSidebar = false;
		editingPrincipal = null;
		isEditing = false;
	}

	function formatDate(dateString: string): string {
		return new Date(dateString).toLocaleString();
	}

	let isSubmitting = false;

	function handleFormSubmit() {
		isSubmitting = true;
		return async ({ result }: any) => {
			isSubmitting = false;
			if (result.type === 'success') {
				closeSidebar();
				await invalidateAll();
			}
		};
	}
</script>

<svelte:head>
	<title>Principal Management - Bouncer</title>
</svelte:head>

<div class="container">
	<div class="header">
		<h1>Principal Management</h1>
		<button class="btn btn-primary" on:click={openCreateForm}>Add Principal</button>
	</div>

	{#if form?.error}
		<div class="error-message">
			{form.error}
		</div>
	{/if}

	<div class="grid-container">
		<table class="data-grid">
			<thead>
				<tr>
					<th>ID</th>
					<th>External ID</th>
					<th>Created</th>
					<th>Updated</th>
					<th>Actions</th>
				</tr>
			</thead>
			<tbody>
				{#each data.principals as principal}
					<tr>
						<td>{principal.id}</td>
						<td>{principal.externalId}</td>
						<td>{formatDate(principal.createdAt)}</td>
						<td>{principal.updatedAt ? formatDate(principal.updatedAt) : '-'}</td>
						<td class="actions">
							<button
								class="btn btn-secondary btn-sm"
								on:click={() => openEditForm(principal)}
							>
								Edit
							</button>
							<form method="POST" action="?/delete" use:enhance={handleFormSubmit} style="display: inline;">
								<input type="hidden" name="id" value={principal.id} />
								<button
									type="submit"
									class="btn btn-danger btn-sm"
									on:click={(e) => {
										if (!confirm('Are you sure you want to delete this principal?')) {
											e.preventDefault();
										}
									}}
									disabled={isSubmitting}
								>
									Delete
								</button>
							</form>
						</td>
					</tr>
				{/each}
				{#if data.principals.length === 0}
					<tr>
						<td colspan="5" class="empty-state">No principals found</td>
					</tr>
				{/if}
			</tbody>
		</table>
	</div>
</div>

<!-- Sidebar -->
{#if showSidebar}
	<div class="sidebar-overlay" on:click={closeSidebar} role="presentation"></div>
	<div class="sidebar">
		<div class="sidebar-header">
			<h2>{isEditing ? 'Edit Principal' : 'Create Principal'}</h2>
			<button class="close-btn" on:click={closeSidebar}>&times;</button>
		</div>

		<div class="sidebar-content">
			<form
				method="POST"
				action={isEditing ? '?/update' : '?/create'}
				use:enhance={handleFormSubmit}
			>
				{#if isEditing && editingPrincipal}
					<input type="hidden" name="id" value={editingPrincipal.id} />
				{/if}

				<div class="form-group">
					<label for="externalId">External ID</label>
					<input
						type="text"
						id="externalId"
						name="externalId"
						value={editingPrincipal?.externalId || ''}
						required
						disabled={isSubmitting}
					/>
				</div>

				<div class="sidebar-footer">
					<button type="button" class="btn btn-secondary" on:click={closeSidebar} disabled={isSubmitting}>
						Cancel
					</button>
					<button type="submit" class="btn btn-primary" disabled={isSubmitting}>
						{#if isSubmitting}
							{isEditing ? 'Saving...' : 'Creating...'}
						{:else}
							{isEditing ? 'Save' : 'Create'}
						{/if}
					</button>
				</div>
			</form>
		</div>
	</div>
{/if}

<style>
	.container {
		max-width: 1200px;
		margin: 0 auto;
		padding: 2rem;
	}

	.header {
		display: flex;
		justify-content: space-between;
		align-items: center;
		margin-bottom: 2rem;
	}

	.header h1 {
		margin: 0;
		color: #333;
	}

	.error-message {
		background: #fee;
		border: 1px solid #fcc;
		color: #c33;
		padding: 1rem;
		border-radius: 4px;
		margin-bottom: 1rem;
	}

	.grid-container {
		background: white;
		border-radius: 8px;
		box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
		overflow: hidden;
	}

	.data-grid {
		width: 100%;
		border-collapse: collapse;
	}

	.data-grid th,
	.data-grid td {
		padding: 1rem;
		text-align: left;
		border-bottom: 1px solid #eee;
	}

	.data-grid th {
		background: #f8f9fa;
		font-weight: 600;
		color: #333;
	}

	.data-grid tbody tr:hover {
		background: #f8f9fa;
	}

	.actions {
		display: flex;
		gap: 0.5rem;
	}

	.empty-state {
		text-align: center;
		color: #666;
		font-style: italic;
	}

	.btn {
		padding: 0.5rem 1rem;
		border: none;
		border-radius: 4px;
		cursor: pointer;
		font-size: 0.9rem;
		font-weight: 500;
		transition: background-color 0.2s;
	}

	.btn:disabled {
		opacity: 0.6;
		cursor: not-allowed;
	}

	.btn-primary {
		background: #007bff;
		color: white;
	}

	.btn-primary:hover:not(:disabled) {
		background: #0056b3;
	}

	.btn-secondary {
		background: #6c757d;
		color: white;
	}

	.btn-secondary:hover:not(:disabled) {
		background: #545b62;
	}

	.btn-danger {
		background: #dc3545;
		color: white;
	}

	.btn-danger:hover:not(:disabled) {
		background: #c82333;
	}

	.btn-sm {
		padding: 0.25rem 0.5rem;
		font-size: 0.8rem;
	}

	.sidebar-overlay {
		position: fixed;
		top: 0;
		left: 0;
		right: 0;
		bottom: 0;
		background: rgba(0, 0, 0, 0.5);
		z-index: 1000;
	}

	.sidebar {
		position: fixed;
		top: 0;
		right: 0;
		bottom: 0;
		width: 400px;
		background: white;
		box-shadow: -2px 0 8px rgba(0, 0, 0, 0.1);
		z-index: 1001;
		display: flex;
		flex-direction: column;
	}

	.sidebar-header {
		display: flex;
		justify-content: space-between;
		align-items: center;
		padding: 1.5rem;
		border-bottom: 1px solid #eee;
	}

	.sidebar-header h2 {
		margin: 0;
		color: #333;
	}

	.close-btn {
		background: none;
		border: none;
		font-size: 1.5rem;
		cursor: pointer;
		color: #666;
		padding: 0;
		width: 30px;
		height: 30px;
		display: flex;
		align-items: center;
		justify-content: center;
	}

	.close-btn:hover {
		color: #333;
	}

	.sidebar-content {
		flex: 1;
		display: flex;
		flex-direction: column;
		padding: 1.5rem;
	}

	.form-group {
		margin-bottom: 1.5rem;
	}

	.form-group label {
		display: block;
		margin-bottom: 0.5rem;
		font-weight: 500;
		color: #333;
	}

	.form-group input {
		width: 100%;
		padding: 0.75rem;
		border: 1px solid #ddd;
		border-radius: 4px;
		font-size: 1rem;
	}

	.form-group input:focus {
		outline: none;
		border-color: #007bff;
		box-shadow: 0 0 0 2px rgba(0, 123, 255, 0.25);
	}

	.form-group input:disabled {
		background: #f8f9fa;
		cursor: not-allowed;
	}

	.sidebar-footer {
		display: flex;
		gap: 1rem;
		justify-content: flex-end;
		margin-top: auto;
		padding-top: 1rem;
	}

	@media (max-width: 768px) {
		.container {
			padding: 1rem;
		}

		.header {
			flex-direction: column;
			gap: 1rem;
			align-items: stretch;
		}

		.sidebar {
			width: 100%;
		}

		.data-grid {
			font-size: 0.9rem;
		}

		.data-grid th,
		.data-grid td {
			padding: 0.5rem;
		}

		.actions {
			flex-direction: column;
		}
	}
</style>