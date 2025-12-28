using Microsoft.AspNetCore.Mvc;
using SmartCity.Weather.Core.DTOs;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace SmartCity.Weather.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class WeatherController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public WeatherController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<ActionResult<WeatherDto>> GetWeatherAsync(double latitude = 52.52, double longitude = 13.41)
        {
            var httpClient = _httpClientFactory.CreateClient();

            var url = $"https://api.open-meteo.com/v1/forecast?latitude={latitude}&longitude={longitude}&hourly=temperature_2m&current=temperature_2m,wind_speed_10m";

            try
            {
                var response = await httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, "Failed to retrieve weather data.");
                }

                var json = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var weather = JsonSerializer.Deserialize<WeatherDto>(json, options);

                if (weather == null)
                    return NotFound("Weather data not found.");

                return Ok(weather);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, $"Error retrieving weather data: {ex.Message}");
            }
        }
    }
}