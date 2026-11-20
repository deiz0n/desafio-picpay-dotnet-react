namespace Pagamentos.Domain.Erros;

/// <summary>
/// Resultado de uma operação que pode falhar de forma esperada (regra de negócio),
/// sem usar exceção para controlar o fluxo.
/// </summary>
public class Result
{
    protected Result(Erro? erro) => Erro = erro;

    public Erro? Erro { get; }

    public bool Sucesso => Erro is null;

    public static Result Ok() => new(null);

    public static Result<T> Ok<T>(T valor) => Result<T>.Ok(valor);

    public static Result Falha(Erro erro) => new(erro);
}

/// <summary>Resultado que, em caso de sucesso, carrega um valor.</summary>
public sealed class Result<T> : Result
{
    private readonly T? _valor;

    private Result(T? valor, Erro? erro)
        : base(erro) => _valor = valor;

    public T Valor =>
        Sucesso ? _valor! : throw new InvalidOperationException("Result em falha não tem valor.");

    internal static Result<T> Ok(T valor) => new(valor, null);

    public static new Result<T> Falha(Erro erro) => new(default, erro);

    /// <summary>Permite <c>return erro;</c> em métodos que devolvem <c>Result&lt;T&gt;</c>.</summary>
    public static implicit operator Result<T>(Erro erro) => Falha(erro);
}
