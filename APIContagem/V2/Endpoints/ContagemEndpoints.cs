using Asp.Versioning;
using Asp.Versioning.Builder;
using APIContagem.V2.Models;

namespace APIContagem.V2.Endpoints;

public static class ContagemEndpoints
{
    private static readonly Contador _CONTADOR = new ();

    public static IVersionedEndpointRouteBuilder? MapEndpointsContagemV2(
        this IVersionedEndpointRouteBuilder? versionedBuilder, string routePrefix, ILogger logger)
    {
        var contagemV2 = versionedBuilder!.MapGroup(routePrefix)
            .HasApiVersion(new ApiVersion(2, 0));

        contagemV2.MapGet("/", () =>
        {
            int valorAtualContador;
            lock (_CONTADOR)
            {
                _CONTADOR.Incrementar();
                valorAtualContador = _CONTADOR.ValorAtual;
            }
            logger.LogInformation($"Contador 2.0 - Valor atual: {valorAtualContador}");
            return TypedResults.Ok(new ResultadoContador()
            {
                ValorAtual = _CONTADOR.ValorAtual,
                Versao = "2.0",
                Local = _CONTADOR.Local,
                Kernel = _CONTADOR.Kernel,
                Framework = _CONTADOR.Framework,
                Mensagem = "Testes com Minimal APIs e API Versioning"
            });
        }).Produces<ResultadoContador>()
          .MapToApiVersion(new ApiVersion(2, 0));

        return versionedBuilder;
    }
}