using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
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
    public void ImprimirRecibos()
    {
        if (_recibos == null || _recibos.Count == 0)
            return;

        var path = _pdfService.GenerarPdfMultipage(_recibos);

        Process.Start(new ProcessStartInfo(path)
        {
            UseShellExecute = true
        });
    }
    
    [RelayCommand]
    public async void QuitarRecibos()
    {
        var cantidad = SelectedRecibos.Count;
        
        if  (cantidad == 0)
            return;

        var recibosAQuitar = SelectedRecibos.Cast<Recibo>().ToList();

        foreach (var recibo in recibosAQuitar)
        {
            Recibos.Remove(recibo);
            _recibos.RemoveAll(r => r.Id == recibo.Id);
        }
        
        SelectedRecibos.Clear();
    }
}