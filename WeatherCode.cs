namespace RealWeather;

public static class WeatherCode
{
    // WMO codes for weather conditions
    // https://open-meteo.com/en/docs?timezone=Asia%2FTokyo&current=weather_code#data_sources
    //
    // 0	Clear sky
    // 1, 2, 3	Mainly clear, partly cloudy, and overcast
    // 45, 48	Fog and depositing rime fog
    // 51, 53, 55	Drizzle: Light, moderate, and dense intensity
    // 56, 57	Freezing Drizzle: Light and dense intensity
    // 61, 63, 65	Rain: Slight, moderate and heavy intensity
    // 66, 67	Freezing Rain: Light and heavy intensity
    // 71, 73, 75	Snow fall: Slight, moderate, and heavy intensity
    // 77	Snow grains
    // 80, 81, 82	Rain showers: Slight, moderate, and violent
    // 85, 86	Snow showers slight and heavy
    // 95 *	Thunderstorm: Slight or moderate
    // 96, 99 *	Thunderstorm with slight and heavy hail
    public static Weather.Condition GetWeatherConditionFromWMOCode(int wmoCode)
    {
        return wmoCode switch
        {
            0 => Weather.Condition.Fine,
            1 => Weather.Condition.Fine,
            2 or 3 => Weather.Condition.Cloudy,
            45 or 48 => Weather.Condition.Cloudy, // Fog treated as cloudy
            51 or 53 or 55 => Weather.Condition.Rain,
            56 or 57 => Weather.Condition.Rain, // Freezing drizzle treated as rain
            61 or 63 => Weather.Condition.Rain,
            65 => Weather.Condition.RainHeavy,
            66 or 67 => Weather.Condition.RainHeavy, // Freezing rain treated as heavy rain
            71 or 73 => Weather.Condition.Snow,
            75 => Weather.Condition.SnowHeavy,
            77 => Weather.Condition.Snow, // Snow grains treated as light snow
            80 or 81 => Weather.Condition.Rain,
            82 => Weather.Condition.RainHeavy,
            85 => Weather.Condition.Snow,
            86 => Weather.Condition.SnowHeavy,
            95 => Weather.Condition.RainHeavy, // Thunderstorm treated as heavy rain
            96 or 99 => Weather.Condition.RainHeavy, // Thunderstorm with hail treated as heavy rain
            _ => Weather.Condition.Fine, // Default case for unknown codes
        };
    }
}
