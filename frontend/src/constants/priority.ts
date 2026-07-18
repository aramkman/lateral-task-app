import type { TaskPriority } from '../types/task'

/** Display label and dot color per priority. Shared by TaskItem and, later, AddTaskForm. */
export const PRIORITY_META: Record<TaskPriority, { label: string; dot: string }> = {
  Low: { label: 'Low', dot: '#30B14B' },
  Medium: { label: 'Medium', dot: '#F0A020' },
  High: { label: 'High', dot: '#E23B32' },
}
