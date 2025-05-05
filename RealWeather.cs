using System.Collections.Generic;
using System.Configuration;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;

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
    }

    private async void StartFetchingWeather()
    {
        while (true) {
            // TODO: Call API

            await System.Threading.Tasks.Task.Delay(3600 * 1000); // Fetch every hour
        }
    }
}
