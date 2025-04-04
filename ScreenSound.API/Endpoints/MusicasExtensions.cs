using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using ScreenSound.API.Requests;
using ScreenSound.API.Responses;
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
                var listaDeMusicas = dal.Listar();
                if (listaDeMusicas == null)
                {
                    return Results.NotFound();
                }
                return Results.Ok(EntityListToResponseList(listaDeMusicas));
            });

            app.MapGet("/Musicas/{nome}", ([FromServices] DAL<Musica> dal, string nome) =>
            {
                var musica = dal.ListarPor(m => m.Nome == nome);
                if (musica == null)
                {
                    return Results.NotFound();
                }
                return Results.Ok(EntityListToResponseList(musica));
            });

            app.MapPost("/Musicas", ([FromServices] DAL<Musica> dal, [FromServices] DAL<Genero> dalGenero, [FromBody] MusicaRequest musicaRequest) =>
            {
                if (musicaRequest != null)
                {
                    Musica musica = new Musica(musicaRequest.nome)
                    {
                        AnoLancamento = musicaRequest.anoLancamento,
                        ArtistaId = musicaRequest.artistaId,
                        Generos = musicaRequest is not null ? GeneroRequestConverter(musicaRequest.generos, dalGenero) : new List<Genero>()
                    };

                    dal.Adicionar(musica);
                    return Results.Created($"/Musicas/{musica.Id}", musica);
                }
                return Results.BadRequest();

            });

            app.MapPut("/Musicas", ([FromServices] DAL<Musica> dal, [FromBody] MusicaRequestEdit musica) =>
            {
                var musicaDal = dal.RecuperarPor(m => m.Id == musica.Id);
                if (musicaDal == null)
                {
                    return Results.NotFound();
                }
                musicaDal.Nome = musica.nome;
                musicaDal.AnoLancamento = musica.anoLancamento;
                dal.Atualizar(musicaDal);
                return Results.NoContent();
            });
        }



        private static List<Genero> GeneroRequestConverter(ICollection<GeneroRequest> generos,
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

        private static MusicasResponse EntityToResponse(Musica musica)
        {
           var MusicaResponse = new MusicasResponse(musica.Id, musica.Nome, musica.ArtistaId, musica.Artista.Nome, musica.AnoLancamento);
            return MusicaResponse;
        }

        private static ICollection<MusicasResponse> EntityListToResponseList(IEnumerable<Musica> listaMusicas)
        {
            return listaMusicas.Select(a => EntityToResponse(a)).ToList();
        }
    }
}
