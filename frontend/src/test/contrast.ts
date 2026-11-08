// Cálculo de contraste do WCAG 2.x (https://www.w3.org/TR/WCAG21/#dfn-contrast-ratio).
// Usado só pelos testes dos tokens de design.

export type Rgb = readonly [number, number, number]

export function parseHex(hex: string): Rgb {
  if (!/^#[0-9a-f]{6}$/i.test(hex)) {
    throw new Error(`Cor inválida (esperado #RRGGBB): ${hex}`)
  }
  return [1, 3, 5].map((i) =>
    parseInt(hex.slice(i, i + 2), 16),
  ) as unknown as Rgb
}

function luminanciaRelativa([r, g, b]: Rgb): number {
  const [R, G, B] = [r, g, b].map((canal) => {
    const c = canal / 255
    return c <= 0.03928 ? c / 12.92 : ((c + 0.055) / 1.055) ** 2.4
  })
  return 0.2126 * R + 0.7152 * G + 0.0722 * B
}

export function contraste(corA: string, corB: string): number {
  const [maior, menor] = [
    luminanciaRelativa(parseHex(corA)),
    luminanciaRelativa(parseHex(corB)),
  ].sort((x, y) => y - x)
  return (maior + 0.05) / (menor + 0.05)
}
