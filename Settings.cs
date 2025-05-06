using System;
using System.Collections.Generic;
using BepInEx.Configuration;
using UnityEngine;

namespace RealWeather;

public static class Settings
{

    public static ConfigEntry<double> latitude;
    public static ConfigEntry<double> longitude;

    public static double Latitude
    {
        get => latitude.Value;
        set => latitude.Value = value;
    }

    public static double Longitude
    {
        get => longitude.Value;
        set => longitude.Value = value;
    }
}
