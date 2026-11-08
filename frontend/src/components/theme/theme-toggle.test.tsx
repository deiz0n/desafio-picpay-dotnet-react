import { render, screen } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { describe, expect, it } from 'vitest'
import { ThemeProvider } from '@/components/theme/theme-provider'
import { ThemeToggle } from '@/components/theme/theme-toggle'
import { mockMatchMedia } from '@/test/match-media'

function renderizar() {
  return render(
    <ThemeProvider>
      <ThemeToggle />
    </ThemeProvider>,
  )
}

describe('ThemeToggle', () => {
  it('alterna entre claro e escuro ao clicar', async () => {
    mockMatchMedia(false)
    const usuario = userEvent.setup()
    renderizar()

    await usuario.click(screen.getByRole('button', { name: 'Ativar tema escuro' }))
    expect(document.documentElement).toHaveClass('dark')

    await usuario.click(screen.getByRole('button', { name: 'Ativar tema claro' }))
    expect(document.documentElement).not.toHaveClass('dark')
  })

  it('anuncia a ação que vai acontecer, não o estado atual', () => {
    mockMatchMedia(true)
    renderizar()

    expect(
      screen.getByRole('button', { name: 'Ativar tema claro' }),
    ).toBeInTheDocument()
    expect(
      screen.queryByRole('button', { name: 'Ativar tema escuro' }),
    ).not.toBeInTheDocument()
  })
})
