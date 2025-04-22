using ScreenSound.Web.Requests;
using ScreenSound.Web.Responses;
using System.Net.Http.Json;

namespace ScreenSound.Web.Services
{
    public class ArtistasApi
    {
        private readonly HttpClient _httpClient;

        public ArtistasApi(IHttpClientFactory httpClientFactory)
        {
             _httpClient = httpClientFactory.CreateClient("API");
        }


        public async Task<IEnumerable<ArtistasResponse>?> GetArtistasAsync()
        {
            return await
                _httpClient.GetFromJsonAsync<IEnumerable<ArtistasResponse>>("Artistas");
        }

        public async Task AddArtistaAsync(ArtistaRequest artista)
        {
            await _httpClient.PostAsJsonAsync("artistas", artista);
        }
    }
}
