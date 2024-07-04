using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WeatherForecastModels.Models;

public interface IWeatherService
{
    Task<WeatherForecast> GetWeatherForecastAsync(string city);
}

public class WeatherService : IWeatherService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public WeatherService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<WeatherForecast> GetWeatherForecastAsync(string city)
    {
        var keyApiWeather = _configuration["key:keyApiWeather"];
        HttpResponseMessage response = await _httpClient.GetAsync($"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={keyApiWeather}&units=metric");

        if (response == null)
        {
            throw new Exception("Failed to get data from the API.");
        }

        var weather = await response.Content.ReadAsStringAsync();

        dynamic weatherData = Newtonsoft.Json.JsonConvert.DeserializeObject(weather);

        var weatherForecast = new WeatherForecast
        {
            City = city,
            TemperatureC = weatherData["main"]["temp"].ToObject<double>(),
            Date = DateTimeOffset.FromUnixTimeSeconds((long)weatherData["dt"]).DateTime,
            Description = weatherData["weather"][0]["description"]
        };
        return weatherForecast;
    }
}