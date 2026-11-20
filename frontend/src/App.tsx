import { ThemeProvider } from '@/components/theme/theme-provider'
import { ThemeToggle } from '@/components/theme/theme-toggle'
import { Showcase } from '@/features/showcase/showcase'

function App() {
  return (
    <ThemeProvider>
      <div className="min-h-svh">
        <header className="mx-auto flex max-w-3xl items-center justify-between px-4 py-4">
          <h1 className="text-xl font-semibold text-brand">DuduPay</h1>
          <ThemeToggle />
        </header>
        <main className="mx-auto max-w-3xl px-4 pb-8">
          <Showcase />
        </main>
      </div>
    </ThemeProvider>
  )
}

export default App
