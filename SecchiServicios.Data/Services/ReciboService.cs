using SecchiServicios.Core.Models;
using SecchiServicios.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using SecchiServicios.Core.Enums;

namespace SecchiServicios.Data.Services;

public class ReciboService
{
    private readonly GenericRepository<Recibo> _reciboRepository;
    private readonly GenericRepository<Cliente> _clienteRepository;
    private readonly GenericRepository<ProductoServicio> _productoServicioRepository;

    public ReciboService(
        GenericRepository<Recibo> reciboRepository,
        GenericRepository<Cliente> clienteRepository,
        GenericRepository<ProductoServicio> productoServicioRepository)
    {
        _reciboRepository = reciboRepository;
        _clienteRepository = clienteRepository;
        _productoServicioRepository = productoServicioRepository;
    }

    public async Task<Recibo?> RegistrarRecibo(Cliente clienteRecibo, ProductoServicio productoServicioRecibo)
    {
        var cliente = (await _clienteRepository.GetByPredicateAsync(c => c.Id == clienteRecibo.Id)).FirstOrDefault();

        if (cliente is null)
        {
            return null;
        }

        var productoServicio =
            (await _productoServicioRepository
                .GetByPredicateAsync(p => p.Id == productoServicioRecibo.Id))
                .FirstOrDefault();
        
        if (productoServicio is null)
            return null;

        var recibo = new Recibo
        {
            ClienteId = cliente.Id,
            Fecha = DateTime.Now,
            ProductoServicioId = productoServicio.Id,
            Total = productoServicio.Importe
        };
        
        await _reciboRepository.AddAsync(recibo);
        return recibo;
    }

    public async Task<List<Recibo>> GetRecibosWithEverithing()
    {
        return await _reciboRepository.Query().Include(r => r.ProductoServicio).Include(r => r.Cliente).ToListAsync();
    }

    public async Task<List<Recibo>> GetRecibosByDatePeriod(DateTime dateFrom, DateTime dateTo)
    {
        var recibos = await _reciboRepository.Query().Include(r => r.ProductoServicio)
            .Include(r => r.Cliente).ThenInclude(c => c.Direccion)
            .Include(r => r.Cliente).ThenInclude(c => c.Localidad)
            .Where(r => r.Fecha >= dateFrom && r.Fecha <= dateTo).ToListAsync();
        return recibos;
    }

    public async Task<List<Recibo>> ReemitRecibo(List<Recibo> recibos)
    {
        
        var recibosReemitidos = new List<Recibo>();

        foreach (var recibo in recibos)
        {
            var servicio = (await _productoServicioRepository.GetByPredicateAsync(s => s.Id == recibo.ProductoServicioId)).FirstOrDefault();

            if (servicio.Tipo == TipoProductoServicio.Producto)
            {
                continue;
            }
            
            var cliente = (await _clienteRepository.GetByPredicateAsync(c => c.Id == recibo.ClienteId)).FirstOrDefault();
            
            if (cliente is null)
                continue;

            var newRecibo = new Recibo
            {
                Id = recibo.Id,
                Fecha = DateTime.Now,
                
                ProductoServicioId = servicio.Id,
                ProductoServicio = servicio,
                
                Total = servicio.Importe,
                
                ClienteId = recibo.ClienteId,
                Cliente = cliente,
            };
            
            
            recibosReemitidos.Add(newRecibo);
        }
        
        return recibosReemitidos;
    }
}