import { describe, expect, it } from 'vitest'
import { formatBRL, reaisToCents } from '@/lib/money'

// O Intl separa "R$" do número com um espaço sem quebra (U+00A0). Trocamos por
// um espaço comum só para os testes ficarem legíveis.
const normalizar = (texto: string) => texto.replace(/ /g, ' ')

describe('formatBRL', () => {
  it('formata centavos como moeda brasileira', () => {
    expect(normalizar(formatBRL(123456))).toBe('R$ 1.234,56')
    expect(normalizar(formatBRL(100000))).toBe('R$ 1.000,00')
    expect(normalizar(formatBRL(5))).toBe('R$ 0,05')
  })

  it('formata zero e negativo', () => {
    expect(normalizar(formatBRL(0))).toBe('R$ 0,00')
    expect(normalizar(formatBRL(-525))).toMatch(/^[-−]R\$ 5,25$/)
  })

  it('rejeita centavos que não são inteiros', () => {
    expect(() => formatBRL(10.5)).toThrow(RangeError)
    expect(() => formatBRL(Number.NaN)).toThrow(RangeError)
    expect(() => formatBRL(Number.POSITIVE_INFINITY)).toThrow(RangeError)
  })
})

describe('reaisToCents', () => {
  it('converte valores com duas casas decimais', () => {
    expect(reaisToCents(100.1)).toBe(10010)
    expect(reaisToCents(1234.56)).toBe(123456)
    expect(reaisToCents(1000)).toBe(100000)
    expect(reaisToCents(-5.25)).toBe(-525)
  })

  it('não perde centavo em valores que o ponto flutuante representa mal', () => {
    // No JavaScript, v * 100 sozinho dá: 0.29 → 28.999999999999996,
    // 1.15 → 114.99999999999999, 4.35 → 434.99999999999994 e
    // 19.99 → 1998.9999999999998. Só o arredondamento acerta.
    expect(reaisToCents(0.29)).toBe(29)
    expect(reaisToCents(1.15)).toBe(115)
    expect(reaisToCents(4.35)).toBe(435)
    expect(reaisToCents(19.99)).toBe(1999)
  })

  it('arredonda para o centavo mais próximo', () => {
    // 0.1 + 0.2 === 0.30000000000000004
    expect(reaisToCents(0.1 + 0.2)).toBe(30)
  })

  it('nunca devolve -0', () => {
    expect(Object.is(reaisToCents(-0.001), 0)).toBe(true)
  })

  it('rejeita NaN e infinito', () => {
    expect(() => reaisToCents(Number.NaN)).toThrow(RangeError)
    expect(() => reaisToCents(Number.POSITIVE_INFINITY)).toThrow(RangeError)
  })
})
