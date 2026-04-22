using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using SecchiServicios.App.ViewModels.CrearViewModels;

namespace SecchiServicios.App.Windows.CrearWindows;

public partial class CrearLocalidadWindow : Window
{
    public CrearLocalidadWindow(CrearLocalidadViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
    }
}