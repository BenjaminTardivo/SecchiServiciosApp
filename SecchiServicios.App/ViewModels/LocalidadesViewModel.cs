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
using SecchiServicios.Data.Repositories;

namespace SecchiServicios.App.ViewModels;

public partial class LocalidadesViewModel : ObservableObject
{
    private readonly IRepository<Localidad> _localidadRepository;
    private readonly Func<Window> _crearLocalidadWindowFactory;
    private readonly Func<int, Window> _eliminarWindowFactory;
    
    //Coleccion bindeada al grind
    public ObservableCollection<Localidad> Localidades { get; set; } = new();
    private List<Localidad> _originalLocalidades;

    public ObservableCollection<Localidad> ModifiedLocalidades { get; set; } = new();

    public FilterViewModel Filter { get; }
    
    private IList<object> _selectedLocalidades = new List<object>();

    public IList<object> SelectedLocalidades
    {
        get => _selectedLocalidades;
        set
        {
            _selectedLocalidades = value;
            OnPropertyChanged();
        }
    }
    
    public bool CanDelete => SelectedLocalidades.Count >= 1;

    public LocalidadesViewModel(IRepository<Localidad> localidadRepository, Func<int, DeleteConfirmationWindow> eliminarWindowFactory,
        Func<CrearLocalidadWindow> crearLocalidadWindowFactory)
    {
        _localidadRepository = localidadRepository;
        _eliminarWindowFactory = eliminarWindowFactory;
        _crearLocalidadWindowFactory = crearLocalidadWindowFactory;

        Filter = new FilterViewModel(new[]
        {
            new FilterPropertyInfo { DisplayName = "Nombre", PropertyPath = "Nombre", PropertyType = FilterPropertyType.String },
            new FilterPropertyInfo { DisplayName = "CP", PropertyPath = "CodigoPostal", PropertyType = FilterPropertyType.String },
            new FilterPropertyInfo { DisplayName = "Provincia", PropertyPath = "Provincia", PropertyType = FilterPropertyType.String },
        });

        Filter.OnFilterApplied += ApplyFilter;
        Filter.OnFilterCleared += ClearFilter;

        ModifiedLocalidades.CollectionChanged += (_, __) =>
        {
            OnPropertyChanged(nameof(CanDelete));
        };

        _ = LoadLocalidades();
    }

    private void ApplyFilter()
    {
        Localidades.Clear();
        foreach (var localidad in _originalLocalidades.Where(l => Filter.MatchesFilter(l)))
        {
            Localidades.Add(localidad);
        }
    }

    private void ClearFilter()
    {
        Localidades.Clear();
        foreach (var localidad in _originalLocalidades)
        {
            Localidades.Add(localidad);
        }
    }

    private async Task LoadLocalidades()
    {
        var localidades = await _localidadRepository.GetAllAsync();
        
        Localidades.Clear();

        foreach (var localidad in localidades)
        {
            Localidades.Add(localidad);
        }
        
        _originalLocalidades = localidades
            .Select(localidad => new Localidad
            {
                Id = localidad.Id,
                Nombre = localidad.Nombre,
                CodigoPostal = localidad.CodigoPostal,
                Provincia = localidad.Provincia
            }).ToList();
    }

    [RelayCommand]
    private void NewLocalidad()
    {
        var window = _crearLocalidadWindowFactory();
        
        if (window.DataContext is CrearLocalidadViewModel vm)
        {
            vm.OnLocalidadCreada = localidad =>
            {
                Localidades.Add(localidad);

                _originalLocalidades.Add(new Localidad()
                {
                    Id = localidad.Id,
                    Nombre = localidad.Nombre,
                    CodigoPostal = localidad.CodigoPostal,
                    Provincia = localidad.Provincia
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
        var modified = GetModifiedLocalidades().ToList();

        if (modified.Any())
        {
            await _localidadRepository.UpdateRangeAsync(
                modified,
                keySelector: l => l.Id
                );
        }
    }

    private IEnumerable<Localidad> GetModifiedLocalidades()
    {
        foreach (var actual in Localidades)
        {
            var original = _originalLocalidades.First(o => o.Id == actual.Id);

            if (actual.Nombre != original.Nombre ||
                actual.CodigoPostal != original.CodigoPostal
                || actual.Provincia != original.Provincia)
            {
                yield return actual;
            }
        }
    }

    [RelayCommand]
    private async Task BorrarLocalidades()
    {
        var cantidad = SelectedLocalidades.Count;

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
                    var localidadesAEliminar = SelectedLocalidades.Cast<Localidad>().ToList();

                    foreach (var localidad in localidadesAEliminar)
                    {
                        await _localidadRepository.DeleteAsync(l => l.Id == localidad.Id);
                        Localidades.Remove(localidad);
                        _originalLocalidades.RemoveAll(l => l.Id == localidad.Id);
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