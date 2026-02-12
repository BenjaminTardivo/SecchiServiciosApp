using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using SecchiServicios.App.ViewModels;

namespace SecchiServicios.App.Windows;

public partial class CondicionesIVAWindow : Window
{
    public CondicionesIVAWindow(CondicionesIVAWindowViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
    }

    public CondicionesIVAWindow()
    {
        InitializeComponent();
        DataContext = new CondicionesIVAWindowViewModel();
    }
}