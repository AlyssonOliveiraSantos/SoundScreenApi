using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Client;
using ScreenSound.API.Requests;
using ScreenSound.API.Responses;
using ScreenSound.Banco;
using ScreenSound.Modelos;
using ScreenSound.Shared.Data.Modelos;
using ScreenSound.Shared.Models.Modelos;
using System.Security.Claims;

namespace ScreenSound.API.Endpoints
{
    public static class ArtistasExtensions
    {
        public static void AddEndpointsArtistas(this WebApplication app)
        {
            var groupBuilder = app.MapGroup("artistas").
                RequireAuthorization().
                WithTags("Artistas");

            #region Endpoint Artistas

            groupBuilder.MapGet("", ([FromServices] DAL<Artista> dal) =>
            {
                var listaDeArtistas = dal.ListarComInclude(a => a.Avaliacoes);

                if (listaDeArtistas == null)
                {
                    return Results.NotFound();
                }
                return Results.Ok(EntityListToResponseList(listaDeArtistas));
            });

            groupBuilder.MapGet("{nome}", ([FromServices] DAL<Artista> dal, string nome) =>
            {
                var artista = dal.RecuperarPor(a => a.Nome.ToUpper().Equals(nome.ToUpper()));
                if (artista == null)
                {
                    return Results.NotFound();
                }
                return Results.Ok(EntityToResponse(artista));
            });

            groupBuilder.MapPost("", async ([FromServices]IHostEnvironment env,  [FromServices] DAL<Artista> dal, [FromBody] ArtistaRequest artistaRequest) =>
            {

                if(artistaRequest.fotoPerfil != null)
                {
                    var nome = artistaRequest.nome.Trim();
                    var imagemArtista = DateTime.Now.ToString("ddMMyyyyhhss") + "." + nome + ".jpeg";

                    var path = Path.Combine(env.ContentRootPath,
            "wwwroot", "FotosPerfil", imagemArtista);

                    using MemoryStream ms = new MemoryStream(Convert.FromBase64String(artistaRequest.fotoPerfil!));
                    using FileStream fs = new(path, FileMode.Create);
                    await ms.CopyToAsync(fs);

                    var artistaFoto = new Artista(artistaRequest.nome, artistaRequest.bio)
                    {
                        FotoPerfil = $"/FotosPerfil/{imagemArtista}"
                    };

                    dal.Adicionar(artistaFoto);
                    return Results.Ok();

                }

                var artista = new Artista(artistaRequest.nome, artistaRequest.bio);
                dal.Adicionar(artista);
                return Results.Ok();


            });

            groupBuilder.MapDelete("{id}", ([FromServices] DAL<Artista> dal, int id) =>
            {
                var artista = dal.RecuperarPor(a => a.Id == id);
                if (artista == null)
                {
                    return Results.NotFound();
                }
                dal.Deletar(artista);
                return Results.NoContent();
            });

            groupBuilder.MapPut("", ([FromServices] DAL<Artista> dal, [FromBody] ArtistaRequestEdit artista) =>
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

            groupBuilder.MapPost("avaliacao", (
                HttpContext context,
                [FromBody] AvaliacaoArtistaRequest request,
                [FromServices] DAL <PessoaComAcesso> dalPessoa,
                [FromServices] DAL<AvaliacaoArtista> dalAvaliacao,
                [FromServices] DAL < Artista > dalArtista) =>
            {
                var artista = dalArtista.RecuperarPor(a => a.Id == request.artistaId);
                if (artista == null)
                {
                    return Results.NotFound();
                }

               var email =  context.User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value?? throw new InvalidOperationException("Usuário não autenticado");

                var pessoa = dalPessoa.RecuperarPor(c => c.Email.Equals(email)) ?? throw new InvalidOperationException("Usuário não autenticado");
                
                var avaliacao = dalAvaliacao.RecuperarPor(a => a.ArtistaId == artista.Id && a.PessoaId == pessoa.Id);

                if (avaliacao is null )
                {
                    AvaliacaoArtista avaliacaoArt = new AvaliacaoArtista
                    {
                        ArtistaId = artista.Id,
                        PessoaId = pessoa.Id,
                        Nota = request.nota
                    };

                    dalAvaliacao.Adicionar(avaliacaoArt);
                }
                else
                {
                    avaliacao.Nota = request.nota;
                    dalAvaliacao.Atualizar(avaliacao);
                }
                return Results.Created();


            });

            groupBuilder.MapGet("/{id}/avaliacao", (
                [FromServices] DAL<Artista> dalArtista,
                [FromServices] DAL<AvaliacaoArtista> dalAvaliacao,
                [FromServices] DAL<PessoaComAcesso> dalPessoa,
                HttpContext context,
                int id
                ) =>
            {
               var artista =  dalArtista.RecuperarPor(a => a.Id == id);

               var email = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email) ?? throw new InvalidOperationException("Não foi encontrado o email da pessoa logada.");

                
               var pessoa = dalPessoa.RecuperarPor(p => p.Email.Equals(email.Value)) ?? throw new InvalidOperationException("Não foi encontrado o email da pessoa logada.");

               var avaliação = dalAvaliacao.RecuperarPor(a => a.ArtistaId == artista.Id && a.PessoaId == pessoa.Id);
               if (avaliação == null)
                {
                    return Results.Ok(new AvaliacaoArtistaResponse(id, 0));
                }
                return Results.Ok(new AvaliacaoArtistaResponse(id, avaliação.Nota));

            });
            #endregion
        }
        private static ICollection<ArtistaResponse> EntityListToResponseList(IEnumerable<Artista> listaDeArtistas)
        {
            return listaDeArtistas.Select(a => EntityToResponse(a)).ToList();
        }

        private static ArtistaResponse EntityToResponse(Artista artista)
        {
            var artistaResponse = new ArtistaResponse(artista.Id, artista.Nome, artista.Bio, artista.FotoPerfil)
            {
                classificacao = artista.Avaliacoes.Any() ? artista.Avaliacoes.Average(a => a.Nota) : null
            };
            return artistaResponse;
        }
    }
}
