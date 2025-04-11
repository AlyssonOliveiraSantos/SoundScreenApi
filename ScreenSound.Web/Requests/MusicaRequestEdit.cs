using System.ComponentModel;

namespace ScreenSound.Web.Requests
{
    public record MusicaRequestEdit(int Id, string nome, int artistaId, int anoLancamento, ICollection<GeneroRequest> generos):MusicaResponse(nome, artistaId, anoLancamento, generos);
}