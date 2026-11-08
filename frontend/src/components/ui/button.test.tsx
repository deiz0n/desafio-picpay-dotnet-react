import { render, screen } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { describe, expect, it, vi } from 'vitest'
import { Button } from '@/components/ui/button'

describe('Button', () => {
  it('é encontrado pelo papel e pelo nome', () => {
    render(<Button>Transferir</Button>)

    expect(
      screen.getByRole('button', { name: 'Transferir' }),
    ).toBeInTheDocument()
  })

  it('dispara o clique e não dispara quando desabilitado', async () => {
    const aoClicar = vi.fn()
    const usuario = userEvent.setup()
    const { rerender } = render(<Button onClick={aoClicar}>Enviar</Button>)

    await usuario.click(screen.getByRole('button', { name: 'Enviar' }))
    expect(aoClicar).toHaveBeenCalledTimes(1)

    rerender(
      <Button onClick={aoClicar} disabled>
        Enviar
      </Button>,
    )
    await usuario.click(screen.getByRole('button', { name: 'Enviar' }))
    expect(aoClicar).toHaveBeenCalledTimes(1)
  })
})
