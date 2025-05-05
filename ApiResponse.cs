using System.Collections.Generic;
using Newtonsoft.Json;

namespace RealWeather;
public class ApiResponse
{
    [JsonProperty("hourly")]
    public HourlyWeatherData Hourly { get; set; }

    public class HourlyWeatherData
    {
        [JsonProperty("weather_code")]
        public List<int> WeatherCode { get; set; }
    }
}
