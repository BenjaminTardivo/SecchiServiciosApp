using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using SecchiServicios.App.ViewModels;

namespace SecchiServicios.App.Windows;

public partial class LocalidadesWindow : Window
{
    public LocalidadesWindow(LocalidadesViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
    }
    
    private void DataGrid_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (DataContext is not LocalidadesViewModel vm)
            return;

        if (sender is DataGrid grid)
        {
            vm.SelectedLocalidades = grid.SelectedItems.Cast<object>().ToList();
        }
    }
}