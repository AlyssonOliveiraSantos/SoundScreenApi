using Microsoft.AspNetCore.Mvc;
using ScreenSound.API.Requests;
using ScreenSound.Banco;
using ScreenSound.Modelos;

namespace ScreenSound.API.Endpoints
{
    public static class ArtistasExtensions
    {
        public static void AddEndpointsArtistas(this WebApplication app) 
        {

            app.MapGet("/Artistas", ([FromServices] DAL<Artista> dal) =>
            {
                return Results.Ok(dal.Listar());
            });

            app.MapGet("/Artistas/{nome}", ([FromServices] DAL<Artista> dal, string nome) =>
            {
                var artista = dal.RecuperarPor(a => a.Nome.ToUpper().Equals(nome.ToUpper()));
                if (artista == null)
                {
                    return Results.NotFound();
                }
                return Results.Ok(artista);
            });

            app.MapPost("/Artistas", ([FromServices] DAL<Artista> dal, [FromBody] ArtistaRequest artistaRequest) =>
            {
                Artista artista = new Artista(artistaRequest.nome, artistaRequest.bio);
                if (artista != null)
                {
                    dal.Adicionar(artista);
                    return Results.Created($"/Artistas/{artista.Id}", artista);
                }
                return Results.BadRequest();
            });

            app.MapDelete("/Artistas/{id}", ([FromServices] DAL<Artista> dal, int id) =>
            {
                var artista = dal.RecuperarPor(a => a.Id == id);
                if (artista == null)
                {
                    return Results.NotFound();
                }
                dal.Deletar(artista);
                return Results.NoContent();
            });

            app.MapPut("/Artistas/{id}", ([FromServices] DAL<Artista> dal, [FromBody] Artista artista) =>
            {
                var artistaDal = dal.RecuperarPor(dal => dal.Id == artista.Id);

                if (artistaDal == null)
                {
                    return Results.NotFound();
                }
                artistaDal.Nome = artista.Nome;
                artistaDal.FotoPerfil = artista.FotoPerfil;
                artistaDal.Bio = artista.Bio;

                dal.Atualizar(artistaDal);
                return Results.NoContent();
            });
        }
    }
}
