using AssetsAPI.Classes;
using Microsoft.EntityFrameworkCore;

namespace AssetsAPI.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
    }
}
