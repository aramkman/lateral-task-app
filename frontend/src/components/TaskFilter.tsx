import type { TaskStatusFilter } from '../types/task'

interface TaskFilterProps {
  value: TaskStatusFilter
  onChange: (filter: TaskStatusFilter) => void
}

const FILTERS: TaskStatusFilter[] = ['All', 'Active', 'Completed']

/** Segmented control for the all/active/completed status filter. */
export function TaskFilter({ value, onChange }: TaskFilterProps) {
  return (
    <div className="mb-6 flex justify-center">
      <div className="inline-flex gap-0.5 rounded-[9px] bg-[#E8E8EA] p-0.5">
        {FILTERS.map((key) => {
          const active = value === key
          return (
            <button
              key={key}
              onClick={() => onChange(key)}
              className={`rounded-[7px] px-[22px] py-1.5 text-[14px] font-medium transition ${
                active ? 'bg-white text-[#1D1D1F] shadow-[0_1px_3px_rgba(0,0,0,0.14)]' : 'text-[#636366]'
              }`}
            >
              {key}
            </button>
          )
        })}
      </div>
    </div>
  )
}
