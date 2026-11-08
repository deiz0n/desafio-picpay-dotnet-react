import { describe, expect, it } from 'vitest'
import { cn } from '@/lib/utils'

describe('cn', () => {
  it('mescla classes e mantém a última em conflito', () => {
    expect(cn('px-2', 'px-4')).toBe('px-4')
  })

  it('ignora valores falsos e junta o restante', () => {
    const escondido = false as boolean

    expect(cn('flex', escondido && 'hidden', undefined, 'gap-2')).toBe(
      'flex gap-2',
    )
  })
})
