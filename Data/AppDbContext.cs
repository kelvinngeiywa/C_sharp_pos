using Microsoft.EntityFrameworkCore;
using my_pos.Models;

namespace my_pos.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options): DbContext(options)
    {
        public DbSet<UserModel> users { get; set; }
    }
}
