import { act, render, screen } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { describe, expect, it, vi } from 'vitest'
import { ThemeProvider } from '@/components/theme/theme-provider'
import { useTheme } from '@/components/theme/use-theme'
import { mockMatchMedia } from '@/test/match-media'

const CHAVE = 'dudupay-theme'

function Sonda() {
  const { theme, toggleTheme } = useTheme()
  return (
    <button type="button" onClick={toggleTheme}>
      tema: {theme}
    </button>
  )
}

function renderizar() {
  return render(
    <ThemeProvider>
      <Sonda />
    </ThemeProvider>,
  )
}

const html = () => document.documentElement

describe('ThemeProvider', () => {
  it('usa a preferência do sistema quando não há escolha salva', () => {
    mockMatchMedia(true)
    renderizar()

    expect(screen.getByRole('button', { name: 'tema: dark' })).toBeInTheDocument()
    expect(html()).toHaveClass('dark')
  })

  it('usa o tema claro quando o sistema prefere claro e não há escolha salva', () => {
    mockMatchMedia(false)
    renderizar()

    expect(screen.getByRole('button', { name: 'tema: light' })).toBeInTheDocument()
    expect(html()).not.toHaveClass('dark')
  })

  it('aplica dark no html e grava a escolha ao alternar', async () => {
    mockMatchMedia(false)
    const usuario = userEvent.setup()
    renderizar()

    await usuario.click(screen.getByRole('button', { name: 'tema: light' }))

    expect(html()).toHaveClass('dark')
    expect(localStorage.getItem(CHAVE)).toBe('dark')

    await usuario.click(screen.getByRole('button', { name: 'tema: dark' }))

    expect(html()).not.toHaveClass('dark')
    expect(localStorage.getItem(CHAVE)).toBe('light')
  })

  it('respeita a escolha salva mesmo contra a preferência do sistema', () => {
    mockMatchMedia(true)
    localStorage.setItem(CHAVE, 'light')
    renderizar()

    expect(screen.getByRole('button', { name: 'tema: light' })).toBeInTheDocument()
    expect(html()).not.toHaveClass('dark')
  })

  it('ignora valor inválido salvo e usa o sistema', () => {
    mockMatchMedia(true)
    localStorage.setItem(CHAVE, 'roxo')
    renderizar()

    expect(screen.getByRole('button', { name: 'tema: dark' })).toBeInTheDocument()
    expect(html()).toHaveClass('dark')
  })

  it('acompanha a mudança do sistema enquanto não há escolha salva', () => {
    const sistema = mockMatchMedia(false)
    renderizar()
    expect(html()).not.toHaveClass('dark')

    act(() => sistema.mudarSistemaPara(true))

    expect(screen.getByRole('button', { name: 'tema: dark' })).toBeInTheDocument()
    expect(html()).toHaveClass('dark')
  })

  it('continua funcionando quando o localStorage lança erro', async () => {
    mockMatchMedia(false)
    vi.spyOn(Storage.prototype, 'getItem').mockImplementation(() => {
      throw new Error('storage bloqueado')
    })
    vi.spyOn(Storage.prototype, 'setItem').mockImplementation(() => {
      throw new Error('storage bloqueado')
    })
    const usuario = userEvent.setup()
    renderizar()

    await usuario.click(screen.getByRole('button', { name: 'tema: light' }))

    expect(screen.getByRole('button', { name: 'tema: dark' })).toBeInTheDocument()
    expect(html()).toHaveClass('dark')
  })

  it('useTheme lança erro claro fora do ThemeProvider', () => {
    vi.spyOn(console, 'error').mockImplementation(() => {})

    expect(() => render(<Sonda />)).toThrow(/ThemeProvider/)
  })
})
