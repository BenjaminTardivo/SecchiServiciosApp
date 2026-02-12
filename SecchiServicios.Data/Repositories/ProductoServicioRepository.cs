using SecchiServicios.Core.Models;
using SecchiServicios.Data.Data;

namespace SecchiServicios.Data.Repositories;

public class ProductoServicioRepository : GenericRepository<ProductoServicio>
{
    public ProductoServicioRepository(ApplicationDbContext context) : base(context)
    {
        
    }
}