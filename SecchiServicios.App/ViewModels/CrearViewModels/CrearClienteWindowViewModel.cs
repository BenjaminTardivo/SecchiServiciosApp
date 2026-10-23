using System;
using System.Collections.ObjectModel;
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
    private readonly IRepository<Localidad> _localidadRepository;

    [ObservableProperty] private string nombreCliente;
    [ObservableProperty] private int dni;
    [ObservableProperty] private string vendedor;
    [ObservableProperty] private string telefono;
    [ObservableProperty] private string calle;
    [ObservableProperty] private int altura;
    [ObservableProperty] private Provincias? provinciaSeleccionada;
    [ObservableProperty] private CondicionesIVA? condicionIva;
    
    private Localidad? selectedLocalidad;
    public ObservableCollection<Localidad> Localidades { get; } = new();

    public Localidad SelectedLocalidad
    {
        get => selectedLocalidad;
        set
        {
            if (selectedLocalidad != value)
            {
                selectedLocalidad = value;
                OnPropertyChanged();
            }
        }
    }
    
    public Provincias[] provinciasList { get; } = Enum.GetValues<Provincias>();
    public CondicionesIVA[] condicionIvaList { get; } = Enum.GetValues<CondicionesIVA>();
    
    public Action<Cliente> OnClienteCreado { get; set; }

    public CrearClienteWindowViewModel(IRepository<Cliente> clienteRepository, IRepository<Localidad> localidadRepository)
    {
        _clienteRepository = clienteRepository;
        _localidadRepository = localidadRepository;
        
        _ = GetLocalidades();
    }

    private async Task GetLocalidades()
    {
        var localidades = await _localidadRepository.GetAllAsync();
        
        Localidades.Clear();

        foreach (var localidad in localidades)
        {
            Localidades.Add(localidad);
        }
    }
    [RelayCommand]
    private async Task Crear()
    {
        if (string.IsNullOrWhiteSpace(NombreCliente) ||
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
            Nombre = NombreCliente,
            Telefono = telefono,
            DNI = dni,
            Localidad = SelectedLocalidad,
            LocalidadId = SelectedLocalidad.Id,
            Direccion = newDireccion,
            Vendedor = vendedor,
            CondicionIva = condicionIva.Value
        };
        
        await _clienteRepository.AddAsync(newCliente);
        
        OnClienteCreado?.Invoke(newCliente);
    }
}