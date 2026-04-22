using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using SecchiServicios.App.ViewModels;

namespace SecchiServicios.App.Windows;

public partial class ProductoServicioWindow : Window
{
    public ProductoServicioWindow(ProductoServicioViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
    }
    
    private void DataGrid_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (DataContext is not ProductoServicioViewModel vm)
            return;

        if (sender is DataGrid grid)
        {
            vm.SelectedPyS = grid.SelectedItems.Cast<object>().ToList();
        }
    }
}