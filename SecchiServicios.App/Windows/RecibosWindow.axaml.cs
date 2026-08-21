using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using SecchiServicios.App.ViewModels;

namespace SecchiServicios.App.Windows;

public partial class RecibosWindow : Window
{
    public RecibosWindow(RecibosViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
    }
    
    private void DataGrid_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (DataContext is not RecibosViewModel vm)
            return;

        if (sender is DataGrid grid)
        {
            vm.SelectedRecibos = grid.SelectedItems.Cast<object>().ToList();
        }
    }
}