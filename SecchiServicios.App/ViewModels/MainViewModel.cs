using System;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using SecchiServicios.App.Windows;
using SecchiServicios.Core.Models;
using SecchiServicios.Data.Interfaces;

namespace SecchiServicios.App.ViewModels;

public partial class MainViewModel : ObservableObject
{

    private readonly IServiceProvider _serviceProvider;
    public MainViewModel(IServiceProvider serviceProvider)
    {
        _serviceProvider =  serviceProvider;
    }

    [RelayCommand]
    private void OpenClientesWindow()
    {

        var window = _serviceProvider.GetRequiredService<ClienteWindow>();
        
        
        var owner = App.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop
            ? desktop.MainWindow
            : null;
        
        window.ShowDialog(owner);
    }

    [RelayCommand]
    private void OpenCondicionesIVAWindow()
    {
        var window = _serviceProvider.GetRequiredService<CondicionesIVAWindow>();
        
        var owner = App.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop
            ? desktop.MainWindow
            : null;
        
        window.ShowDialog(owner);
    }
    
    [RelayCommand]
    private void OpenProvinciasWindow()
    {
        var window = _serviceProvider.GetRequiredService<ProvinciasWindow>();
        
        var owner = App.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop
            ? desktop.MainWindow
            : null;
        
        window.ShowDialog(owner);
    }

    [RelayCommand]
    private void OpenServiciosWindow()
    {
        var window = _serviceProvider.GetRequiredService<ProductoServicioWindow>();
        
        var owner = App.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop
            ? desktop.MainWindow
            : null;
        
        window.ShowDialog(owner);
    }

    [RelayCommand]
    private void OpenLocalidadWindow()
    {
        var window = _serviceProvider.GetRequiredService<LocalidadesWindow>();
        
        var owner = App.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop
            ? desktop.MainWindow
            : null;
        
        window.ShowDialog(owner);
    }
}