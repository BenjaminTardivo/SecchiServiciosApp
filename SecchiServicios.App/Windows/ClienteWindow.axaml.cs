using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using SecchiServicios.App.ViewModels;
using SecchiServicios.Core.Models;

namespace SecchiServicios.App.Windows;

public partial class ClienteWindow : Window
{
    public ClienteWindow(ClientesWindowViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
    }
    
    private void DataGrid_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (DataContext is not ClientesWindowViewModel vm)
            return;

        if (sender is DataGrid grid)
        {
            vm.SelectedClientes = grid.SelectedItems.Cast<object>().ToList();
        }
    }
}