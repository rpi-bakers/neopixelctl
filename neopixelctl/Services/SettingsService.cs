using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using neopixelctl.Models;

namespace neopixelctl.Services;

public sealed class SettingsService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow,
    };
    private static readonly HashSet<string> SupportedPatterns =
    [
        "gradation",
        "rainbow",
        "random",
        "halloween_jack_o_lantern",
        "halloween_witch_portion",
        "halloween_vampire_night",
        "halloween_ghostly_moon",
        "halloween_black_cat",
    ];

    public SettingsService()
    {
        SettingsPath = FindSettingsPath();
    }

    public string SettingsPath { get; }

    public NeoPixelSettings Load()
    {
        if (!File.Exists(SettingsPath))
            return new NeoPixelSettings();

        var json = File.ReadAllText(SettingsPath);
        var settings = JsonSerializer.Deserialize<NeoPixelSettings>(json, JsonOptions)
            ?? throw new InvalidDataException("settings.json must contain a JSON object.");
        Validate(settings);
        return settings;
    }

    public void Save(NeoPixelSettings settings)
    {
        Validate(settings);
        var json = JsonSerializer.Serialize(settings, JsonOptions) + Environment.NewLine;
        var temporaryPath = SettingsPath + ".tmp";
        Directory.CreateDirectory(Path.GetDirectoryName(SettingsPath)!);
        File.WriteAllText(temporaryPath, json);
        File.Move(temporaryPath, SettingsPath, true);
    }

    public static void Validate(NeoPixelSettings settings)
    {
        ValidateRange(settings.LedNum, 1, 300, "LED_NUM");
        ValidateRange(settings.LedOnNum, 1, 300, "LED_ON_NUM");
        ValidateRange(settings.MaxBrightness, 0, 100, "MAX_BRIGHTNESS");
        ValidateRange(settings.FadeInRate, 0, 100, "FADE_IN_RATE");
        ValidateRange(settings.FadeOutRate, 0, 100, "FADE_OUT_RATE");
        ValidateRange(settings.PatternFreq, 1, 50, "PATTERN_FREQ");
        ValidateRange(settings.OnTime, 0, 10, "ON_TIME");
        ValidateRange(settings.OffTime, 0, 10, "OFF_TIME");
        ValidateRange(settings.CycleSec, 0.01, 10, "CYCLE_SEC");
        ValidateRange(settings.TimeInterval, 0.01, 10, "TIME_INTERVAL");

        if (settings.LedOnNum > settings.LedNum)
            throw new InvalidDataException("LED_ON_NUM cannot exceed LED_NUM.");
        if (settings.StartPosition < 0 || settings.StartPosition >= settings.LedNum)
            throw new InvalidDataException("START_POSITION must be between 0 and LED_NUM - 1.");
        if (!SupportedPatterns.Contains(settings.Pattern))
            throw new InvalidDataException("PATTERN is not a supported pattern.");
    }

    private static void ValidateRange(int value, int minimum, int maximum, string name)
    {
        if (value < minimum || value > maximum)
            throw new InvalidDataException($"{name} must be between {minimum} and {maximum}.");
    }

    private static void ValidateRange(double value, double minimum, double maximum, string name)
    {
        if (!double.IsFinite(value) || value < minimum || value > maximum)
            throw new InvalidDataException($"{name} must be between {minimum} and {maximum}.");
    }

    private static string FindSettingsPath()
    {
        var roots = new[] { Environment.CurrentDirectory, AppContext.BaseDirectory };
        foreach (var root in roots)
        {
            for (var directory = new DirectoryInfo(root); directory is not null; directory = directory.Parent)
            {
                var projectSettings = Path.Combine(directory.FullName, "neopixel-demo", "settings.json");
                if (File.Exists(projectSettings))
                    return projectSettings;

                var solutionSettings = Path.Combine(directory.FullName, "neopixelctl", "neopixel-demo", "settings.json");
                if (File.Exists(solutionSettings))
                    return solutionSettings;
            }
        }

        var packagedSettings = Path.Combine(AppContext.BaseDirectory, "neopixel-demo", "settings.json");
        if (File.Exists(packagedSettings))
            return packagedSettings;

        return packagedSettings;
    }
}
