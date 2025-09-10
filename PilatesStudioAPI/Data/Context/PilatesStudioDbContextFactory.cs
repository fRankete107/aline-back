using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace PilatesStudioAPI.Data.Context;

public class PilatesStudioDbContextFactory : IDesignTimeDbContextFactory<PilatesStudioDbContext>
{
    public PilatesStudioDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<PilatesStudioDbContext>();
        
        // Use SQLite for design-time and development
        optionsBuilder.UseSqlite("Data Source=aline.db");
        
        return new PilatesStudioDbContext(optionsBuilder.Options);
    }
}