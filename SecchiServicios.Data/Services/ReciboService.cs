using SecchiServicios.Core.Models;
using SecchiServicios.Data.Repositories;
using Microsoft.EntityFrameworkCore;

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
}