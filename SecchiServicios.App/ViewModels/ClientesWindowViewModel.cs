using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SecchiServicios.App.ViewModels.CrearViewModels;
using SecchiServicios.App.Windows;
using SecchiServicios.App.Windows.CrearWindows;
using SecchiServicios.Core.Models;
using SecchiServicios.Data.Helpers;
using SecchiServicios.Data.Interfaces;

namespace SecchiServicios.App.ViewModels;

public partial class ClientesWindowViewModel : ObservableObject
{
    private readonly IRepository<Cliente> _clienteRepository;
    private readonly Func<Window> _crearClienteWindowFactory;
    private readonly Func<int, Window> _eliminarWindowFactory;
    
    //Coleccion bindeada al grid
    public ObservableCollection<Cliente> Clientes { get; set; } = new();
    private List<Cliente> _originalClientes = new();

    public ObservableCollection<Cliente> ModifiedClientes { get; set; } = new();
    
    private IList<object> _selectedClientes = new List<object>();

    public IList<object> SelectedClientes
    {
        get => _selectedClientes;
        set
        {
            _selectedClientes = value;
            OnPropertyChanged();
            SaveSelectedItemsChanges();
        }
    }
    
    
    public bool CanDelete => SelectedClientes.Count >= 1;
    
    
    public ClientesWindowViewModel(IRepository<Cliente> clienteRepository, Func<CrearClienteWindow> crearClienteWindowFactory, 
        Func<int, DeleteConfirmationWindow> eliminarWindowFactory)
    {
        _clienteRepository = clienteRepository;
        _crearClienteWindowFactory = crearClienteWindowFactory;
        _eliminarWindowFactory = eliminarWindowFactory;
        
        ModifiedClientes.CollectionChanged += (_, __) =>
        {
            OnPropertyChanged(nameof(CanDelete));
        };

        _ = LoadClientes();

    }

    private async Task LoadClientes()
    {
        var clientes = await _clienteRepository.GetAllAsync();
        
        Clientes.Clear();

        foreach (var cliente in clientes)
        {
            Clientes.Add(cliente);
        }
        
        _originalClientes = clientes
            .Select(cliente => new Cliente
            {
                Id = cliente.Id,
                DNI = cliente.DNI,
                Nombre = cliente.Nombre,
                Direccion = cliente.Direccion,
                Vendedor = cliente.Vendedor
            }).ToList();
    }

    [RelayCommand]
    private void NewCliente()
    {
        var window = _crearClienteWindowFactory();

        if (window.DataContext is CrearClienteWindowViewModel vm)
        {
            vm.OnClienteCreado = cliente =>
            {
                Clientes.Add(cliente);

                _originalClientes.Add(new Cliente()
                {
                    Id = cliente.Id,
                    DNI = cliente.DNI,
                    Nombre = cliente.Nombre,
                    Vendedor = cliente.Vendedor,
                    CondicionIva = cliente.CondicionIva,
                    Direccion = new Direccion
                    {
                        Calle = cliente.Direccion.Calle,
                        Altura = cliente.Direccion.Altura,
                        Provincia = cliente.Direccion.Provincia
                    }
                });

                window.Close();
            };
        }
        
        var owner = App.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop
            ? desktop.MainWindow
            : null;
        
        window.ShowDialog(owner);
    }

    [RelayCommand]
    private async Task SaveChanges()
    {
        var modified = GetModifiedClientes().ToList();

        if (modified.Any())
        {
            await _clienteRepository.UpdateRangeAsync(
                modified,
                keySelector: c => c.Id
                );
        }
    }

    private IEnumerable<Cliente> GetModifiedClientes()
    {
        foreach (var actual in Clientes)
        {
            var original = _originalClientes.First(o => o.Id == actual.Id);
            

            if (actual.Nombre != original.Nombre ||
                !DireccionEqualsHelper.DireccionEquals(original.Direccion, actual.Direccion) ||
                actual.Vendedor != original.Vendedor ||
                actual.CondicionIva != original.CondicionIva)
            {
                yield return actual;
            }
        }
    }

    [RelayCommand]
    private async Task BorrarClientes()
    {
        var cantidad = SelectedClientes.Count;
        Debug.Print($"Borrar {cantidad} Clientes");
        
        if (SelectedClientes.Count == 0)
        {
            return;
        }
        
        
        var window = _eliminarWindowFactory(cantidad);

        if (window.DataContext is DeleteWindowViewModel vm)
        {
            vm.OnConfirm = async confirmed =>
            {
                if (confirmed)
                {
                    var clientesAEliminar = SelectedClientes.Cast<Cliente>().ToList();

                    foreach (var cliente in clientesAEliminar)
                    {
                        await _clienteRepository.DeleteAsync(c => c.Id == cliente.Id);
                        Clientes.Remove(cliente);
                        _originalClientes.RemoveAll(o => o.Id == cliente.Id);
                    }
                    
                    window.Close();
                }
                else
                {
                    window.Close();
                }
            };
        }
        
        var owner = App.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop
            ? desktop.MainWindow
            : null;
        
        window.ShowDialog(owner);
        
    }
    
    private void SaveSelectedItemsChanges()
    {
        if (SelectedClientes is INotifyCollectionChanged obs)
        {
            obs.CollectionChanged += (s,e) =>
            {
                OnPropertyChanged(nameof(CanDelete));
            };
        }
    }
}