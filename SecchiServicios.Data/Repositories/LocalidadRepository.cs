using SecchiServicios.Core.Models;
using SecchiServicios.Data.Data;

namespace SecchiServicios.Data.Repositories;

public class LocalidadRepository : GenericRepository<Localidad>
{
    public LocalidadRepository(ApplicationDbContext context) : base(context)
    {
            
    }
}