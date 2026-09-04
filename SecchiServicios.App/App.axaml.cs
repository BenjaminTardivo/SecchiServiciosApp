using System;
using System.Collections.Generic;
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
using SecchiServicios.Data.Services;
using SecchiServicios.Pdf.Services;

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
        
        //Services
        collection.AddScoped<ReciboService>();
        collection.AddScoped<ReciboPdfService>();
        
        // Repos
        collection.AddScoped<IRepository<Cliente>, ClienteRepository>();
        collection.AddScoped<IRepository<ProductoServicio>, ProductoServicioRepository>();
        collection.AddScoped<IRepository<Localidad>, LocalidadRepository>();
        collection.AddScoped<IRepository<Recibo>, ReciboRepository>();
        
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
        collection.AddTransient<RecibosViewModel>();
        collection.AddTransient<CrearReciboViewModel>();
        collection.AddTransient<ReemisionViewModel>();
        collection.AddTransient<ImprimirRecibosViewModel>();
        
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
        collection.AddTransient<RecibosWindow>();
        collection.AddTransient<CrearReciboWindow>();
        collection.AddTransient<ReemisonWindow>();
        collection.AddTransient<ConfirmarReemision>();
        collection.AddTransient<ImprimirRecibosWindow>();

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
        collection.AddTransient<Func<CrearLocalidadWindow>>(sp =>
            () => sp.GetRequiredService<CrearLocalidadWindow>());
        collection.AddTransient<Func<CrearReciboWindow>>(sp =>
            () => sp.GetRequiredService<CrearReciboWindow>());
        collection.AddTransient<Func<List<Recibo>, ConfirmarReemision>>(sp => recibos =>
        {
            var vm = new ConfirmarReemisionViewModel(sp.GetRequiredService<ReciboService>(),
                sp.GetRequiredService<IRepository<Recibo>>(), recibos);

            return new ConfirmarReemision(vm);
        });
        
        
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