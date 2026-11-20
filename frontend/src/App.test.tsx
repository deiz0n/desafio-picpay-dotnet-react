import { render, screen } from '@testing-library/react'
import { describe, expect, it } from 'vitest'
import App from './App'

describe('App', () => {
  it('mostra o nome DuduPay', () => {
    render(<App />)

    expect(
      screen.getByRole('heading', { name: 'DuduPay', level: 1 }),
    ).toBeInTheDocument()
  })

  it('oferece o botão de alternar tema', () => {
    render(<App />)

    expect(
      screen.getByRole('button', { name: /Ativar tema (claro|escuro)/ }),
    ).toBeInTheDocument()
  })

  it('mostra o conteúdo da vitrine', () => {
    render(<App />)

    expect(screen.getByLabelText('E-mail')).toBeInTheDocument()
  })
})
