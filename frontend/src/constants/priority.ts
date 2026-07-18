import type { TaskPriority } from '../types/task'

/** Display label and dot color per priority. Shared by TaskItem and AddTaskForm. */
export const PRIORITY_META: Record<TaskPriority, { label: string; dot: string }> = {
  Low: { label: 'Low', dot: '#30B14B' },
  Medium: { label: 'Medium', dot: '#F0A020' },
  High: { label: 'High', dot: '#E23B32' },
}

/** Display order for the priority picker (low to high). */
export const PRIORITY_ORDER: TaskPriority[] = ['Low', 'Medium', 'High']
