using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SecchiServicios.Core.Models;
using SecchiServicios.Data.Interfaces;

namespace SecchiServicios.App.ViewModels.CrearViewModels;

public partial class CrearReciboViewModel : ObservableObject
{
    private readonly IRepository<Recibo> _reciboRepository;
    private readonly IRepository<Cliente> _clienteRepository;
    private readonly IRepository<ProductoServicio> _productoServicioRepository;

    private Cliente? selectedCliente;

    public ObservableCollection<Cliente> Clientes { get; } = new();

    public Cliente? SelectedCliente
    {
        get => selectedCliente;
        set
        {
            if (selectedCliente != value)
            {
                selectedCliente = value;
                OnPropertyChanged(nameof(SelectedCliente));
            }
        }
    }
    
    private ProductoServicio? selectedProductoServicio;

    public ObservableCollection<ProductoServicio> Productos { get; } = new();

    public ProductoServicio? SelectedProductoServicio
    {
        get => selectedProductoServicio;
        set
        {
            if (selectedProductoServicio != value)
            {
                selectedProductoServicio = value;
                OnPropertyChanged(nameof(SelectedProductoServicio));
            }
        }
    }
    
    public Action<Recibo>? OnReciboCreado { get; set; }

    public CrearReciboViewModel(IRepository<Recibo> reciboRepository,
        IRepository<Cliente> clienteRepository,
        IRepository<ProductoServicio> productoServicioRepository)
    {
        _reciboRepository = reciboRepository;
        _clienteRepository = clienteRepository;
        _productoServicioRepository = productoServicioRepository;

        _ = GetClientes();
        _ = GetProductos();
    }

    private async Task GetClientes()
    {
        var clientes = await _clienteRepository.GetAllAsync();
        
        Clientes.Clear();

        foreach (var cliente in clientes)
        {
            Clientes.Add(cliente);
        }
    }

    private async Task GetProductos()
    {
        var productos = await _productoServicioRepository.GetAllAsync();
        
        Productos.Clear();

        foreach (var producto in productos)
        {
            Productos.Add(producto);
        }
    }

    [RelayCommand]
    private async Task CrearRecibo()
    {
        if (SelectedCliente == null)
            return;
        
        if (SelectedProductoServicio == null)
            return;

        var recibo = new Recibo()
        {
            Fecha = DateTime.Today,
            ClienteId = SelectedCliente.Id,
            Cliente = SelectedCliente,
            ProductoServicioId = SelectedProductoServicio.Id,
            ProductoServicio = SelectedProductoServicio,
            Total = SelectedProductoServicio.Importe
        };
        
        await _reciboRepository.AddAsync(recibo);
        
        OnReciboCreado?.Invoke(recibo);
    }
}