import type { Task, TaskStatusFilter } from '../types/task'

// Backend's default dev port (see backend/src/TaskApp.Api/Properties/launchSettings.json).
// Adjust here if the API runs elsewhere.
const API_BASE_URL = 'http://localhost:5211/api/tasks'

/**
 * Throws with a message taken from the API's ProblemDetails body when the
 * response isn't ok, falling back to a generic message if the body isn't JSON.
 */
async function throwIfNotOk(response: Response): Promise<void> {
  if (response.ok) return
  const problem = await response.json().catch(() => null)
  throw new Error(problem?.title ?? `Request failed with status ${response.status}`)
}

/** Fetches tasks, optionally filtered by status. */
export async function getTasks(filter: TaskStatusFilter, signal?: AbortSignal): Promise<Task[]> {
  const response = await fetch(`${API_BASE_URL}?status=${filter}`, { signal })
  await throwIfNotOk(response)
  return response.json() as Promise<Task[]>
}

/** Toggles a task's completed status and returns the updated task. */
export async function toggleTaskStatus(id: number, signal?: AbortSignal): Promise<Task> {
  const response = await fetch(`${API_BASE_URL}/${id}/toggle`, { method: 'PATCH', signal })
  await throwIfNotOk(response)
  return response.json() as Promise<Task>
}

/** Deletes a task. */
export async function deleteTask(id: number, signal?: AbortSignal): Promise<void> {
  const response = await fetch(`${API_BASE_URL}/${id}`, { method: 'DELETE', signal })
  await throwIfNotOk(response)
}
