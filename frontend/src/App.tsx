import { useTasks } from './hooks/useTasks'
import { AddTaskForm } from './components/AddTaskForm'
import { TaskList } from './components/TaskList'

function App() {
  const { tasks, isLoading, error, addTask, toggleTask, removeTask } = useTasks()

  return (
    <div className="flex min-h-screen justify-center bg-[#F5F5F7] px-6 pb-24 pt-18 text-[#1D1D1F] antialiased font-sans">
      <div className="w-full max-w-140">
        <header className="mb-8 flex items-baseline justify-between">
          <h1 className="m-0 text-[34px] font-bold tracking-[-0.02em]">Tasks</h1>
        </header>

        <AddTaskForm onAdd={addTask} />

        {isLoading && <p className="text-center text-[15px] text-[#86868B]">Loading…</p>}
        {error && <p className="text-center text-[15px] text-[#FF3B30]">{error}</p>}
        {!isLoading && !error && <TaskList tasks={tasks} onToggle={toggleTask} onDelete={removeTask} />}
      </div>
    </div>
  )
}

export default App
