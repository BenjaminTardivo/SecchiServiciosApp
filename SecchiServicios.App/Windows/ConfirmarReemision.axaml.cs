using System.Linq;
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
    
    private void DatePicker_OnSelectedDateChanged(object? sender, DatePickerSelectedValueChangedEventArgs e)
    {
        if (DataContext is not ConfirmarReemisionViewModel vm)
            return;
        
        if (sender is DatePicker datePicker)
            vm.FechaReemision = datePicker.SelectedDate.Value.DateTime;
    }

    private void DataGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (DataContext is not ConfirmarReemisionViewModel vm)
            return;
        
        if (sender is DataGrid grid)
            vm.SelectedRecibos = grid.SelectedItems.Cast<object>().ToList();
    }
}