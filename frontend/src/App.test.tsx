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
})
