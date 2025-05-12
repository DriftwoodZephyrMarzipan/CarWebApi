using CarWebApi.EvDataModels.DTOs;
using CarWebApi.EvDataModels.Models;
using System.Net.Http.Json;

namespace CarWebApiClientLibrary
{
    /// <summary>
    /// This class is a client for the Car Web API. It provides methods to interact with the API endpoints for Makes, Models, Cars, 
    /// CafvTypes, and EvTypes.
    /// 
    /// Though it hides much of the detail, it sets the LastHttpResponseMessage value to the last HttpResponse received. This can be
    /// interrogated for error messages or further processing.
    /// </summary>
    public class CarWebApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseApiUrl;
        public HttpResponseMessage LastHttpResponseMessage { get; set; } = null!;

        public CarWebApiClient(string baseApiUrl)
        {
            _baseApiUrl = baseApiUrl.TrimEnd('/');
            _httpClient = new HttpClient();
        }

        public async Task<IdList> GetAllMakeIdsAsync()
        {
            var response = await _httpClient.GetAsync($"{_baseApiUrl}/Makes");
            LastHttpResponseMessage = response;
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to get all make IDs. Status code: {response.StatusCode}");
            }
            return await response.Content.ReadFromJsonAsync<IdList>();
        }

        public async Task<Make?> GetMakeAsync(int id)
        {
            var response = await _httpClient.GetAsync($"{_baseApiUrl}/Makes/{id}");
            LastHttpResponseMessage = response;
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadFromJsonAsync<Make>();
        }

        public async Task<List<Make>?> GetAllMakesAsync()
        {
            var response = await _httpClient.GetAsync($"{_baseApiUrl}/Makes/all");
            LastHttpResponseMessage = response;
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
            return await response.Content.ReadFromJsonAsync<List<Make>>();
        }

        public async Task<Make?> CreateMakeAsync(Make make)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_baseApiUrl}/Makes",make);
            LastHttpResponseMessage = response;
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
            return await response.Content.ReadFromJsonAsync<Make>();
        }

        public async Task<Make?> UpdateMakeAsync(Make make)
        {
            var response = await _httpClient.PutAsJsonAsync($"{_baseApiUrl}/Makes", make);
            LastHttpResponseMessage = response;
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
            return await response.Content.ReadFromJsonAsync<Make>();
        }

        public async Task<bool> DeleteMakeAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{_baseApiUrl}/Makes/{id}");
            LastHttpResponseMessage = response;
            return response.IsSuccessStatusCode;
        }

        public async Task<IdList> GetAllModelIdsAsync()
        {
            var response = await _httpClient.GetAsync($"{_baseApiUrl}/Models");
            LastHttpResponseMessage = response;
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to get all model IDs. Status code: {response.StatusCode}");
            }
            return await response.Content.ReadFromJsonAsync<IdList>();
        }

        public async Task<List<Model>?> GetAllModelsAsync()
        {
            var response = await _httpClient.GetAsync($"{_baseApiUrl}/Models/all");
            LastHttpResponseMessage = response;
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
            return await response.Content.ReadFromJsonAsync<List<Model>>();
        }
        public async Task<List<Model>?> GetModelsByMakeIdAsync(int make_id)
        {
            var response = await _httpClient.GetAsync($"{_baseApiUrl}/Models/all/{make_id}");
            LastHttpResponseMessage = response;
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
            return await response.Content.ReadFromJsonAsync<List<Model>>();
        }

        public async Task<Model?> GetModelAsync(int id)
        {
            var response = await _httpClient.GetAsync($"{_baseApiUrl}/Models/{id}");
            LastHttpResponseMessage = response;
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
            return await response.Content.ReadFromJsonAsync<Model>();
        }

        public async Task<Model?> CreateModelAsync(Model model)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_baseApiUrl}/Models", model);
            LastHttpResponseMessage = response;
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
            return await response.Content.ReadFromJsonAsync<Model>();
        }

        public async Task<Model?> UpdateModelAsync(Model model)
        {
            var response = await _httpClient.PutAsJsonAsync($"{_baseApiUrl}/Models", model);
            LastHttpResponseMessage = response;
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
            return await response.Content.ReadFromJsonAsync<Model>();
        }

        public async Task<bool> DeleteModelAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{_baseApiUrl}/Models/{id}");
            LastHttpResponseMessage = response;
            return response.IsSuccessStatusCode;
        }

        public async Task<IdList> GetCarsIdsAsync()
        {
            var response = await _httpClient.GetAsync($"{_baseApiUrl}/Cars");
            LastHttpResponseMessage = response;
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to get all car IDs. Status code: {response.StatusCode}");
            }
            return await response.Content.ReadFromJsonAsync<IdList>();
        }

        public async Task<Car?> GetCarAsync(int id)
        {
            var response = await _httpClient.GetAsync($"{_baseApiUrl}/Cars/{id}");
            LastHttpResponseMessage = response;
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
            return await response.Content.ReadFromJsonAsync<Car>();
        }

        public async Task<Car?> CreateCarAsync(Car car)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_baseApiUrl}/Cars", car);
            LastHttpResponseMessage = response;
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
            return await response.Content.ReadFromJsonAsync<Car>();
        }

        public async Task<Car?> UpdateCarAsync(Car car)
        {
            var response = await _httpClient.PutAsJsonAsync($"{_baseApiUrl}/Cars", car);
            LastHttpResponseMessage = response;
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
            return await response.Content.ReadFromJsonAsync<Car>();
        }

        public async Task<bool> DeleteCarAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{_baseApiUrl}/Cars/{id}");
            LastHttpResponseMessage = response;
            return response.IsSuccessStatusCode;
        }

        public async Task<List<EnumIdentifier>> GetCafvTypes()
        {
            var response = await _httpClient.GetAsync($"{_baseApiUrl}/CafvTypes");
            LastHttpResponseMessage = response;
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to get all CafvTypes. Status code: {response.StatusCode}");
            }
            return await response.Content.ReadFromJsonAsync<List<EnumIdentifier>>();
        }

        public async Task<EnumIdentifier> GetCafv(int id)
        {
            var response = await _httpClient.GetAsync($"{_baseApiUrl}/CafvTypes/{id}");
            LastHttpResponseMessage = response;
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to get CafvType. Status code: {response.StatusCode}");
            }
            return await response.Content.ReadFromJsonAsync<EnumIdentifier>();
        }

        public async Task<List<EnumIdentifier>> GetEvTypes()
        {
            var response = await _httpClient.GetAsync($"{_baseApiUrl}/EvTypes");
            LastHttpResponseMessage = response;
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to get all EvTypes. Status code: {response.StatusCode}");
            }
            return await response.Content.ReadFromJsonAsync<List<EnumIdentifier>>();
        }

        public async Task<EnumIdentifier> GetEvType(int id)
        {
            var response = await _httpClient.GetAsync($"{_baseApiUrl}/EvTypes/{id}");
            LastHttpResponseMessage = response;
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to get EvType. Status code: {response.StatusCode}");
            }
            return await response.Content.ReadFromJsonAsync<EnumIdentifier>();
        }
    }
}
