import type { Task, TaskPriority, TaskStatusFilter } from '../types/task'

// Backend's default dev port (see backend/src/TaskApp.Api/Properties/launchSettings.json).
// Adjust here if the API runs elsewhere.
const API_BASE_URL = 'http://localhost:5211/api/tasks'

/**
 * Runs `fetch`, replacing the browser's raw network-failure message (e.g.
 * "Failed to fetch") with user-facing copy. Deliberate aborts (component
 * unmount) still throw an AbortError — callers already check the signal
 * before showing anything, so rewording it here is harmless.
 */
async function safeFetch(input: RequestInfo, init?: RequestInit): Promise<Response> {
  try {
    return await fetch(input, init)
  } catch {
    throw new Error('Could not reach the server. Check your connection and try again.')
  }
}

/**
 * Throws with a message taken from the API's ProblemDetails body when the
 * response isn't ok, falling back to a generic message if the body isn't JSON.
 */
async function throwIfNotOk(response: Response): Promise<void> {
  if (response.ok) return
  const problem = await response.json().catch(() => null)
  throw new Error(problem?.title ?? `Request failed with status ${response.status}`)
}

/** Creates a task and returns it as persisted (with its assigned id and timestamps). */
export async function createTask(title: string, priority: TaskPriority): Promise<Task> {
  const response = await safeFetch(API_BASE_URL, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ title, priority }),
  })
  await throwIfNotOk(response)
  return response.json() as Promise<Task>
}

/**
 * Fetches tasks, optionally filtered by status. Takes an AbortSignal (unlike
 * the mutations below) because it's called from an effect on mount: under
 * React StrictMode's dev-only double-invoke, the first invocation's request
 * needs to be cancellable so it doesn't call setState after that first,
 * synthetic cleanup runs.
 */
export async function getTasks(filter: TaskStatusFilter, signal?: AbortSignal): Promise<Task[]> {
  const response = await safeFetch(`${API_BASE_URL}?status=${filter}`, { signal })
  await throwIfNotOk(response)
  return response.json() as Promise<Task[]>
}

/** Toggles a task's completed status and returns the updated task. */
export async function toggleTaskStatus(id: number): Promise<Task> {
  const response = await safeFetch(`${API_BASE_URL}/${id}/toggle`, { method: 'PATCH' })
  await throwIfNotOk(response)
  return response.json() as Promise<Task>
}

/** Deletes a task. */
export async function deleteTask(id: number): Promise<void> {
  const response = await safeFetch(`${API_BASE_URL}/${id}`, { method: 'DELETE' })
  await throwIfNotOk(response)
}
