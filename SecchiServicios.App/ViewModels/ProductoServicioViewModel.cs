using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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

namespace SecchiServicios.App.ViewModels;

public partial class ProductoServicioViewModel : ObservableObject
{
    private readonly IRepository<ProductoServicio> _productoServicioRepository;
    private readonly Func<Window>  _crearProductoServicioWindowFactory;
    private readonly Func<int, Window> _eliminarWindowFactory;
    
    //Coleccion bindeada al grid
    public ObservableCollection<ProductoServicio> PyS { get; set; } = new();
    private List<ProductoServicio> _originalPyS = new();

    public ObservableCollection<ProductoServicio> ModifiedPyS { get; set; } = new();

    public FilterViewModel Filter { get; }


    private IList<object> _selectedPyS = new List<object>();
    
    public IList<object> SelectedPyS
    {
        get => _selectedPyS;
        set
        {
            _selectedPyS = value;
            OnPropertyChanged();
            SaveSelectedItemsChanges();
        }
    }

    public bool CanDelete => SelectedPyS.Count >= 1;

    public ProductoServicioViewModel(IRepository<ProductoServicio> productoServicioRepository, Func<CrearProductoServicioWindow> crearProductoServicioWindowFactory,
        Func<int, DeleteConfirmationWindow> eliminarWindowFactory)
    {
        _productoServicioRepository = productoServicioRepository;
        _crearProductoServicioWindowFactory = crearProductoServicioWindowFactory;
        _eliminarWindowFactory = eliminarWindowFactory;

        Filter = new FilterViewModel(new[]
        {
            new FilterPropertyInfo { DisplayName = "Nombre", PropertyPath = "Nombre", PropertyType = FilterPropertyType.String },
            new FilterPropertyInfo { DisplayName = "Descripcion", PropertyPath = "Descripcion", PropertyType = FilterPropertyType.String },
            new FilterPropertyInfo { DisplayName = "Importe", PropertyPath = "Importe", PropertyType = FilterPropertyType.Numeric },
            new FilterPropertyInfo { DisplayName = "Tipo", PropertyPath = "Tipo", PropertyType = FilterPropertyType.String },
        });

        Filter.OnFilterApplied += ApplyFilter;
        Filter.OnFilterCleared += ClearFilter;

        ModifiedPyS.CollectionChanged += (_, __) => OnPropertyChanged(nameof(CanDelete));

        _ = LoadPyS();
    }

    private void ApplyFilter()
    {
        PyS.Clear();
        foreach (var ps in _originalPyS.Where(p => Filter.MatchesFilter(p)))
        {
            PyS.Add(ps);
        }
    }

    private void ClearFilter()
    {
        PyS.Clear();
        foreach (var ps in _originalPyS)
        {
            PyS.Add(ps);
        }
    }

    private async Task LoadPyS()
    {
        var productos = await _productoServicioRepository.GetAllAsync();
        
        PyS.Clear();

        foreach (var producto in productos)
        {
            PyS.Add(producto);
        }
        
        _originalPyS = productos
            .Select(producto => new ProductoServicio
            {
                Id = producto.Id,
                Nombre = producto.Nombre,
                Descripcion = producto.Descripcion,
                Importe = producto.Importe,
                Tipo = producto.Tipo
            }).ToList();
    }

    [RelayCommand]
    private void AddNewProductOrService()
    {
        var window = _crearProductoServicioWindowFactory();

        if (window.DataContext is CrearProductoServicioViewModel vm)
        {
            vm.OnProductoServicioCreado = productoServicio =>
            {
                PyS.Add(productoServicio);

                _originalPyS.Add(new ProductoServicio()
                {
                    Id = productoServicio.Id,
                    Nombre = productoServicio.Nombre,
                    Descripcion = productoServicio.Descripcion,
                    Importe = productoServicio.Importe,
                    Tipo = productoServicio.Tipo
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
        var modified = GetModifiedPyS().ToList();

        if (modified.Any())
        {
            await _productoServicioRepository.UpdateRangeAsync(
                modified, keySelector: ps => ps.Id);
        }
    }

    private IEnumerable<ProductoServicio> GetModifiedPyS()
    {
        foreach (var actual in PyS)
        {
            var original = _originalPyS.First(o => o.Id == actual.Id);

            if (actual.Nombre != original.Nombre ||
                actual.Descripcion != original.Descripcion ||
                actual.Importe != original.Importe ||
                actual.Tipo != original.Tipo)
            {
                yield return actual;
            }
        }
    }
    
    [RelayCommand]
    private async Task BorrarPyS()
    {
        var cantidad = SelectedPyS.Count;
        
        if (cantidad == 0)
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
                    var pySAEliminar = SelectedPyS.Cast<ProductoServicio>().ToList();

                    foreach (var productoServicio in pySAEliminar)
                    {
                        await _productoServicioRepository.DeleteAsync(ps => ps.Id == productoServicio.Id);
                        PyS.Remove(productoServicio);
                        _originalPyS.RemoveAll(o => o.Id == productoServicio.Id);
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
        if (SelectedPyS is INotifyCollectionChanged obs)
        {
            obs.CollectionChanged += (s,e) =>
            {
                OnPropertyChanged(nameof(CanDelete));
            };
        }
    }
}