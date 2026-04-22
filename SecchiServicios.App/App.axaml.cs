using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualBasic;
using SecchiServicios.App.ViewModels;
using SecchiServicios.App.ViewModels.CrearViewModels;
using SecchiServicios.App.Windows;
using SecchiServicios.App.Windows.CrearWindows;
using SecchiServicios.Core.Models;
using SecchiServicios.Data.Data;
using SecchiServicios.Data.Interfaces;
using SecchiServicios.Data.Repositories;

namespace SecchiServicios.App;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public static ServiceProvider ServiceProvider { get; private set; }
    
    public override void OnFrameworkInitializationCompleted()
    {
        var collection = new ServiceCollection();
        
        //DB Context
        var dbPath = SecchiServicios.Data.Helpers.AppPaths.GetDataFilePath("SecchiServicios.bd");
        collection.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlite($"Data Source={dbPath}");
        });
        
        collection.AddScoped(typeof(GenericRepository<>));
        
        
        // Repos
        collection.AddScoped<IRepository<Cliente>, ClienteRepository>();
        collection.AddScoped<IRepository<ProductoServicio>, ProductoServicioRepository>();
        collection.AddScoped<IRepository<Localidad>, LocalidadRepository>();
        
        // ViewModels
        collection.AddSingleton<MainViewModel>();
        collection.AddTransient<ClientesWindowViewModel>();
        collection.AddTransient<CondicionesIVAWindowViewModel>();
        collection.AddTransient<CrearClienteWindowViewModel>();
        collection.AddTransient<ProvinciasViewModel>();
        collection.AddTransient<DeleteWindowViewModel>();
        collection.AddTransient<ProductoServicioViewModel>();
        collection.AddTransient<CrearProductoServicioViewModel>();
        collection.AddTransient<LocalidadesViewModel>();
        collection.AddTransient<CrearLocalidadViewModel>();
        
        // Windows
        collection.AddSingleton<MainWindow>();
        collection.AddTransient<ClienteWindow>();
        collection.AddTransient<CondicionesIVAWindow>();
        collection.AddTransient<CrearClienteWindow>();
        collection.AddTransient<ProvinciasWindow>();
        collection.AddTransient<DeleteConfirmationWindow>();
        collection.AddTransient<ProductoServicioWindow>();
        collection.AddTransient<CrearProductoServicioWindow>();
        collection.AddTransient<LocalidadesWindow>();
        collection.AddTransient<CrearLocalidadWindow>();

        //factories
        collection.AddTransient<Func<CrearClienteWindow>>(sp =>
            () => sp.GetRequiredService<CrearClienteWindow>());
        collection.AddTransient<Func<int, DeleteConfirmationWindow>>(sp => cantidad =>
        {
            var vm = new DeleteWindowViewModel(cantidad);
            return new DeleteConfirmationWindow(vm);
        });
        collection.AddTransient<Func<CrearProductoServicioWindow>>(sp =>
            () => sp.GetRequiredService<CrearProductoServicioWindow>());
        
        
        ServiceProvider = collection.BuildServiceProvider();

        using (var scope = ServiceProvider.CreateScope())
        {
            var dbContext =  scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            dbContext.Database.EnsureCreated();
        };
        
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = ServiceProvider.GetRequiredService<MainWindow>();
        }

        base.OnFrameworkInitializationCompleted();
    }
}