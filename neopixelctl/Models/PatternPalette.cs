using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Media;

namespace neopixelctl.Models;

public sealed class PatternPalette(string pattern, string name, params string[] colors)
{
    public string Pattern { get; } = pattern;
    public string Name { get; } = name;
    public ObservableCollection<PaletteColor> Colors { get; } = new ObservableCollection<PaletteColor>(colors.Select(color => new PaletteColor(color)));
}

public sealed class PaletteColor(string hex)
{
    public string Hex { get; } = hex;
    public IBrush Brush { get; } = new SolidColorBrush(Color.Parse(hex));
}
