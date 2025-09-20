using ServiceDesk.Dto;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Text.Json;

namespace ServiceDesk.Class
{
    public class ProductServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly string _apiKey;
        public ProductServiceClient()
        {
            _httpClient= new HttpClient();
            //Read the API URL from configuration file
            _baseUrl=ConfigurationManager.AppSettings["ProductServiceUrl"] ?? "http://inventory166.az:5001/api";
            _apiKey = ConfigurationManager.AppSettings["ProductServiceApiKey"] ?? "";
            _httpClient.BaseAddress = new Uri(_baseUrl);
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            // Add the API key to all requests
            if (!string.IsNullOrEmpty(_apiKey))
            {
                _httpClient.DefaultRequestHeaders.Add("X-Api-Key", _apiKey);
            }
        }
        public async Task<ProductDto> GetProductByInventoryCodeAsync(string inventoryCode)
        {
            try
            {
                // Call the ProductService API endpoint
                var response = await _httpClient.GetAsync($"api/products/search/inventory-code/{inventoryCode}");

                if(response.IsSuccessStatusCode)
                {
                    var json=await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    return JsonSerializer.Deserialize<ProductDto>(json, options);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    // Log the authentication failure
                    await Logger.Log("System", "Failed to authenticate with ProductService API. Please check API key configuration.");
                    return null;
                }

                return null;
            }
            catch (Exception ex)
            {
                await Logger.Log("System", $"Error calling ProductService API: {ex.Message}");
                return null;
            }
        }

        public async Task<List<DepartmentDto>> GetDepartmentsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/departments");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    return JsonSerializer.Deserialize<List<DepartmentDto>>(json, options);
                }

                return new List<DepartmentDto>();
            }
            catch (Exception ex)
            {
                await Logger.Log("System", $"Error getting departments from API: {ex.Message}");
                return new List<DepartmentDto>();
            }
        }
    }
}