using Microsoft.Extensions.Options;
using Asp.Versioning;
using Swashbuckle.AspNetCore.SwaggerGen;
using APIContagem.Versioning;
using APIContagem.V1.Endpoints;
using APIContagem.V2.Endpoints;

// GitHub do ASP.NET API Versioning:
// https://github.com/microsoft/aspnet-api-versioning

// GitHub do projeto que utilizei como base para a
// a implementacaoo desta aplicacao:
// https://github.com/dotnet/aspnet-api-versioning/tree/2292fbe6a1598d944cd5cbca918cb79da7339116/examples/AspNetCore/WebApi/MinimalOpenApiExample

// Algumas referencias sobre ASP.NET API Versioning:
// https://renatogroffe.medium.com/net-7-asp-net-core-versionamento-de-apis-rest-em-um-exemplo-simples-8e780a05b249
// https://renatogroffe.medium.com/dica-asp-net-core-versionando-apis-rest-sem-grandes-complica%C3%A7%C3%B5es-1ff3cfcefb2d

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services
    .AddApiVersioning(options =>
        {
            // Retorna os headers "api-supported-versions" e "api-deprecated-versions"
            // indicando versoes suportadas pela API e o que esta como deprecated
            options.ReportApiVersions = true;

            options.AssumeDefaultVersionWhenUnspecified = true;
            options.DefaultApiVersion = new ApiVersion(1, 2);
        })
    .AddApiExplorer(
        options =>
        {
            // Agrupar por numero de versao
            // add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
            // note: the specified format code will format the version as "'v'major[.minor][-status]"
            options.GroupNameFormat = "'v'VVV";

            // Necessario para o correto funcionamento das rotas
            // note: this option is only necessary when versioning by url segment. the SubstitutionFormat
            // can also be used to control the format of the API version in route templates
            options.SubstituteApiVersionInUrl = true;
        })
    // this enables binding ApiVersion as a endpoint callback parameter. if you don't use it, then
    // you should remove this configuration.
    .EnableApiVersionBinding();
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
builder.Services.AddSwaggerGen( options => options.OperationFilter<SwaggerDefaultValues>() );

builder.Services.AddCors();

var app = builder.Build();

app.UseCors(policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

var apiContagem = app.NewVersionedApi("Contagem");
apiContagem.MapEndpointsContagemV1("/contador", app.Logger);
apiContagem.MapEndpointsContagemV1("/v{version:apiVersion}/contador", app.Logger);
apiContagem.MapEndpointsContagemV2("/contador", app.Logger);
apiContagem.MapEndpointsContagemV2("/v{version:apiVersion}/contador", app.Logger);

app.UseSwagger();
app.UseSwaggerUI(
    options =>
    {
        var descriptions = app.DescribeApiVersions();

        // build a swagger endpoint for each discovered API version
        foreach ( var description in descriptions )
        {
            var url = $"/swagger/{description.GroupName}/swagger.json";
            var name = description.GroupName.ToUpperInvariant();
            options.SwaggerEndpoint( url, name );
        }
    } );

app.Run();