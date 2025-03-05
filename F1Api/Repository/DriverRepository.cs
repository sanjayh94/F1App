using F1Api.Models;
using Microsoft.EntityFrameworkCore;

namespace F1Api.Repository
{
    public class DriverRepository : IDriverRepository
    {
        private readonly F1DbContext _context;

        public DriverRepository(F1DbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Driver>> GetAllAsync()
        {
            return await _context.Drivers.ToListAsync();
        }

        public async Task<Driver> GetByIdAsync(int id)
        {
            return await _context.Drivers.FindAsync(id);
        }
    }
}