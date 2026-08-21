using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SecchiServicios.Core.Enums;
using SecchiServicios.Core.Models;
using SecchiServicios.Data.Interfaces;

namespace SecchiServicios.App.ViewModels.CrearViewModels;

public partial class CrearLocalidadViewModel : ObservableObject
{
    private readonly IRepository<Localidad> _localidadRepository;
    
    [ObservableProperty] private string nombre;
    [ObservableProperty] private string codigoPostal;
    [ObservableProperty] private Provincias? provinciaSeleccionada;

    public Provincias[] provinciasList { get; } = Enum.GetValues<Provincias>();
    
    public Action<Localidad> OnLocalidadCreada {get; set;}

    public CrearLocalidadViewModel(IRepository<Localidad> localidadRepository)
    {
        _localidadRepository = localidadRepository;
    }

    [RelayCommand]
    private async Task Crear()
    {
        if (string.IsNullOrWhiteSpace(nombre) ||
            string.IsNullOrWhiteSpace(codigoPostal))
            return;

        var newLocalidad = new Localidad()
        {
            Nombre = nombre,
            CodigoPostal = codigoPostal,
            Provincia = provinciaSeleccionada.Value
        };
        
        await _localidadRepository.AddAsync(newLocalidad);
        
        OnLocalidadCreada?.Invoke(newLocalidad);
    }
}