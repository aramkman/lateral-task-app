import { useCallback, useEffect, useMemo, useState } from 'react'
import { createTask, deleteTask, getTasks, toggleTaskStatus } from '../api/tasksApi'
import type { Task, TaskPriority, TaskStatusFilter } from '../types/task'

interface UseTasksResult {
  tasks: Task[]
  remainingCount: number
  isLoading: boolean
  error: string | null
  filter: TaskStatusFilter
  setFilter: (filter: TaskStatusFilter) => void
  addTask: (title: string, priority: TaskPriority) => Promise<void>
  toggleTask: (id: number) => Promise<void>
  removeTask: (id: number) => Promise<void>
}

/**
 * Encapsulates task list state and all API interaction, so components stay
 * presentation-only (no fetch calls inside components).
 *
 * Fetches the full task list once and filters it client-side (rather than
 * refetching from the server on every filter change). This keeps filter
 * switching instant with no loading flash, and makes `remainingCount` a
 * simple derived value instead of a second round trip — the backend's
 * `status` query param stays fully implemented for other callers, the
 * frontend just doesn't need it for this UX.
 */
export function useTasks(): UseTasksResult {
  const [allTasks, setAllTasks] = useState<Task[]>([])
  const [filter, setFilter] = useState<TaskStatusFilter>('All')
  const [isLoading, setIsLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    const controller = new AbortController()

    async function loadTasks() {
      setIsLoading(true)
      setError(null)
      try {
        const data = await getTasks('All', controller.signal)
        setAllTasks(data)
      } catch (err) {
        if (controller.signal.aborted) return
        setError(err instanceof Error ? err.message : 'Failed to load tasks.')
      } finally {
        if (!controller.signal.aborted) setIsLoading(false)
      }
    }

    loadTasks()

    return () => controller.abort()
  }, [])

  const tasks = useMemo(() => {
    switch (filter) {
      case 'Active':
        return allTasks.filter((task) => !task.isCompleted)
      case 'Completed':
        return allTasks.filter((task) => task.isCompleted)
      default:
        return allTasks
    }
  }, [allTasks, filter])

  const remainingCount = useMemo(() => allTasks.filter((task) => !task.isCompleted).length, [allTasks])

  const addTask = useCallback(async (title: string, priority: TaskPriority) => {
    try {
      const created = await createTask(title, priority)
      setAllTasks((current) => [...current, created])
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to create the task.')
    }
  }, [])

  // Update local state directly from the response instead of refetching the
  // whole list — fewer round trips, and the list doesn't flicker on toggle.
  const toggleTask = useCallback(async (id: number) => {
    try {
      const updated = await toggleTaskStatus(id)
      setAllTasks((current) => current.map((task) => (task.id === id ? updated : task)))
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to update the task.')
    }
  }, [])

  const removeTask = useCallback(async (id: number) => {
    try {
      await deleteTask(id)
      setAllTasks((current) => current.filter((task) => task.id !== id))
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to delete the task.')
    }
  }, [])

  return { tasks, remainingCount, isLoading, error, filter, setFilter, addTask, toggleTask, removeTask }
}
