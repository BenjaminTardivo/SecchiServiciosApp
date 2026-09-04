using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SecchiServicios.App.Windows;
using SecchiServicios.Core.Models;
using SecchiServicios.Data.Interfaces;
using SecchiServicios.Data.Services;

namespace SecchiServicios.App.ViewModels;

public partial class ReemisionViewModel : ObservableObject
{
    private readonly IRepository<Recibo> _recibosRepository;
    private readonly ReciboService _reciboService;
    private readonly Func<List<Recibo>,Window> _confirmReemisionWindowFactory;

    public ObservableCollection<Recibo> Recibos { get; set; } = new();
    private List<Recibo> _recibos { get; set; }

    public DateTimeOffset? _dateFrom;

    public DateTimeOffset? DateFrom
    {
        get => _dateFrom;
        set
        {
            _dateFrom = value;
            OnPropertyChanged();
        }
    }
    
    public DateTimeOffset? _dateTo;

    public DateTimeOffset? DateTo
    {
        get => _dateTo;
        set
        {
            _dateTo = value;
            OnPropertyChanged();
        }
    }
    

    public ReemisionViewModel(IRepository<Recibo> reciboRepository, 
        ReciboService reciboService,
        Func<List<Recibo>, ConfirmarReemision> confirmReemisionWindowFactory)
    {
        _recibosRepository = reciboRepository;
        _reciboService = reciboService;
        _confirmReemisionWindowFactory = confirmReemisionWindowFactory;
    }

    [RelayCommand]
    public async void FilterRecibos()
    {
        if (_dateFrom == null || _dateTo == null)
        {
            return;
        }
        
        var dateFrom = _dateFrom.Value.DateTime;

        var recibos = await _reciboService.GetRecibosByDatePeriod(dateFrom, _dateTo.Value.DateTime);
        
        Recibos.Clear();

        foreach (var recibo in recibos)
        {
            Recibos.Add(recibo);
        }
        
        _recibos = recibos;
    }

    [RelayCommand]
    public async void Reemitir()
    {
        var window =  _confirmReemisionWindowFactory(_recibos);

        if (window.DataContext is ConfirmarReemisionViewModel vm)
        {
            vm.OnConfirm = recibos =>
            {
                window.Close();
            };
        }
        
        var owner = App.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop
            ? desktop.MainWindow
            : null;

        window.ShowDialog(owner);
    }
}