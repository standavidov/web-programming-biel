using Microsoft.EntityFrameworkCore;
using GT.Models;

namespace GT.Server.Data
{
    public class GroceryContext : DbContext
    {
        public GroceryContext(DbContextOptions options) : base(options)
        { }

        public DbSet<Grocery> GroceryList { get; set; }
    }

}
