using ScreenSound.API.Requests;

namespace ScreenSound.API.Responses
{
    public record MusicasResponse(int Id, string Nome, int? ArtistaId,string NomeArtista, int? AnoLancamento);
}
