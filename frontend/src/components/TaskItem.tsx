import type { Task } from '../types/task'
import { PRIORITY_META } from '../constants/priority'

interface TaskItemProps {
  task: Task
  isFirst: boolean
  onToggle: (id: number) => void
  onDelete: (id: number) => void
}

/** A single row in the task list: checkbox, title, priority dot, delete affordance. */
export function TaskItem({ task, isFirst, onToggle, onDelete }: TaskItemProps) {
  const priority = PRIORITY_META[task.priority]

  return (
    <div
      className={`group flex items-center gap-3.5 px-[18px] py-[15px] transition hover:bg-[#FAFAFB] ${
        isFirst ? '' : 'border-t border-black/5'
      }`}
    >
      <button
        onClick={() => onToggle(task.id)}
        aria-label={task.isCompleted ? 'Mark as active' : 'Mark as completed'}
        className="flex h-[22px] w-[22px] flex-none items-center justify-center rounded-full border-[1.5px] text-[13px] leading-none text-white transition"
        style={{
          borderColor: task.isCompleted ? '#0071E3' : '#D1D1D6',
          background: task.isCompleted ? '#0071E3' : 'transparent',
        }}
      >
        {task.isCompleted ? '✓' : ''}
      </button>
      <span
        className={`flex-1 text-[17px] leading-[1.3] ${
          task.isCompleted ? 'text-[#B0B0B5] line-through' : 'text-[#1D1D1F]'
        }`}
      >
        {task.title}
      </span>
      <span
        className="h-[9px] w-[9px] flex-none rounded-full"
        style={{ background: priority.dot }}
        title={priority.label}
      />
      <button
        onClick={() => onDelete(task.id)}
        title="Delete"
        aria-label="Delete task"
        className="flex-none rounded-md p-1 text-[18px] leading-none text-[#C7C7CC] opacity-0 transition hover:text-[#FF3B30] group-hover:opacity-100"
      >
        ×
      </button>
    </div>
  )
}
