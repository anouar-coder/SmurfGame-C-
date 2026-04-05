using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SmurfDAL
{
    public class SmurfDbContextFactory : IDesignTimeDbContextFactory<SmurfDbContext>
    {
        public SmurfDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<SmurfDbContext>();
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=SmurfForestDB;Trusted_Connection=True;MultipleActiveResultSets=true");

            return new SmurfDbContext(optionsBuilder.Options);
        }
    }
}