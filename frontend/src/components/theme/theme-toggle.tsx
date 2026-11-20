import { Moon, Sun } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { useTheme } from '@/components/theme/use-theme'

export function ThemeToggle() {
  const { theme, toggleTheme } = useTheme()
  const escuro = theme === 'dark'

  return (
    <Button
      variant="ghost"
      size="icon"
      onClick={toggleTheme}
      // O nome diz o que o clique FAZ, não o estado atual: é o que um leitor
      // de tela precisa ouvir.
      aria-label={escuro ? 'Ativar tema claro' : 'Ativar tema escuro'}
    >
      {escuro ? <Sun /> : <Moon />}
    </Button>
  )
}
