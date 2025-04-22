using ScreenSound.Web.Requests;
using System.Net.Http.Json;

namespace ScreenSound.Web.Services
{
    public class MusicaApi
    {
        private readonly HttpClient _httpclient;

        public MusicaApi(IHttpClientFactory _httpclientFactory)
        {
            _httpclient = _httpclientFactory.CreateClient("API");
        }

        public async Task<IEnumerable<MusicaResponse>?> GetMusicasAsync()
        {
            return await _httpclient.GetFromJsonAsync<IEnumerable<MusicaResponse>>("Musicas");
        }
    }
}
