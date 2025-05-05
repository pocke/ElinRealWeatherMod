namespace RealWeather;
public class ApiResponse
{
    public CurrentWeatherData Current { get; set; }

    public class CurrentWeatherData
    {
        public int WeatherCode { get; set; }
    }
}
