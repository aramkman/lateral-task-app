import type { Task, TaskStatusFilter } from '../types/task'
import { TaskItem } from './TaskItem'
import { StatusCard } from './StatusCard'

interface TaskListProps {
  tasks: Task[]
  filter: TaskStatusFilter
  onToggle: (id: number) => void
  onDelete: (id: number) => void
}

const EMPTY_STATE_COPY: Record<TaskStatusFilter, { title: string; subtitle: string }> = {
  All: { title: 'No tasks yet', subtitle: 'Add your first task above to get started.' },
  Active: { title: 'All caught up', subtitle: 'You have no active tasks.' },
  Completed: { title: 'Nothing completed', subtitle: 'Completed tasks will show up here.' },
}

/** Renders the task list as a bordered card, or a per-filter empty state when there's nothing to show. */
export function TaskList({ tasks, filter, onToggle, onDelete }: TaskListProps) {
  if (tasks.length === 0) {
    const copy = EMPTY_STATE_COPY[filter]
    return <StatusCard icon="☰" title={copy.title} subtitle={copy.subtitle} />
  }

  return (
    <div className="overflow-hidden rounded-xl border border-black/5 bg-white shadow-[0_1px_3px_rgba(0,0,0,0.05)]">
      {tasks.map((task, index) => (
        <TaskItem key={task.id} task={task} isFirst={index === 0} onToggle={onToggle} onDelete={onDelete} />
      ))}
    </div>
  )
}
