using consensoria_autos.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace consensoria_autos.Services
{
    public class ApiClient
    {
        private readonly HttpClient _http;
        private readonly IHttpContextAccessor _context;
        private const string BaseUrl = "http://localhost:3000/api/";

        public ApiClient(HttpClient http, IHttpContextAccessor context)
        {
            _http = http;
            _context = context;
        }

        // ======================================
        // 🔐 Agrega token JWT a los encabezados
        // ======================================
        private void AddAuthHeader()
        {
            var token = _context.HttpContext?.Session.GetString("token");

            if (!string.IsNullOrEmpty(token))
            {
                _http.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                _http.DefaultRequestHeaders.Authorization = null;
            }
        }

        // ==============================
        // 🟢 GET
        // ==============================

        public async Task<ApiResponse<T>> GetAsync<T>(string endpoint)
        {
            AddAuthHeader();
            try
            {
                var response = await _http.GetAsync(BaseUrl + endpoint);
                var text = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"❌ GET {endpoint} FAILED ({response.StatusCode})\n{text}");
                    return new ApiResponse<T> { Success = false, StatusCode = (int)response.StatusCode, Message = text };
                }
                Console.WriteLine(response);
                var data = JsonSerializer.Deserialize<T>(text, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                Console.WriteLine(data);
                return new ApiResponse<T> { Success = true, Data = data, StatusCode = (int)response.StatusCode };
            }
            catch (Exception ex)
            {
                return new ApiResponse<T> { Success = false, Message = ex.Message };
            }
        }
        public async Task<T?> GetAsync2<T>(string endpoint)
        {
            AddAuthHeader();

            try
            {
                var response = await _http.GetAsync(BaseUrl + endpoint);
                var text = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"🌐 GET {BaseUrl + endpoint}");
                Console.WriteLine($"📨 Status: {response.StatusCode}");
                Console.WriteLine($"📦 Raw Response:\n{text}\n");

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"❌ Error al obtener datos desde {endpoint}: {response.ReasonPhrase}");
                    return default;
                }

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                // 🔹 Ahora devolvemos directamente la lista u objeto
                var data = JsonSerializer.Deserialize<T>(text, options);

                Console.WriteLine($"✅ Deserialización exitosa: {typeof(T).Name}");
                return data;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"💥 Error en GET {endpoint}: {ex.Message}");
                return default;
            }
        }


        // ==============================
        // 🟡 POST
        // ==============================
        public async Task<ApiResponse<T>> PostAsync<T>(string endpoint, object data)
        {
            AddAuthHeader();
            var options = new JsonSerializerOptions
            {
                IgnoreNullValues = true,
                PropertyNamingPolicy = null,
                WriteIndented = true
            };

            try
            {
                var json = JsonSerializer.Serialize(data, options);
                Console.WriteLine($"\n🔁 POST {BaseUrl + endpoint}");
                Console.WriteLine($"📦 BODY ENVIADO:\n{json}\n");

                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _http.PostAsync(BaseUrl + endpoint, content);
                var text = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"❌ POST FAILED → {response.StatusCode}");
                    Console.WriteLine($"📨 RESPONSE:\n{text}");
                    return new ApiResponse<T> { Success = false, StatusCode = (int)response.StatusCode, Message = text };
                }

                var result = JsonSerializer.Deserialize<T>(text, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return new ApiResponse<T> { Success = true, Data = result, StatusCode = (int)response.StatusCode };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"💥 Error POST {endpoint}: {ex.Message}");
                return new ApiResponse<T> { Success = false, Message = ex.Message };
            }
        }

        // ==============================
        // 🔵 PUT
        // ==============================
        public async Task<ApiResponse<T>> PutAsync<T>(string endpoint, object data)
        {
            AddAuthHeader();
            try
            {
                var options = new JsonSerializerOptions
                {
                    IgnoreNullValues = true,
                    WriteIndented = true
                };

                var json = JsonSerializer.Serialize(data, options);
                Console.WriteLine($"\n🔁 PUT {BaseUrl + endpoint}");
                Console.WriteLine($"📦 BODY ENVIADO:\n{json}\n");

                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _http.PutAsync(BaseUrl + endpoint, content);
                var text = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"❌ PUT FAILED → {response.StatusCode}");
                    Console.WriteLine($"📨 RESPONSE:\n{text}");
                    return new ApiResponse<T> { Success = false, StatusCode = (int)response.StatusCode, Message = text };
                }

                var result = JsonSerializer.Deserialize<T>(text, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return new ApiResponse<T> { Success = true, Data = result, StatusCode = (int)response.StatusCode };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"💥 Error en PUT {endpoint}: {ex.Message}");
                return new ApiResponse<T> { Success = false, Message = ex.Message };
            }
        }

        // ==============================
        // 🔴 DELETE
        // ==============================
        public async Task<ApiResponse<object>> DeleteAsync(string endpoint)
        {
            AddAuthHeader();
            try
            {
                var response = await _http.DeleteAsync(BaseUrl + endpoint);
                var text = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"❌ DELETE FAILED ({response.StatusCode}) → {text}");
                    return new ApiResponse<object> { Success = false, StatusCode = (int)response.StatusCode, Message = text };
                }

                return new ApiResponse<object> { Success = true, StatusCode = (int)response.StatusCode, Message = "Deleted successfully." };
            }
            catch (Exception ex)
            {
                return new ApiResponse<object> { Success = false, Message = ex.Message };
            }
        }
    }
}
