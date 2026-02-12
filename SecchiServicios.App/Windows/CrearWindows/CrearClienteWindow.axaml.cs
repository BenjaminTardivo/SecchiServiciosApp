using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using SecchiServicios.App.ViewModels;
using SecchiServicios.App.ViewModels.CrearViewModels;

namespace SecchiServicios.App.Windows.CrearWindows;

public partial class CrearClienteWindow : Window
{

    public CrearClienteWindow(CrearClienteWindowViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
    }
}