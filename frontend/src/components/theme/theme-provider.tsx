import { useEffect, useMemo, useState, type ReactNode } from 'react'
import { ThemeContext, type Theme, type ThemeContextValue } from './use-theme'

const CHAVE = 'dudupay-theme'
const CONSULTA_ESCURO = '(prefers-color-scheme: dark)'

// O localStorage pode lançar (modo privado, dados de site bloqueados). O tema
// nunca deve quebrar o app por isso: sem storage, só não lembramos a escolha.
function lerEscolhaSalva(): Theme | null {
  try {
    const valor = localStorage.getItem(CHAVE)
    return valor === 'light' || valor === 'dark' ? valor : null
  } catch {
    return null
  }
}

function salvarEscolha(theme: Theme) {
  try {
    localStorage.setItem(CHAVE, theme)
  } catch {
    // sem storage: a escolha vale só nesta sessão
  }
}

function sistemaPrefereEscuro(): boolean {
  return (
    typeof window.matchMedia === 'function' &&
    window.matchMedia(CONSULTA_ESCURO).matches
  )
}

export function ThemeProvider({ children }: { children: ReactNode }) {
  // `escolha` é o que o usuário decidiu; null = ainda segue o sistema.
  const [escolha, setEscolha] = useState<Theme | null>(lerEscolhaSalva)
  const [sistemaEscuro, setSistemaEscuro] = useState(sistemaPrefereEscuro)

  const theme: Theme = escolha ?? (sistemaEscuro ? 'dark' : 'light')

  useEffect(() => {
    if (typeof window.matchMedia !== 'function') return
    const consulta = window.matchMedia(CONSULTA_ESCURO)
    const aoMudar = (evento: MediaQueryListEvent) =>
      setSistemaEscuro(evento.matches)
    consulta.addEventListener('change', aoMudar)
    return () => consulta.removeEventListener('change', aoMudar)
  }, [])

  useEffect(() => {
    const raiz = document.documentElement
    raiz.classList.toggle('dark', theme === 'dark')
    raiz.style.colorScheme = theme
  }, [theme])

  const valor = useMemo<ThemeContextValue>(() => {
    const setTheme = (novo: Theme) => {
      setEscolha(novo)
      salvarEscolha(novo)
    }
    return {
      theme,
      setTheme,
      toggleTheme: () => setTheme(theme === 'dark' ? 'light' : 'dark'),
    }
  }, [theme])

  return <ThemeContext.Provider value={valor}>{children}</ThemeContext.Provider>
}
