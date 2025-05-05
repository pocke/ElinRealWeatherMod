using System.Collections.Generic;
using System.Configuration;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;
using System.Net.Http;
using Newtonsoft.Json;
using System.Linq;
using System;

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

        if (IsSyncNeeded)
        {
            EClass.world.weather.forecasts = forecasts;
            forecasts = new List<Weather.Forecast>();
            Logger.LogInfo($"Weather data synced: {forecasts.Count} entries.");
            IsSyncNeeded = false;
        }
    }

    private async void StartFetchingWeather()
    {
        while (true) {
            if (IsSyncNeeded)
            {
                await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(1));
                continue;
            }

            try
            {
                // TODO: Get latitude and longitude from the config file
                string url = $"{apiUrlBase}?latitude=35.6895&longitude=139.6917&hourly=weather_code";
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        string jsonResponse = await response.Content.ReadAsStringAsync();
                        var weatherData = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiResponse>(jsonResponse);
                        if (weatherData != null && weatherData.Hourly != null && weatherData.Hourly.WeatherCode != null && weatherData.Hourly.WeatherCode.Count() > 0)
                        {
                            List<int> wmoCodes = weatherData.Hourly.WeatherCode;
                            forecasts = new List<Weather.Forecast>();

                            Weather.Condition? currentCondition = null;
                            int currentDuration = 0;

                            foreach (int wmoCode in wmoCodes)
                            {
                                Weather.Condition condition = WeatherCode.GetWeatherConditionFromWMOCode(wmoCode);

                                if (currentCondition == condition)
                                {
                                    currentDuration++;
                                }
                                else
                                {
                                    if (currentCondition != null)
                                    {
                                        forecasts.Add(new Weather.Forecast
                                        {
                                            duration = currentDuration,
                                            condition = currentCondition.Value,
                                        });
                                    }

                                    currentCondition = condition;
                                    currentDuration = 1;
                                }
                            }

                            // Add the last forecast
                            if (currentCondition != null)
                            {
                                forecasts.Add(new Weather.Forecast
                                {
                                    duration = currentDuration,
                                    condition = currentCondition.Value,
                                });
                            }
                            IsSyncNeeded = true;

                            Logger.LogInfo($"Updated forecasts: {forecasts.Count} entries.");
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

            await System.Threading.Tasks.Task.Delay(TimeSpan.FromHours(1));
        }
    }
}
