using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using SecchiServicios.App.ViewModels;

namespace SecchiServicios.App.Windows;

public partial class ConfirmarReemision : Window
{
    public ConfirmarReemision(ConfirmarReemisionViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
    }
}