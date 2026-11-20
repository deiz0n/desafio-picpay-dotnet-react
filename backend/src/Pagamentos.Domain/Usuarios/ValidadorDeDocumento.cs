namespace Pagamentos.Domain.Usuarios;

/// <summary>Valida CPF e CNPJ numéricos (só dígitos) pelos dígitos verificadores.</summary>
public static class ValidadorDeDocumento
{
    public const int TamanhoCpf = 11;
    public const int TamanhoCnpj = 14;

    private static readonly int[] PesosCnpj1 = [5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];
    private static readonly int[] PesosCnpj2 = [6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];

    public static bool SoDigitos(string valor) => valor.Length > 0 && valor.All(char.IsAsciiDigit);

    public static bool CpfValido(string cpf)
    {
        if (cpf.Length != TamanhoCpf || !SoDigitos(cpf) || TodosIguais(cpf))
            return false;

        var digito1 = DigitoCpf(cpf, quantidade: 9);
        var digito2 = DigitoCpf(cpf, quantidade: 10);
        return Valor(cpf[9]) == digito1 && Valor(cpf[10]) == digito2;
    }

    public static bool CnpjValido(string cnpj)
    {
        if (cnpj.Length != TamanhoCnpj || !SoDigitos(cnpj) || TodosIguais(cnpj))
            return false;

        var digito1 = DigitoCnpj(cnpj, PesosCnpj1);
        var digito2 = DigitoCnpj(cnpj, PesosCnpj2);
        return Valor(cnpj[12]) == digito1 && Valor(cnpj[13]) == digito2;
    }

    // Pesos decrescentes a partir de (quantidade + 1); resto 10 vira 0.
    private static int DigitoCpf(string cpf, int quantidade)
    {
        var soma = 0;
        for (var i = 0; i < quantidade; i++)
            soma += Valor(cpf[i]) * (quantidade + 1 - i);

        var resto = soma * 10 % 11;
        return resto == 10 ? 0 : resto;
    }

    private static int DigitoCnpj(string cnpj, int[] pesos)
    {
        var soma = 0;
        for (var i = 0; i < pesos.Length; i++)
            soma += Valor(cnpj[i]) * pesos[i];

        var resto = soma % 11;
        return resto < 2 ? 0 : 11 - resto;
    }

    private static bool TodosIguais(string valor) => valor.All(c => c == valor[0]);

    private static int Valor(char digito) => digito - '0';
}
