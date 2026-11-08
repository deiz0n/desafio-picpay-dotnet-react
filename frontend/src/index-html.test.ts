import { describe, expect, it, vi } from 'vitest'
import html from '../index.html?raw'
import { mockMatchMedia } from '@/test/match-media'

// O index.html tem um script inline que aplica o tema ANTES do React montar,
// para quem usa o tema escuro não ver um flash claro. Ele repete a regra do
// ThemeProvider (mesma chave, mesma prioridade), então estes testes vigiam que
// os dois não se separem.
function executarScriptDoIndex() {
  const script = html.match(/<script>([\s\S]*?)<\/script>/)?.[1]
  if (!script)
    throw new Error('script inline do tema não encontrado no index.html')
  new Function(script)()
}

const raiz = () => document.documentElement

describe('index.html (tema antes do React)', () => {
  it('aplica dark quando a escolha salva é dark', () => {
    mockMatchMedia(false)
    localStorage.setItem('dudupay-theme', 'dark')

    executarScriptDoIndex()

    expect(raiz()).toHaveClass('dark')
  })

  it('não aplica dark quando a escolha salva é light, mesmo com o sistema escuro', () => {
    mockMatchMedia(true)
    localStorage.setItem('dudupay-theme', 'light')

    executarScriptDoIndex()

    expect(raiz()).not.toHaveClass('dark')
  })

  it('usa o sistema quando não há escolha salva', () => {
    mockMatchMedia(true)

    executarScriptDoIndex()

    expect(raiz()).toHaveClass('dark')
  })

  it('ignora valor inválido salvo e usa o sistema', () => {
    mockMatchMedia(true)
    localStorage.setItem('dudupay-theme', 'roxo')

    executarScriptDoIndex()

    expect(raiz()).toHaveClass('dark')
  })

  it('não quebra quando o localStorage lança erro', () => {
    mockMatchMedia(true)
    vi.spyOn(Storage.prototype, 'getItem').mockImplementation(() => {
      throw new Error('storage bloqueado')
    })

    expect(() => executarScriptDoIndex()).not.toThrow()
    expect(raiz()).toHaveClass('dark')
  })
})
