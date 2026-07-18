import { useTasks } from './hooks/useTasks'
import { AddTaskForm } from './components/AddTaskForm'
import { TaskFilter } from './components/TaskFilter'
import { TaskList } from './components/TaskList'
import { StatusCard } from './components/StatusCard'

function App() {
  const { tasks, remainingCount, isLoading, error, filter, setFilter, addTask, toggleTask, removeTask } = useTasks()

  return (
    <div className="flex min-h-screen justify-center bg-[#F5F5F7] px-6 pb-24 pt-18 text-[#1D1D1F] antialiased font-sans">
      <div className="w-full max-w-140">
        <header className="mb-8 flex items-baseline justify-between">
          <h1 className="m-0 text-[34px] font-bold tracking-[-0.02em]">Tasks</h1>
          <span className="text-[15px] font-normal text-[#86868B]">
            {remainingCount === 0 ? 'All done' : `${remainingCount} remaining`}
          </span>
        </header>

        <AddTaskForm onAdd={addTask} />

        <TaskFilter value={filter} onChange={setFilter} />

        {isLoading && (
          <div className="flex justify-center py-18">
            <div className="h-6 w-6 animate-spin rounded-full border-2 border-[#E5E4E7] border-t-[#0071E3]" />
          </div>
        )}
        {!isLoading && error && <StatusCard icon="⚠" title="Something went wrong" subtitle={error} />}
        {!isLoading && !error && (
          <TaskList tasks={tasks} filter={filter} onToggle={toggleTask} onDelete={removeTask} />
        )}
      </div>
    </div>
  )
}

export default App
