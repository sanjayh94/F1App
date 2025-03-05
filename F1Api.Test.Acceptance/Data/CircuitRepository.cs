using F1Api.Models;
using Microsoft.EntityFrameworkCore;

namespace F1Api.Repository
{
    public class CircuitRepository : ICircuitRepository
    {
        private readonly F1ApiDbContext _context;

        public CircuitRepository(F1ApiDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Circuit>> GetAllAsync()
        {
            return await _context.Circuits.ToListAsync();
        }

        public async Task<Circuit?> GetByIdAsync(int id)
        {
            return await _context.Circuits.FindAsync(id);
        }
    }
}
