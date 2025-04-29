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


        public async Task<IEnumerable<ArtistaResponse>?> GetArtistasAsync()
        {
            return await
                _httpClient.GetFromJsonAsync<IEnumerable<ArtistaResponse>>("Artistas");
        }

        public async Task AddArtistaAsync(ArtistaRequest artista)
        {
            await _httpClient.PostAsJsonAsync("artistas", artista);
        }

        public async Task EditarArtistaAsync(ArtistaRequestEdit artista)
        {
            await _httpClient.PutAsJsonAsync($"artistas", artista);
        }

        public async Task DeleteArtistaAsync(int id)
        {
            await _httpClient.DeleteAsync($"artistas/{id}");
                
        }

        public async Task UpdateArtistaAsync(ArtistaRequestEdit artista)
        {
            await _httpClient.PutAsJsonAsync($"artistas", artista);
        }

        public async Task<ArtistaResponse?> GetArtistaPorNomeAsync(string nome)
        {
            return await _httpClient.GetFromJsonAsync<ArtistaResponse>($"artistas/{nome}");
        }
    }
}
