using Microsoft.AspNetCore.Mvc;
using ScreenSound.API.Requests;
using ScreenSound.API.Responses;
using ScreenSound.Banco;
using ScreenSound.Shared.Models.Modelos;
using System.Runtime.CompilerServices;

namespace ScreenSound.API.Endpoints
{
    public static class GenerosExtensions
    {


        public static void AddEndpointsGeneros(this WebApplication app)
        {


            var groupBuilder = app.MapGroup("generos").
                RequireAuthorization().
                WithTags("Gêneros");

            groupBuilder.MapGet("", ([FromServices] DAL<Genero> dal) =>
            {
                var generos = dal.Listar();
                return Results.Ok(EntityListToResponseList(generos));
            });

            groupBuilder.MapGet("{nome}", ([FromServices] DAL<Genero> dal, string nome) =>
            {
                var generos = dal.ListarPor(g => g.Nome.ToUpper().Equals(nome.ToUpper()));

                if (generos == null)
                {
                    return Results.NotFound();
                }
                return Results.Ok(EntityListToResponseList(generos));
            });

            groupBuilder.MapPost("", ([FromServices] DAL<Genero> dal, GeneroRequest generoRequest) =>
            {
                if (generoRequest != null)
                {
                    Genero genero = RequestToEntity(generoRequest);
                    dal.Adicionar(genero);
                    return Results.Created($"{genero.Id}", genero);
                }
                return Results.BadRequest();

            });

            groupBuilder.MapPut("", ([FromServices] DAL<Genero> dal, GeneroRequestEdit generoRequest) => {

                var generoDal = dal.RecuperarPor(g => g.Id == generoRequest.id);
                if (generoDal == null)
                {
                    return Results.NotFound();
                }
                generoDal.Nome = generoRequest.Nome;
                generoDal.Descricao = generoRequest.Descricao;
                dal.Atualizar(generoDal);
                return Results.NoContent();
            });

            groupBuilder.MapDelete("{id}" , ([FromServices] DAL<Genero> dal, int id ) =>
            {
                var genero = dal.RecuperarPor(g => g.Id == id);
                if (genero == null)
                {
                    return Results.NotFound();
                }
                dal.Deletar(genero);
                return Results.NoContent();
            });

        }

        private static Genero RequestToEntity(GeneroRequest generoRequest)
        {
            var genero = new Genero
            {
                Nome = generoRequest.Nome,
                Descricao = generoRequest.Descricao
            };

            return genero;
        }
        private static GeneroResponse EntityToResponse(Genero genero)
        {
            var generoResponse = new GeneroResponse(genero.Id, genero.Nome, genero.Descricao);
            return generoResponse;
        }

        private static ICollection<GeneroResponse> EntityListToResponseList(IEnumerable<Genero> generoList)
        {
            return generoList.Select(a => EntityToResponse(a)).ToList();
        }
    }
}
