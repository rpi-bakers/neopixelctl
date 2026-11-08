using System.Text.Json.Serialization;

namespace neopixelctl.Models;

public sealed class NeoPixelSettings
{
    [JsonPropertyName("LED_NUM")]
    public int LedNum { get; set; } = 164;

    [JsonPropertyName("LED_ON_NUM")]
    public int LedOnNum { get; set; } = 100;

    [JsonPropertyName("START_POSITION")]
    public int StartPosition { get; set; }

    [JsonPropertyName("MAX_BRIGHTNESS")]
    public int MaxBrightness { get; set; } = 5;

    [JsonPropertyName("ON_TIME")]
    public double OnTime { get; set; } = 1.5;

    [JsonPropertyName("OFF_TIME")]
    public double OffTime { get; set; } = 0.5;

    [JsonPropertyName("FADE_IN_RATE")]
    public int FadeInRate { get; set; } = 80;

    [JsonPropertyName("FADE_OUT_RATE")]
    public int FadeOutRate { get; set; } = 80;

    [JsonPropertyName("CYCLE_SEC")]
    public double CycleSec { get; set; } = 0.5;

    [JsonPropertyName("TIME_INTERVAL")]
    public double TimeInterval { get; set; } = 0.1;

    [JsonPropertyName("PATTERN_FREQ")]
    public int PatternFreq { get; set; } = 10;

    [JsonPropertyName("MOVE_REVERSE")]
    public bool MoveReverse { get; set; }

    [JsonPropertyName("PATTERN")]
    public string Pattern { get; set; } = "gradation";
}
