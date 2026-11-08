import { ArrowDownLeft, ArrowUpRight } from 'lucide-react'
import { Badge } from '@/components/ui/badge'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardHeader } from '@/components/ui/card'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { formatBRL } from '@/lib/money'

// Vitrine TEMPORÁRIA da fundação (F1): mostra os tokens, os componentes e as
// regras de cor do dinheiro nos dois temas. As telas reais (F4 em diante) a
// substituem. Os valores abaixo são de exemplo, em centavos.
const SALDO = 150_000

// O sinal e o selo em texto garantem que "recebido" e "enviado" não dependam só
// da cor (ADR 0009).
const MOVIMENTACOES = [
  { id: 'mov-1', nome: 'Ana Souza', tipo: 'recebido', centavos: 20_000 },
  { id: 'mov-2', nome: 'Padaria do Zé', tipo: 'enviado', centavos: 3_590 },
] as const

export function Showcase() {
  return (
    <div className="grid gap-4">
      <Card>
        <CardHeader>
          <h2 className="text-sm font-medium text-muted-foreground">
            Saldo disponível
          </h2>
        </CardHeader>
        <CardContent>
          <p className="text-3xl font-semibold tabular-nums">
            {formatBRL(SALDO)}
          </p>
        </CardContent>
      </Card>

      <Card>
        <CardHeader>
          <h2 id="movimentacoes-titulo" className="text-base font-medium">
            Últimas movimentações
          </h2>
        </CardHeader>
        <CardContent>
          <ul
            aria-labelledby="movimentacoes-titulo"
            className="divide-y divide-border"
          >
            {MOVIMENTACOES.map((mov) => {
              const recebido = mov.tipo === 'recebido'
              const Icone = recebido ? ArrowDownLeft : ArrowUpRight
              return (
                <li
                  key={mov.id}
                  aria-labelledby={mov.id}
                  className="flex items-center justify-between gap-3 py-3"
                >
                  <div className="flex items-center gap-3">
                    <Icone
                      aria-hidden="true"
                      className={recebido ? 'size-4 text-success' : 'size-4'}
                    />
                    <span id={mov.id}>{mov.nome}</span>
                    <Badge variant={recebido ? 'success' : 'secondary'}>
                      {recebido ? 'Recebido' : 'Enviado'}
                    </Badge>
                  </div>
                  <span
                    className={
                      recebido
                        ? 'font-medium text-success tabular-nums'
                        : 'font-medium tabular-nums'
                    }
                  >
                    {recebido ? '+' : '−'} {formatBRL(mov.centavos)}
                  </span>
                </li>
              )
            })}
          </ul>
        </CardContent>
      </Card>

      <Card>
        <CardHeader>
          <h2 className="text-base font-medium">Botões</h2>
        </CardHeader>
        <CardContent className="flex flex-wrap gap-2">
          <Button>Transferir</Button>
          <Button variant="outline">Cancelar</Button>
          <Button variant="secondary">Salvar rascunho</Button>
          <Button variant="ghost">Ver extrato</Button>
          <Button variant="destructive">Excluir conta</Button>
          <Button variant="link">Esqueci minha senha</Button>
        </CardContent>
      </Card>

      <Card>
        <CardHeader>
          <h2 className="text-base font-medium">Campo de formulário</h2>
        </CardHeader>
        <CardContent className="grid max-w-sm gap-2">
          <Label htmlFor="showcase-email">E-mail</Label>
          <Input
            id="showcase-email"
            type="email"
            placeholder="voce@exemplo.com"
            autoComplete="email"
          />
        </CardContent>
      </Card>
    </div>
  )
}
