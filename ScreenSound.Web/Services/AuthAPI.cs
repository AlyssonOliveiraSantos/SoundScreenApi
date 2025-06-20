﻿using ScreenSound.Web.Responses;
using System.Net.Http.Json;

namespace ScreenSound.Web.Services
{
    public class AuthAPI(IHttpClientFactory factory)
    {
        private readonly HttpClient _httpClient = factory.CreateClient("API");

        public async Task<AuthResponse> LoginAsync(string email, 
                                                   string senha)
        {

            var response = await _httpClient.PostAsJsonAsync("auth/login", new 
            { 
                email, 
                password = senha
            });

            if (response.IsSuccessStatusCode)
            {
                return new AuthResponse { Sucesso = true };
            }else
            {
                return new AuthResponse { Sucesso = false, Erros = ["Login ou senha invalidos."] };
            }

        }

        

    }
}
