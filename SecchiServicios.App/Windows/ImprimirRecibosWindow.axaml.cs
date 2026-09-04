using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using SecchiServicios.App.ViewModels;

namespace SecchiServicios.App.Windows;

public partial class ImprimirRecibosWindow : Window
{
    public ImprimirRecibosWindow(ImprimirRecibosViewModel vm)
    {
        InitializeComponent();
        
        DataContext = vm;
    }
    
    private void DatePicker_OnSelectedDateFromChanged(object? sender, DatePickerSelectedValueChangedEventArgs e)
    {
        if (DataContext is not ReemisionViewModel vm)
            return;
        
        if (sender is DatePicker datePicker)
            vm.DateFrom = datePicker.SelectedDate.Value.DateTime;
    }
    
    private void DatePicker_OnSelectedDateToChanged(object? sender, DatePickerSelectedValueChangedEventArgs e)
    {
        if (DataContext is not ReemisionViewModel vm)
            return;
        
        if (sender is DatePicker datePicker)
            vm.DateTo = datePicker.SelectedDate.Value.DateTime;
    }
}