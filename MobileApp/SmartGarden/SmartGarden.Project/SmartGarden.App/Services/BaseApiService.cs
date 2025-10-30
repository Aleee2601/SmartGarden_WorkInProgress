using SmartGarden.App.Constants;
using SmartGarden.App.Helpers;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace SmartGarden.App.Services
{
    public class BaseApiService
    {
        protected readonly HttpClient _httpClient;

        public BaseApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(ApiConstants.BaseUrl);
        }

        protected async Task<HttpClient> GetAuthenticatedClientAsync()
        {
            var token = await SecureStorageHelper.GetTokenAsync();
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }
            return _httpClient;
        }

        protected async Task<T?> GetAsync<T>(string endpoint)
        {
            try
            {
                var client = await GetAuthenticatedClientAsync();
                var response = await client.GetAsync(endpoint);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<T>();
                }

                return default;
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine($"GET Error: {ex.Message}");
                return default;
            }
        }

        protected async Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest data)
        {
            try
            {
                var client = await GetAuthenticatedClientAsync();
                var response = await client.PostAsJsonAsync(endpoint, data);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<TResponse>();
                }

                return default;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"POST Error: {ex.Message}");
                return default;
            }
        }

        protected async Task<bool> PostAsync<TRequest>(string endpoint, TRequest data)
        {
            try
            {
                var client = await GetAuthenticatedClientAsync();
                var response = await client.PostAsJsonAsync(endpoint, data);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"POST Error: {ex.Message}");
                return false;
            }
        }

        protected async Task<TResponse?> PutAsync<TRequest, TResponse>(string endpoint, TRequest data)
        {
            try
            {
                var client = await GetAuthenticatedClientAsync();
                var response = await client.PutAsJsonAsync(endpoint, data);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<TResponse>();
                }

                return default;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"PUT Error: {ex.Message}");
                return default;
            }
        }

        protected async Task<bool> PutAsync(string endpoint)
        {
            try
            {
                var client = await GetAuthenticatedClientAsync();
                var response = await client.PutAsync(endpoint, null);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"PUT Error: {ex.Message}");
                return false;
            }
        }

        protected async Task<bool> DeleteAsync(string endpoint)
        {
            try
            {
                var client = await GetAuthenticatedClientAsync();
                var response = await client.DeleteAsync(endpoint);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DELETE Error: {ex.Message}");
                return false;
            }
        }
    }
}
