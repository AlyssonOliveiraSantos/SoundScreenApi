namespace ScreenSound.Web.Responses
{
    public record MusicasResponse(int Id, string Nome, int? ArtistaId,string NomeArtista, int? AnoLancamento);
}
