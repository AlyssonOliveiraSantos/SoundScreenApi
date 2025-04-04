using ScreenSound.Shared.Models.Modelos;
using System.ComponentModel;

namespace ScreenSound.API.Requests
{
    public record MusicaRequestEdit(int Id, string Nome, int ArtistaId, int AnoLancamento, ICollection<GeneroRequest> Generos):MusicaRequest(Nome, ArtistaId, AnoLancamento, Generos);
}