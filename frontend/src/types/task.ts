/** Priority levels, mirrors the backend's `TaskPriority` enum (serialized as text). */
export type TaskPriority = 'Low' | 'Medium' | 'High'

/** Status filter accepted by the list endpoint's `status` query param. */
export type TaskStatusFilter = 'All' | 'Active' | 'Completed'

/** A task as returned by the API. Mirrors the backend's `TaskDto`. */
export interface Task {
  id: number
  title: string
  isCompleted: boolean
  priority: TaskPriority
  createdAt: string
  completedAt: string | null
}
