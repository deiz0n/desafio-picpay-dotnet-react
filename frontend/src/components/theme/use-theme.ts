import { createContext, useContext } from 'react'

export type Theme = 'light' | 'dark'

export type ThemeContextValue = {
  /** Tema em vigor agora (escolha salva ou, na falta dela, o do sistema). */
  theme: Theme
  setTheme: (theme: Theme) => void
  toggleTheme: () => void
}

export const ThemeContext = createContext<ThemeContextValue | null>(null)

export function useTheme(): ThemeContextValue {
  const contexto = useContext(ThemeContext)
  if (!contexto) {
    throw new Error('useTheme precisa estar dentro de um <ThemeProvider>.')
  }
  return contexto
}
