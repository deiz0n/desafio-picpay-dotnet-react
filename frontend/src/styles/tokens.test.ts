import { describe, expect, it } from 'vitest'
import css from '@/index.css?raw'
import { contraste } from '@/test/contrast'

// Os tokens DuduPay moram em src/index.css (ADR 0009). `?raw` entrega o texto
// do arquivo sem processar, então um token mudado sem querer derruba o
// contraste aqui.

function lerBloco(seletor: string): Record<string, string> {
  const inicio = css.indexOf(`${seletor} {`)
  if (inicio === -1) return {}
  const fim = css.indexOf('}', inicio)
  const corpo = css.slice(inicio, fim)
  const tokens: Record<string, string> = {}
  for (const [, nome, valor] of corpo.matchAll(/--([\w-]+):\s*([^;]+);/g)) {
    tokens[nome] = valor.trim()
  }
  return tokens
}

const claro = lerBloco(':root')
const escuro = lerBloco('.dark')

// [primeiro plano, fundo, mínimo]: 4,5 para texto (AA) e 3 para componentes de
// interface (WCAG 1.4.11). `border` é decorativo e fica de fora de propósito.
type Par = readonly [string, string, number]

const TEXTO = 4.5
const COMPONENTE = 3

const PARES: readonly Par[] = [
  ['foreground', 'background', TEXTO],
  ['card-foreground', 'card', TEXTO],
  ['popover-foreground', 'popover', TEXTO],
  ['primary-foreground', 'primary', TEXTO],
  ['secondary-foreground', 'secondary', TEXTO],
  ['accent-foreground', 'accent', TEXTO],
  ['muted-foreground', 'background', TEXTO],
  ['muted-foreground', 'card', TEXTO],
  ['muted-foreground', 'muted', TEXTO],
  ['brand', 'background', TEXTO],
  ['brand', 'card', TEXTO],
  ['success', 'background', TEXTO],
  ['success', 'card', TEXTO],
  ['success-foreground', 'success', TEXTO],
  ['destructive', 'background', TEXTO],
  ['destructive', 'card', TEXTO],
  ['destructive-foreground', 'destructive', TEXTO],
  ['input', 'background', COMPONENTE],
  ['input', 'card', COMPONENTE],
  ['ring', 'background', COMPONENTE],
  ['primary', 'background', COMPONENTE],
  ['primary', 'card', COMPONENTE],
]

function reprovados(tema: Record<string, string>): string[] {
  return PARES.flatMap(([frente, fundo, minimo]) => {
    if (!tema[frente] || !tema[fundo]) {
      return [`--${frente} / --${fundo}: token não definido`]
    }
    const razao = contraste(tema[frente], tema[fundo])
    return razao >= minimo
      ? []
      : [`--${frente} sobre --${fundo}: ${razao.toFixed(2)} (mínimo ${minimo})`]
  })
}

describe('tokens', () => {
  it('pares de texto do tema claro passam em AA', () => {
    expect(reprovados(claro)).toEqual([])
  })

  it('pares de texto do tema escuro passam em AA', () => {
    expect(reprovados(escuro)).toEqual([])
  })

  it('claro e escuro definem as mesmas variáveis', () => {
    expect(Object.keys(claro).length).toBeGreaterThan(0)
    const semRaio = (t: Record<string, string>) =>
      Object.keys(t)
        .filter((nome) => nome !== 'radius')
        .sort()
    expect(semRaio(escuro)).toEqual(semRaio(claro))
  })

  it('todas as cores dos temas estão em hexadecimal #RRGGBB', () => {
    const invalidas = [claro, escuro].flatMap((tema) =>
      Object.entries(tema)
        .filter(([nome]) => nome !== 'radius')
        .filter(([, valor]) => !/^#[0-9a-f]{6}$/i.test(valor))
        .map(([nome, valor]) => `--${nome}: ${valor}`),
    )
    expect(invalidas).toEqual([])
  })
})
