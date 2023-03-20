using APICatalogoMinimal.Context;
using APICatalogoMinimal.Models;
using Microsoft.EntityFrameworkCore;

namespace APICatalogoMinimal;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

        var app = builder.Build();

        app.MapGet("/", () => "Catalogo de Produtos - 2023");

        app.MapPost("/categorias", async (Categoria categoria, AppDbContext db) =>
        {
            db.Categorias.Add(categoria);
            await db.SaveChangesAsync();

            return Results.Created($"/categorias/{categoria.CategoriaId}", categoria);
        });

        app.MapGet("/categorias", async(AppDbContext db) => await db.Categorias.ToListAsync());

        app.MapGet("/categorias/{id:int}", async (int id, AppDbContext db) =>
        {
            return await db.Categorias.FindAsync(id) is Categoria categoria ? Results.Ok(categoria) : Results.NotFound();
        });

        app.MapPut("/categorias/{id:int}", async (int id, Categoria categoria, AppDbContext db) =>
        {
            if (categoria.CategoriaId != id)
                return Results.BadRequest();
            

            var categoriaDB = await db.Categorias.FindAsync(id);

            if (categoriaDB is null) return Results.NotFound();

            categoriaDB.Nome = categoria.Nome;
            categoriaDB.Descricao = categoria.Descricao;

            await db.SaveChangesAsync();

            return Results.Ok(categoriaDB);

        });

        app.MapDelete("/categorias/{id:int}", async (int id, AppDbContext db) =>
        {
            var categoria = await db.Categorias.FindAsync(id);

            if(categoria is null)
                return Results.NotFound();

            db.Categorias.Remove(categoria);
            await db.SaveChangesAsync();

            return Results.NoContent();
        });

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.Run();
    }
}