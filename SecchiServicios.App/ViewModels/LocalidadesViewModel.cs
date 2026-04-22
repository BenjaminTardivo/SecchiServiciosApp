using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SecchiServicios.Core.Models;
using SecchiServicios.Data.Interfaces;
using SecchiServicios.Data.Repositories;

namespace SecchiServicios.App.ViewModels;

public partial class LocalidadesViewModel : ObservableObject
{
    private readonly IRepository<Localidad> _localidadRepository;
    private readonly Func<int, Window> _eliminarWindowFactory;
    
    //Coleccion bindeada al grind
    public ObservableCollection<Localidad> Localidads { get; set; } = new();
    private List<Localidad> _originalLocalidads;

    public ObservableCollection<Localidad> ModifiedLocalidades { get; set; } = new();
    
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

    public LocalidadesViewModel(IRepository<Localidad> localidadRepository,  Func<int, Window> eliminarWindowFactory)
    {
        _localidadRepository = localidadRepository;
        _eliminarWindowFactory = eliminarWindowFactory;

        ModifiedLocalidades.CollectionChanged += (_, __) =>
        {
            OnPropertyChanged(nameof(CanDelete));
        };

        _ = LoadLocalidades();
    }

    private async Task LoadLocalidades()
    {
        var localidades = await _localidadRepository.GetAllAsync();
        
        Localidads.Clear();

        foreach (var localidad in localidades)
        {
            Localidads.Add(localidad);
        }
        
        _originalLocalidads = localidades
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
    }
}