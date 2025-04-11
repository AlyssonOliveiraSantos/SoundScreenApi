using System.ComponentModel.DataAnnotations;

namespace ScreenSound.Web.Requests
{
    public record MusicaResponse([Required] string nome, [Required] int artistaId, int anoLancamento, ICollection<GeneroRequest> generos=null);
}
