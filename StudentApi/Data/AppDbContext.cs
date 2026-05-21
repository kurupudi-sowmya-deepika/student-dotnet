using Microsoft.EntityFrameworkCore;
using StudentApi.Models;

namespace StudentApi.Data;

// AppDbContext is the bridge between your C# models and the SQLite database.
// Every DbSet<T> property becomes a table in the database.
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
}
