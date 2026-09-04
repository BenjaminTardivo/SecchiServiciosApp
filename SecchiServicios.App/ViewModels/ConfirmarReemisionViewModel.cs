using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SecchiServicios.Core.Models;
using SecchiServicios.Data.Interfaces;
using SecchiServicios.Data.Services;

namespace SecchiServicios.App.ViewModels;

public partial class ConfirmarReemisionViewModel : ObservableObject
{
    private readonly ReciboService _reciboService;
    private readonly IRepository<Recibo> _reciboRepository;

    private List<Recibo> _recibos;
    public ObservableCollection<Recibo> Recibos { get; set; } = new();
    
    public Action<List<Recibo>?> OnConfirm { get; set; }

    public ConfirmarReemisionViewModel(ReciboService reciboService, 
        IRepository<Recibo> reciboRepository, List<Recibo> recibos)
    {
        _reciboService = reciboService;
        _reciboRepository = reciboRepository;

        _recibos = recibos;
        
        _ = LoadRecibos();
    }

    private async Task LoadRecibos()
    {
        Recibos.Clear();

        var recibos = await _reciboService.ReemitRecibo(_recibos);

        foreach (var recibo in recibos)
        {
            Recibos.Add(recibo);
        }
    }

    [RelayCommand]
    public async Task Confirm()
    {
        foreach (var recibo in _recibos)
        {
           var newRecibo = new Recibo
           {
               ClienteId = recibo.ClienteId,
               Fecha = DateTime.Now,
               ProductoServicioId = recibo.ProductoServicioId,
               Total = recibo.ProductoServicio.Importe,
           }; 
            
           await _reciboRepository.AddAsync(newRecibo);
        }
        
        OnConfirm?.Invoke(_recibos);
    }
    
}