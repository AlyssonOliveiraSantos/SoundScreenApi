using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using ScreenSound.API.Requests;
using ScreenSound.API.Responses;
using ScreenSound.Banco;
using ScreenSound.Modelos;

namespace ScreenSound.API.Endpoints
{
    public static class ArtistasExtensions
    {
        public static void AddEndpointsArtistas(this WebApplication app)
        {
            #region Endpoint Artistas

            app.MapGet("/Artistas", ([FromServices] DAL<Artista> dal) =>
            {
                var listaDeArtistas = dal.Listar();
                if (listaDeArtistas == null)
                {
                    return Results.NotFound();
                }
                return Results.Ok(EntityListToResponseList(listaDeArtistas));
            });

            app.MapGet("/Artistas/{nome}", ([FromServices] DAL<Artista> dal, string nome) =>
            {
                var artista = dal.RecuperarPor(a => a.Nome.ToUpper().Equals(nome.ToUpper()));
                if (artista == null)
                {
                    return Results.NotFound();
                }
                return Results.Ok(EntityToResponse(artista));
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

            app.MapPut("/Artistas", ([FromServices] DAL<Artista> dal, [FromBody] ArtistaRequestEdit artista) =>
            {
                var artistaDal = dal.RecuperarPor(dal => dal.Id == artista.Id);

                if (artistaDal == null)
                {
                    return Results.NotFound();
                }
                artistaDal.Nome = artista.nome;
                artistaDal.Bio = artista.bio;

                dal.Atualizar(artistaDal);
                return Results.Ok();
            });
            #endregion
        }
        private static ICollection<ArtistasResponse> EntityListToResponseList(IEnumerable<Artista> listaDeArtistas)
        {
            return listaDeArtistas.Select(a => EntityToResponse(a)).ToList();
        }

        private static ArtistasResponse EntityToResponse(Artista artista)
        {
            var artistaResponse = new ArtistasResponse(artista.Id, artista.Nome, artista.Bio, artista.FotoPerfil);
            return artistaResponse;
        }
    }
}
