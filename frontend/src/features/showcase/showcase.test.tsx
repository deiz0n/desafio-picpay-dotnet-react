import { render, screen, within } from '@testing-library/react'
import { describe, expect, it } from 'vitest'
import { Showcase } from '@/features/showcase/showcase'

// O Intl usa espaço sem quebra (U+00A0) entre "R$" e o número.
const normalizer = (texto: string) => texto.replace(/\u00a0/g, ' ').trim()

describe('Showcase', () => {
  it('mostra o saldo formatado em reais', () => {
    render(<Showcase />)

    expect(screen.getByText('R$ 1.500,00', { normalizer })).toBeInTheDocument()
  })

  it('mostra + para recebido e − para enviado, além do selo em texto', () => {
    render(<Showcase />)
    const lista = screen.getByRole('list', { name: 'Últimas movimentações' })

    const recebido = within(lista).getByRole('listitem', { name: /Ana Souza/ })
    expect(
      within(recebido).getByText('+ R$ 200,00', { normalizer }),
    ).toBeInTheDocument()
    expect(within(recebido).getByText('Recebido')).toBeInTheDocument()

    const enviado = within(lista).getByRole('listitem', {
      name: /Padaria do Zé/,
    })
    expect(
      within(enviado).getByText('− R$ 35,90', { normalizer }),
    ).toBeInTheDocument()
    expect(within(enviado).getByText('Enviado')).toBeInTheDocument()
  })

  it('associa o rótulo ao campo', () => {
    render(<Showcase />)

    const campo = screen.getByLabelText('E-mail')

    expect(campo).toHaveAttribute('type', 'email')
  })

  it('mostra os botões das variantes que o produto usa', () => {
    render(<Showcase />)

    for (const nome of ['Transferir', 'Cancelar', 'Excluir conta']) {
      expect(screen.getByRole('button', { name: nome })).toBeInTheDocument()
    }
  })
})
