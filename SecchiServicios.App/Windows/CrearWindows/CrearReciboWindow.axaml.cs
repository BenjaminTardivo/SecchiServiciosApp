using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using SecchiServicios.App.ViewModels.CrearViewModels;

namespace SecchiServicios.App.Windows.CrearWindows;

public partial class CrearReciboWindow : Window
{
    public CrearReciboWindow(CrearReciboViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
    }
}