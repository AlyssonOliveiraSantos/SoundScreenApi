using ScreenSound.Web.Responses;
using System.Net.Http.Json;

namespace ScreenSound.Web.Services
{
    public class GenerosApi
    {

        private readonly HttpClient _httpclient;

        public GenerosApi(IHttpClientFactory _httpclientFactory)
        {
            _httpclient = _httpclientFactory.CreateClient("API");
        }

        public async Task<IEnumerable<GeneroResponse>?> GetGenerosAsync()
        {
            return await _httpclient.GetFromJsonAsync<IEnumerable<GeneroResponse>>("Generos");
        }

        public async Task<GeneroResponse?> GetGeneroPorNome(string nome)
        {
            return await _httpclient.GetFromJsonAsync<GeneroResponse>($"Generos/{nome}");
        }
    }
}
