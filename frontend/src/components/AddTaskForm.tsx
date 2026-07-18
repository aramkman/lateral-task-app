import { useState } from 'react'
import type { TaskPriority } from '../types/task'
import { PRIORITY_META, PRIORITY_ORDER } from '../constants/priority'

interface AddTaskFormProps {
  onAdd: (title: string, priority: TaskPriority) => void
}

/** Title input + priority picker + submit, matching the reference "add row". */
export function AddTaskForm({ onAdd }: AddTaskFormProps) {
  const [title, setTitle] = useState('')
  const [priority, setPriority] = useState<TaskPriority>('Medium')

  const submit = () => {
    // Trimmed/non-empty and the 200-char maxLength below mirror the backend's
    // own validation rules, so the common invalid cases never reach the API.
    const trimmed = title.trim()
    if (!trimmed) return
    onAdd(trimmed, priority)
    setTitle('')
  }

  return (
    <div className="mb-5 flex items-center gap-2.5 rounded-xl border border-black/5 bg-white py-2 pl-[18px] pr-2 shadow-[0_1px_2px_rgba(0,0,0,0.04)]">
      <input
        value={title}
        onChange={(event) => setTitle(event.target.value)}
        onKeyDown={(event) => event.key === 'Enter' && submit()}
        placeholder="Add a new task"
        maxLength={200}
        className="flex-1 border-none bg-transparent py-2 text-[17px] text-[#1D1D1F] outline-none placeholder:text-[#b0b0b5]"
      />
      <div className="flex gap-[3px] rounded-lg bg-[#F5F5F7] p-[3px]">
        {PRIORITY_ORDER.map((key) => {
          const meta = PRIORITY_META[key]
          const active = priority === key
          return (
            <button
              key={key}
              onClick={() => setPriority(key)}
              title={meta.label}
              className={`flex items-center gap-1.5 rounded-md px-2.5 py-1.5 text-[13px] font-medium transition ${
                active ? 'bg-white text-[#1D1D1F] shadow-[0_1px_2px_rgba(0,0,0,0.12)]' : 'text-[#86868B]'
              }`}
            >
              <span className="h-2 w-2 rounded-full" style={{ background: meta.dot }} />
              {meta.label}
            </button>
          )
        })}
      </div>
      <button
        onClick={submit}
        className="rounded-lg bg-[#0071E3] px-[18px] py-[9px] text-[15px] font-medium text-white transition hover:bg-[#005bb8]"
      >
        Add
      </button>
    </div>
  )
}
