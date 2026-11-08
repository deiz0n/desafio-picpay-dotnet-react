import { describe, expect, it } from 'vitest'
import { contraste } from './contrast'

describe('contraste', () => {
  it('dá 21 para preto sobre branco', () => {
    expect(contraste('#000000', '#FFFFFF')).toBeCloseTo(21, 5)
  })

  it('dá 1 para duas cores iguais e não depende da ordem', () => {
    expect(contraste('#6D28D9', '#6D28D9')).toBeCloseTo(1, 5)
    expect(contraste('#6D28D9', '#FFFFFF')).toBeCloseTo(
      contraste('#FFFFFF', '#6D28D9'),
      10,
    )
  })

  it('reproduz o valor medido do violeta DuduPay (7,10 com texto branco)', () => {
    expect(contraste('#FFFFFF', '#6D28D9')).toBeCloseTo(7.1, 1)
  })

  it('rejeita cor fora do formato #RRGGBB', () => {
    expect(() => contraste('oklch(1 0 0)', '#FFFFFF')).toThrow(/inválida/)
  })
})
