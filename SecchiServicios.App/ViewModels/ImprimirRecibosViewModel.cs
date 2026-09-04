using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using SecchiServicios.Core.Models;
using SecchiServicios.Data.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SecchiServicios.Data.Services;
using SecchiServicios.Pdf.Services;

namespace SecchiServicios.App.ViewModels;

public partial class ImprimirRecibosViewModel : ObservableObject
{
    private readonly IRepository<Recibo> _reciboRepo;
    private readonly ReciboService _reciboService;
    private readonly ReciboPdfService _pdfService;

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

    public ImprimirRecibosViewModel(IRepository<Recibo> reciboRepo, ReciboService reciboService
    , ReciboPdfService pdfService)
    {
        _reciboRepo = reciboRepo;
        _reciboService = reciboService;
        _pdfService = pdfService;
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
    public async void ImprimirRecibos()
    {
        foreach (var recibo in _recibos)
        {
            _pdfService.GenerarPdf(recibo);
        }
    }
}