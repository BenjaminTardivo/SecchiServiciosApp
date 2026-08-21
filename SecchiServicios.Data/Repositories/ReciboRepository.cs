using SecchiServicios.Core.Models;
using SecchiServicios.Data.Data;

namespace SecchiServicios.Data.Repositories;

public class ReciboRepository : GenericRepository<Recibo>
{
    public ReciboRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }
}