using System.Collections.Generic;
using System.Configuration;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;
using System.Net.Http;
using Newtonsoft.Json;

namespace RealWeather;

internal static class ModInfo
{
    internal const string Guid = "me.pocke.real-weather";
    internal const string Name = "Real Weather";
    internal const string Version = "1.0.0";
}

[BepInPlugin(ModInfo.Guid, ModInfo.Name, ModInfo.Version)]
internal class RealWeather : BaseUnityPlugin
{
    public bool IsSyncNeeded { get; private set; } = true;
    public List<Weather.Forecast> forecasts = new List<Weather.Forecast>();

    static string apiUrlBase = "https://api.open-meteo.com/v1/forecast";

    private void Awake()
    {
        Logger.LogInfo("RealWeather Mod is loaded!");
        StartFetchingWeather();
    }

    private void Update()
    {

        if (!EClass.core.IsGameStarted)
        {
            return;
        }

        // TODO: sync weather from self.forecasts to EClass.world.weather.forecasts
    }

    private async void StartFetchingWeather()
    {
        while (true) {
            try
            {
                // TODO: Get latitude and longitude fro the config file
                string url = $"{apiUrlBase}?latitude=35.6895&longitude=139.6917&current=weather_code";
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        string jsonResponse = await response.Content.ReadAsStringAsync();
                        var weatherData = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiResponse>(jsonResponse);
                        if (weatherData != null && weatherData.Current != null)
                        {
                            int wmoCode = weatherData.Current.WeatherCode;
                            Weather.Condition condition = WeatherCode.GetWeatherConditionFromWMOCode(wmoCode);
                            Logger.LogInfo($"Current weather condition: {condition}");

                            forecasts = new List<Weather.Forecast>();
                            forecasts.Add(new Weather.Forecast
                            {
                                duration = EClass.rnd(24) + 10,
                                condition = condition,
                            });
                        }
                        else
                        {
                            Logger.LogError("Failed to parse weather data:" + jsonResponse);
                        }
                    }
                    else
                    {
                        Logger.LogError($"Failed to fetch weather data: {response.StatusCode}");
                    }
                }
            }
            catch (System.Exception ex)
            {
                Logger.LogError($"Exception while fetching weather data: {ex.Message}");
            }

            await System.Threading.Tasks.Task.Delay(3600 * 1000); // Fetch every hour
        }
    }
}
