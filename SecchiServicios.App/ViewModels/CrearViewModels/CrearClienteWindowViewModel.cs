using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SecchiServicios.Core.Enums;
using SecchiServicios.Core.Models;
using SecchiServicios.Data.Interfaces;

namespace SecchiServicios.App.ViewModels.CrearViewModels;

public partial class CrearClienteWindowViewModel : ObservableObject
{
    private readonly IRepository<Cliente> _clienteRepository;

    [ObservableProperty] private string nombre;
    [ObservableProperty] private int dni;
    [ObservableProperty] private string vendedor;
    [ObservableProperty] private string calle;
    [ObservableProperty] private int altura;
    [ObservableProperty] private Provincias? provinciaSeleccionada;
    [ObservableProperty] private CondicionesIVA? condicionIva;
    
    public Provincias[] provinciasList { get; } = Enum.GetValues<Provincias>();
    public CondicionesIVA[] condicionIvaList { get; } = Enum.GetValues<CondicionesIVA>();
    
    public Action<Cliente> OnClienteCreado { get; set; }

    public CrearClienteWindowViewModel(IRepository<Cliente> clienteRepository)
    {
        _clienteRepository = clienteRepository;
    }

    [RelayCommand]
    private async Task Crear()
    {
        if (string.IsNullOrWhiteSpace(nombre) ||
            string.IsNullOrWhiteSpace(vendedor) ||
            string.IsNullOrWhiteSpace(calle))
            return;
        
        var newDireccion = new Direccion()
        {
            Calle = calle,
            Altura = altura,
            Provincia = provinciaSeleccionada.Value
        };

        var newCliente = new Cliente()
        {
            Nombre = nombre,
            DNI = dni,
            Direccion = newDireccion,
            Vendedor = vendedor,
            CondicionIva = condicionIva.Value
        };
        
        await _clienteRepository.AddAsync(newCliente);
        
        OnClienteCreado?.Invoke(newCliente);
    }
}