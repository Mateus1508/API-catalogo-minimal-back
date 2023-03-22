using APICatalogoMinimal.ApiEndpoints;
using APICatalogoMinimal.AppServicesExtensions;
using APICatalogoMinimal.Context;
using APICatalogoMinimal.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace APICatalogoMinimal;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.AddApiSwagger();
        builder.AddPersistence();
        builder.Services.AddCors();
        builder.AddAutenticationJwt();
       
        var app = builder.Build();
        
        app.MapAutenticacaoEndpoints();
        app.MapCategoriasEndpoints();
        app.MapProdutosEndpoints();

        var enviroment = app.Environment;
        app.UseExceptionHandling(enviroment)
            .UseSwaggerMiddleware()
            .UseAppCors();

        app.UseAuthentication();
        app.UseAuthorization();

        app.Run();
    }
}