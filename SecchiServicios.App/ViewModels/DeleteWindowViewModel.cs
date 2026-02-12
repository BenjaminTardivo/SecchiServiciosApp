using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace SecchiServicios.App.ViewModels;

public partial class DeleteWindowViewModel : ObservableObject
{
    
    [ObservableProperty] public string _message;
    
    public int Cantidad { get; }
    
    public Func<bool, Task>? OnConfirm { get; set; }

    public DeleteWindowViewModel(int cantidad)
    {
        Cantidad = cantidad;
        _message = $@"¿Esta seguro/a que desea eliminar {cantidad} elemento/s?";
    }

    [RelayCommand]
    private async Task Confirm()
    {
        if (OnConfirm != null)
            await OnConfirm(true);
    }

    [RelayCommand]
    public async Task Cancel()
    {
        if (OnConfirm != null)
            await OnConfirm(false);
    }
}