using Asp.Versioning;
using Asp.Versioning.Builder;
using APIContagem.V1.Models;

namespace APIContagem.V1.Endpoints;

public static class ContagemEndpoints
{
    private static readonly Contador _CONTADOR = new ();

    public static IVersionedEndpointRouteBuilder? MapEndpointsContagemV1(
        this IVersionedEndpointRouteBuilder? versionedBuilder, string routePrefix, ILogger logger)
    {
        var contagemV1 = versionedBuilder!.MapGroup(routePrefix)
            .HasDeprecatedApiVersion(new ApiVersion(1, 0))
            .HasApiVersion(new ApiVersion(1, 1))
            .HasApiVersion(new ApiVersion(1, 2));

        contagemV1.MapGet("/", () =>
        {
            int valorAtualContador;
            lock (_CONTADOR)
            {
                _CONTADOR.Incrementar();
                valorAtualContador = _CONTADOR.ValorAtual;
            }
            logger.LogInformation($"Contador 1.0 (Deprecated) - Valor atual: {valorAtualContador}");
            if (valorAtualContador % 4 == 0)
            {
                logger.LogError("Simulando falha...");
                throw new Exception("Simulação de falha!");
            }
            return TypedResults.Ok(new ResultadoContador()
            {
                ValorAtual = _CONTADOR.ValorAtual,
                Local = _CONTADOR.Local,
                Kernel = _CONTADOR.Kernel,
                Framework = _CONTADOR.Framework,
                Mensagem = $"Versao 1.0 (Deprecated)"
            });
        }).Produces<ResultadoContador>()
          .MapToApiVersion(new ApiVersion(1, 0));

        contagemV1.MapGet("/", () =>
        {
            int valorAtualContador;
            lock (_CONTADOR)
            {
                _CONTADOR.Incrementar();
                valorAtualContador = _CONTADOR.ValorAtual;
            }
            logger.LogInformation($"Contador 1.x - Valor atual: {valorAtualContador}");
            return TypedResults.Ok(new ResultadoContador()
            {
                ValorAtual = _CONTADOR.ValorAtual,
                Local = _CONTADOR.Local,
                Kernel = _CONTADOR.Kernel,
                Framework = _CONTADOR.Framework,
                Mensagem = $"Versao 1.x"
            });
        }).Produces<ResultadoContador>()
          .MapToApiVersion(new ApiVersion(1, 1))
          .MapToApiVersion(new ApiVersion(1, 2));

        return versionedBuilder;
    }
}