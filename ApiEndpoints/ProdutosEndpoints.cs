using APICatalogoMinimal.Context;
using APICatalogoMinimal.Models;
using Microsoft.EntityFrameworkCore;

namespace APICatalogoMinimal.ApiEndpoints;

public static class ProdutosEndpoints
{
    public static void MapProdutosEndpoints(this WebApplication app)
    {
        app.MapGet("/produtos", async (AppDbContext db) => await db.Produtos.ToListAsync());

        app.MapGet("/produtos/{id:int}", async (int id, AppDbContext db) =>
        {
            return await db.Produtos.FindAsync(id) is Produto produto ? Results.Ok(produto) : Results.NotFound();
        });

        app.MapPost("/produtos", async (Produto produto, AppDbContext db) =>
        {
            db.Produtos.Add(produto);
            await db.SaveChangesAsync();

            return Results.Created($"/produtos/{produto.ProdutoId}", produto);
        }).RequireAuthorization();


        app.MapPut("/produtos/{id:int}", async (int id, Produto produto, AppDbContext db) =>
        {
            if (produto.ProdutoId != id)
                return Results.BadRequest();

            var produtoDB = await db.Produtos.FindAsync(id);

            if (produtoDB is null) return Results.NotFound();

            produtoDB.Nome = produto.Nome;
            produtoDB.Descricao = produto.Descricao;

            await db.SaveChangesAsync();

            return Results.Ok(produtoDB);

        }).RequireAuthorization();

        app.MapDelete("/produtos/{id:int}", async (int id, AppDbContext db) =>
        {
            var produto = await db.Produtos.FindAsync(id);

            if (produto is null)
                return Results.NotFound();

            db.Produtos.Remove(produto);
            await db.SaveChangesAsync();

            return Results.NoContent();
        }).RequireAuthorization();
    }
}
