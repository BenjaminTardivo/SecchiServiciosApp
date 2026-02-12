using SecchiServicios.Core.Models;
using SecchiServicios.Data.Data;

namespace SecchiServicios.Data.Repositories;

public class ClienteRepository : GenericRepository<Cliente>
{
    public ClienteRepository(ApplicationDbContext context) : base(context)
    {
        
    }
}