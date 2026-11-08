// Dinheiro no front: valores circulam em CENTAVOS INTEIROS (docs/frontend.md).
// A API fala em reais com 2 casas (`100.0`), então a conversão acontece só na
// borda, em `reaisToCents`. Somar/comparar centavos evita 0.1 + 0.2 !== 0.3.

const formatadorBRL = new Intl.NumberFormat('pt-BR', {
  style: 'currency',
  currency: 'BRL',
})

/** Formata centavos inteiros como moeda brasileira: 123456 → "R$ 1.234,56". */
export function formatBRL(centavos: number): string {
  if (!Number.isInteger(centavos)) {
    throw new RangeError(
      `formatBRL espera centavos inteiros, recebeu ${centavos}`,
    )
  }
  return formatadorBRL.format(centavos / 100)
}

/** Converte reais (como vêm da API) em centavos inteiros: 100.1 → 10010. */
export function reaisToCents(reais: number): number {
  if (!Number.isFinite(reais)) {
    throw new RangeError(
      `reaisToCents espera um número finito, recebeu ${reais}`,
    )
  }
  // `+ 0` troca -0 por 0 (Math.round(-0.1) devolve -0).
  return Math.round(reais * 100) + 0
}
