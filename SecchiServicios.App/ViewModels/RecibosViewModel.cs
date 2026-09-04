using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using SecchiServicios.Data.Interfaces;
using SecchiServicios.Data.Services;

namespace SecchiServicios.App.ViewModels;

public partial class RecibosViewModel : ObservableObject
{
    private readonly IRepository<Recibo> _reciboRepository;
    private readonly Func<Window> _crearReciboWindowFactory;
    private readonly Func<int, Window> _eliminarReciboWindowFactory;
    private readonly ReciboService _reciboService;
    
    //Coleccion bindeada al grid
    public ObservableCollection<Recibo> Recibos { get; set; } = new();
    private List<Recibo> _originalRecibos = new();

    public ObservableCollection<Recibo> ModifiedRecibos { get; set; } = new();

    public FilterViewModel Filter { get; }
    
    private IList<object> _selectedRecibos = new List<object>();
    
    public IList<object> SelectedRecibos
    {
        get => _selectedRecibos;
        set
        {
            _selectedRecibos = value;
            OnPropertyChanged();
        }
    }
    
    public bool CanDelete => SelectedRecibos.Count >= 1;

    
    public RecibosViewModel(IRepository<Recibo> reciboRepository, Func<CrearReciboWindow> crearReciboWindowFactory,
        Func<int, DeleteConfirmationWindow> eliminarReciboWindowFactory, ReciboService reciboService)
    {
        _reciboRepository = reciboRepository;
        _crearReciboWindowFactory = crearReciboWindowFactory;
        _eliminarReciboWindowFactory = eliminarReciboWindowFactory;
        _reciboService = reciboService;

        Filter = new FilterViewModel(new[]
        {
            new FilterPropertyInfo { DisplayName = "Cliente", PropertyPath = "Cliente.Nombre", PropertyType = FilterPropertyType.String },
            new FilterPropertyInfo { DisplayName = "Producto/Servicio", PropertyPath = "ProductoServicio.Nombre", PropertyType = FilterPropertyType.String },
            new FilterPropertyInfo { DisplayName = "Fecha", PropertyPath = "Fecha", PropertyType = FilterPropertyType.String },
            new FilterPropertyInfo { DisplayName = "Monto", PropertyPath = "Total", PropertyType = FilterPropertyType.Numeric },
        });

        Filter.OnFilterApplied += ApplyFilter;
        Filter.OnFilterCleared += ClearFilter;

        ModifiedRecibos.CollectionChanged += (_, __) =>
        {
            OnPropertyChanged(nameof(CanDelete));
        };

        _ = LoadRecibos();
    }

    private void ApplyFilter()
    {
        Recibos.Clear();
        foreach (var recibo in _originalRecibos.Where(r => Filter.MatchesFilter(r)))
        {
            Recibos.Add(recibo);
        }
    }

    private void ClearFilter()
    {
        Recibos.Clear();
        foreach (var recibo in _originalRecibos)
        {
            Recibos.Add(recibo);
        }
    }

    public async Task LoadRecibos()
    {
        var recibos = await _reciboService.GetRecibosWithEverithing();
        
        Recibos.Clear();

        foreach (var recibo in recibos)
        {
            Recibos.Add(recibo);
        }
        
        _originalRecibos = recibos
            .Select(recibo => new Recibo
            {
                Id = recibo.Id,
                ClienteId = recibo.ClienteId,
                Cliente = recibo.Cliente,
                Fecha = recibo.Fecha,
                ProductoServicioId = recibo.ProductoServicioId,
                ProductoServicio = recibo.ProductoServicio,
                Total = recibo.Total
            }).ToList();
    }

    [RelayCommand]
    private void NewRecibo()
    {
        var window = _crearReciboWindowFactory();

        if (window.DataContext is CrearReciboViewModel vm)
        {
            vm.OnReciboCreado = recibo =>
            {

                Recibos.Add(recibo);

                _originalRecibos.Add(new Recibo()
                {
                    Fecha = recibo.Fecha,
                    ClienteId = recibo.ClienteId,
                    Cliente = recibo.Cliente,
                    Total = recibo.Total,
                    Id = recibo.Id,
                    ProductoServicioId = recibo.ProductoServicioId,
                    ProductoServicio = recibo.ProductoServicio,
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
        var modified = GetModifiedRecibos().ToList();

        if (modified.Any())
        {
            await _reciboRepository.UpdateRangeAsync(
                modified, 
                keySelector: r => r.Id
                );
        }
    }

    private IEnumerable<Recibo> GetModifiedRecibos()
    {
        foreach (var actual in Recibos)
        {
            var original = _originalRecibos.First(recibo => recibo.Id == actual.Id);

            if (actual.Cliente?.Id != original.ClienteId ||
                actual.Fecha != original.Fecha ||
                actual.ProductoServicio?.Id != original.ProductoServicioId ||
                actual.Total != original.Total)
            {
                yield return actual;
            }
        }
    }
    
    [RelayCommand]
    public async Task BorrarRecibos()
    {
        var cantidad = SelectedRecibos.Count;

        if (cantidad == 0)
        {
            return;
        }

        var window = _eliminarReciboWindowFactory(cantidad);

        if (window.DataContext is DeleteWindowViewModel vm)
        {
            vm.OnConfirm = async confirmed =>
            {
                if (confirmed)
                {
                    var recibosAEliminar = SelectedRecibos.Cast<Recibo>().ToList();

                    foreach (var recibo in recibosAEliminar)
                    {
                        _reciboRepository.DeleteAsync(r => r.Id == recibo.Id);
                        Recibos.Remove(recibo);
                        _originalRecibos.RemoveAll(r => recibo.Id == r.Id);
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
}