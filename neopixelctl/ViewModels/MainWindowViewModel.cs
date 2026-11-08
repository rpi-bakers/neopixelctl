using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using neopixelctl.Models;
using neopixelctl.Services;

namespace neopixelctl.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly SettingsService _settingsService;
    private bool _isLoading;
    private double _paletteViewportHeight = 120;

    public MainWindowViewModel()
    {
        _settingsService = new SettingsService();
        PatternPalettes =
        [
            new("gradation", "Gradation", "#FF0000", "#0000FF"),
            new("rainbow", "Rainbow", "#FF0000", "#800080"),
            new("random", "Random", "#FF0000", "#FF8000", "#FFFF00", "#00FF00", "#00FFFF", "#0000FF", "#8000FF", "#FF00FF"),
            new("halloween_jack_o_lantern", "Jack-o'-lantern", "#FF4500", "#FFD700", "#FF4500", "#8B008B"),
            new("halloween_witch_portion", "Witch potion", "#ADFF2F", "#00C864", "#00FFFF", "#800080"),
            new("halloween_vampire_night", "Vampire night", "#FF0000", "#FF00FF", "#800080", "#191970"),
            new("halloween_ghostly_moon", "Ghostly moon", "#FFFFFF", "#00FFFF", "#0000FF", "#800080"),
            new("halloween_black_cat", "Black cat", "#000000", "#300050", "#FF6600", "#FFFF00"),
        ];

        try
        {
            LoadSettings();
        }
        catch (Exception error)
        {
            ApplySettings(new NeoPixelSettings());
            StatusMessage = $"Could not load settings: {error.Message}";
        }
    }

    public ObservableCollection<PatternPalette> PatternPalettes { get; }
    public string SettingsFilePath => _settingsService.SettingsPath;
    public int LedOnNumMaximum => LedNum;
    public int StartPositionMaximum => Math.Max(0, LedNum - 1);
    public double PaletteViewportHeight
    {
        get => _paletteViewportHeight;
        private set => SetProperty(ref _paletteViewportHeight, value);
    }

    public void UpdatePaletteViewportHeight(double windowHeight)
    {
        PaletteViewportHeight = Math.Max(120, windowHeight - 260);
    }

    [ObservableProperty]
    private int _ledNum;

    [ObservableProperty]
    private int _ledOnNum;

    [ObservableProperty]
    private int _startPosition;

    [ObservableProperty]
    private int _maxBrightness;

    [ObservableProperty]
    private decimal _onTime;

    [ObservableProperty]
    private decimal _offTime;

    [ObservableProperty]
    private int _fadeInRate;

    [ObservableProperty]
    private int _fadeOutRate;

    [ObservableProperty]
    private decimal _cycleSec;

    [ObservableProperty]
    private decimal _timeInterval;

    [ObservableProperty]
    private int _patternFreq;

    [ObservableProperty]
    private bool _moveReverse;

    [ObservableProperty]
    private PatternPalette? _selectedPalette;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    partial void OnLedNumChanged(int value)
    {
        if (_isLoading)
            return;

        if (LedOnNum > value)
            LedOnNum = value;
        if (StartPosition >= value)
            StartPosition = value - 1;

        OnPropertyChanged(nameof(LedOnNumMaximum));
        OnPropertyChanged(nameof(StartPositionMaximum));
    }

    partial void OnSelectedPaletteChanged(PatternPalette? value)
    {
        if (!_isLoading && value is not null)
            StatusMessage = $"Selected {value.Name} palette.";
    }

    [RelayCommand]
    private void SaveSettings()
    {
        try
        {
            _settingsService.Save(CreateSettings());
            StatusMessage = $"Settings saved to {SettingsFilePath}";
        }
        catch (Exception error)
        {
            StatusMessage = $"Could not save settings: {error.Message}";
        }
    }

    [RelayCommand]
    private void ReloadSettings()
    {
        try
        {
            LoadSettings();
        }
        catch (Exception error)
        {
            StatusMessage = $"Could not reload settings: {error.Message}";
        }
    }

    private void LoadSettings()
    {
        ApplySettings(_settingsService.Load());
        StatusMessage = "Settings loaded.";
    }

    private void ApplySettings(NeoPixelSettings settings)
    {
        _isLoading = true;
        LedNum = settings.LedNum;
        LedOnNum = settings.LedOnNum;
        StartPosition = settings.StartPosition;
        MaxBrightness = settings.MaxBrightness;
        OnTime = (decimal)settings.OnTime;
        OffTime = (decimal)settings.OffTime;
        FadeInRate = settings.FadeInRate;
        FadeOutRate = settings.FadeOutRate;
        CycleSec = (decimal)settings.CycleSec;
        TimeInterval = (decimal)settings.TimeInterval;
        PatternFreq = settings.PatternFreq;
        MoveReverse = settings.MoveReverse;
        SelectedPalette = PatternPalettes.FirstOrDefault(palette => palette.Pattern == settings.Pattern)
            ?? PatternPalettes[0];
        _isLoading = false;
        OnPropertyChanged(nameof(LedOnNumMaximum));
        OnPropertyChanged(nameof(StartPositionMaximum));
    }

    private NeoPixelSettings CreateSettings() => new()
    {
        LedNum = LedNum,
        LedOnNum = LedOnNum,
        StartPosition = StartPosition,
        MaxBrightness = MaxBrightness,
        OnTime = (double)OnTime,
        OffTime = (double)OffTime,
        FadeInRate = FadeInRate,
        FadeOutRate = FadeOutRate,
        CycleSec = (double)CycleSec,
        TimeInterval = (double)TimeInterval,
        PatternFreq = PatternFreq,
        MoveReverse = MoveReverse,
        Pattern = SelectedPalette?.Pattern ?? "gradation",
    };
}
