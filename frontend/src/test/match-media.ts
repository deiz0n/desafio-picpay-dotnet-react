import { vi } from 'vitest'

// O jsdom não implementa `window.matchMedia`. Este helper simula a preferência
// de tema do sistema e permite "mudar" a preferência no meio do teste.
export function mockMatchMedia(prefereEscuro: boolean) {
  let atual = prefereEscuro
  const ouvintes = new Set<(evento: MediaQueryListEvent) => void>()

  Object.defineProperty(window, 'matchMedia', {
    writable: true,
    configurable: true,
    value: vi.fn((consulta: string) => ({
      get matches() {
        return consulta.includes('prefers-color-scheme: dark') ? atual : false
      },
      media: consulta,
      addEventListener: (
        _tipo: string,
        ouvinte: (evento: MediaQueryListEvent) => void,
      ) => ouvintes.add(ouvinte),
      removeEventListener: (
        _tipo: string,
        ouvinte: (evento: MediaQueryListEvent) => void,
      ) => ouvintes.delete(ouvinte),
    })),
  })

  return {
    mudarSistemaPara(escuro: boolean) {
      atual = escuro
      ouvintes.forEach((ouvinte) =>
        ouvinte({ matches: escuro } as MediaQueryListEvent),
      )
    },
  }
}
