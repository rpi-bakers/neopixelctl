namespace neopixelctl.Views;

using Avalonia.Controls;
using neopixelctl.ViewModels;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        SizeChanged += (_, _) => UpdatePaletteViewportHeight();
        Opened += (_, _) => UpdatePaletteViewportHeight();
    }

    private void UpdatePaletteViewportHeight()
    {
        if (DataContext is MainWindowViewModel viewModel)
            viewModel.UpdatePaletteViewportHeight(Bounds.Height);
    }
}