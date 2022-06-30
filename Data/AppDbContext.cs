using CommandZone.Models;
using Microsoft.EntityFrameworkCore;

namespace CommandZone.Data
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options): base(options)
        {
            
        }
        public DbSet<Command> Commands=> Set<Command>();
    }
}