import '@testing-library/jest-dom/vitest'
import { cleanup } from '@testing-library/react'
import { afterEach, vi } from 'vitest'

// Sem `globals: true` o Testing Library não limpa o DOM sozinho entre os testes.
// Também zeramos o que o tema deixa no navegador simulado (jsdom), senão um
// teste herdaria a escolha de tema do anterior.
afterEach(() => {
  cleanup()
  vi.restoreAllMocks()
  localStorage.clear()
  document.documentElement.classList.remove('dark')
  document.documentElement.style.removeProperty('color-scheme')
})
