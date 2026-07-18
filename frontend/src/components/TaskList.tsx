import type { Task } from '../types/task'
import { TaskItem } from './TaskItem'

interface TaskListProps {
  tasks: Task[]
  onToggle: (id: number) => void
  onDelete: (id: number) => void
}

/**
 * Renders the task list as a bordered card. The empty case here is a plain
 * message — the polished empty state (icon, per-filter copy) is TASK-13.
 */
export function TaskList({ tasks, onToggle, onDelete }: TaskListProps) {
  if (tasks.length === 0) {
    return <p className="px-6 py-[72px] text-center text-[15px] text-[#86868B]">No tasks to show.</p>
  }

  return (
    <div className="overflow-hidden rounded-xl border border-black/5 bg-white shadow-[0_1px_3px_rgba(0,0,0,0.05)]">
      {tasks.map((task, index) => (
        <TaskItem key={task.id} task={task} isFirst={index === 0} onToggle={onToggle} onDelete={onDelete} />
      ))}
    </div>
  )
}
