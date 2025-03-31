using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using ScreenSound.API.Requests;
using ScreenSound.Banco;
using ScreenSound.Modelos;
using ScreenSound.Shared.Models.Modelos;
using System.Linq;

namespace ScreenSound.API.Endpoints
{
    public static class MusicasExtensions
    {
        public static void AddEndpointsMusicas(this WebApplication app)
        {

            app.MapGet("/Musicas", ([FromServices] DAL<Musica> dal) =>
            {
                return Results.Ok(dal.Listar());
            });

            app.MapGet("/Musicas/{nome}", ([FromServices] DAL<Musica> dal, string nome) =>
            {
                var musica = dal.ListarPor(m => m.Nome == nome);
                if (musica == null)
                {
                    return Results.NotFound();
                }
                return Results.Ok(musica);
            });

            app.MapPost("/Musicas", ([FromServices] DAL<Musica> dal, [FromServices] DAL<Genero> dalGenero, [FromBody] MusicaRequest musicaRequest,) =>
            {
                if (musicaRequest != null)
                {
                    Musica musica = new Musica(musicaRequest.nome)
                    {
                        AnoLancamento = musicaRequest.anoLancamento,
                        ArtistaId = musicaRequest.ArtistaId,
                        Generos = musicaRequest is not null ? GeneroRequestConverter(musicaRequest.generos) : new List<Genero>()
                    };

                    dal.Adicionar(musica);
                    return Results.Created($"/Musicas/{musica.Id}", musica);
                }
                return Results.BadRequest();

            });

            app.MapPut("/Musicas/{id}", ([FromServices] DAL<Musica> dal, [FromBody] Musica musica) =>
            {
                var musicaDal = dal.RecuperarPor(m => m.Id == musica.Id);
                if (musicaDal == null)
                {
                    return Results.NotFound();
                }
                musicaDal.Nome = musica.Nome;
                musicaDal.AnoLancamento = musica.AnoLancamento;
                dal.Atualizar(musicaDal);
                return Results.NoContent();
            });
        }

        private static ICollection<Genero> GeneroRequestConverter(ICollection<GeneroRequest> generos,
                                                                     DAL<Genero> dalGenero)
        {
            var listaDeGeneros = new List<Genero>();
            foreach (var genero in generos)
            {
                var generoDal = dalGenero.RecuperarPor(g => g.Nome.ToUpper().Equals(genero.Nome.ToUpper()));
                if (generoDal == null)
                {
                    generoDal = RequestToEntity(genero);
                    dalGenero.Adicionar(generoDal);
                }
                listaDeGeneros.Add(generoDal);
            }
            return listaDeGeneros;
        }

        private static Genero RequestToEntity(GeneroRequest generoRequest)
        {
            return new Genero()
            {
                Nome = generoRequest.Nome,
                Descricao = generoRequest.Descricao,

            };
        }
    }
}
