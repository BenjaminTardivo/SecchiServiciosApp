using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using SecchiServicios.App.ViewModels;

namespace SecchiServicios.App.Windows;

public partial class ProvinciasWindow : Window
{
    public ProvinciasWindow()
    {
        InitializeComponent();
    }

    public ProvinciasWindow(ProvinciasViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
    }
}