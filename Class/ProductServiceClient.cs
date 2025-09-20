using ServiceDesk.Dto;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace ServiceDesk.Class
{
    public class ProductServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        public ProductServiceClient()
        {
            _httpClient= new HttpClient();
            //Read the API URL from configuration file
            _baseUrl=ConfigurationManager.AppSettings["ProductServiceUrl"] ?? "https://inventory166.az/api";
            _httpClient.BaseAddress = new Uri(_baseUrl);
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
        public async Task<ProductDto> GetProductByInventoryCodeAsync(string inventoryCode,string authToken = null)
        {
            try
            {
                // Add authorization header if token is provided
                if (!string.IsNullOrEmpty(authToken))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = 
                            new AuthenticationHeaderValue("Bearer", authToken);
                }

                // Call the ProductService API endpoint
                var response = await _httpClient.GetAsync($"/products/search/inventory-code/{inventoryCode}");

                if(response.IsSuccessStatusCode)
                {
                    var json=await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    return JsonSerializer.Deserialize<ProductDto>(json, options);
                }

                return null;
            }
            catch (Exception)
            {
                await Logger.Log("System", $"Error calling ProductService API: {ex.Message}");
                return null;
            }
        }

        public async Task<List<DepartmentDto>> GetDepartmentsAsync(string authToken = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(authToken))
                {
                    _httpClient.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", authToken);
                }

                var response = await _httpClient.GetAsync("/departments");

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