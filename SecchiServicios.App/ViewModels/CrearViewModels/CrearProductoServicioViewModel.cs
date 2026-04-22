using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SecchiServicios.Core.Enums;
using SecchiServicios.Core.Models;
using SecchiServicios.Data.Interfaces;

namespace SecchiServicios.App.ViewModels.CrearViewModels;

public partial class CrearProductoServicioViewModel : ObservableObject
{
    private readonly IRepository<ProductoServicio> _repository;
    
    [ObservableProperty] private string nombre;
    [ObservableProperty] private string descripcion;
    [ObservableProperty] private decimal importe;
    [ObservableProperty] private TipoProductoServicio tipoSeleccionado;

    public TipoProductoServicio[] tipo { get; } = Enum.GetValues<TipoProductoServicio>();
    
    public Action<ProductoServicio> OnProductoServicioCreado { get; set; }

    public CrearProductoServicioViewModel(IRepository<ProductoServicio> repository)
    {
        _repository = repository;
    }
    
    [RelayCommand]
    private async Task Create()
    {
        if (string.IsNullOrWhiteSpace(nombre) ||
            string.IsNullOrWhiteSpace(descripcion))
            return;

        var newProductoServicio = new ProductoServicio()
        {
            Nombre = nombre,
            Descripcion = descripcion,
            Importe = importe,
            Tipo = TipoSeleccionado
        };
        
        await _repository.AddAsync(newProductoServicio);
        OnProductoServicioCreado?.Invoke(newProductoServicio);
    }
}