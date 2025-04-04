using ScreenSound.Shared.Models.Modelos;
using System.ComponentModel;

namespace ScreenSound.API.Requests
{
    public record MusicaRequestEdit(int Id, string nome, int artistaId, int anoLancamento, ICollection<GeneroRequest> generos):MusicaRequest(nome, artistaId, anoLancamento, generos);
}