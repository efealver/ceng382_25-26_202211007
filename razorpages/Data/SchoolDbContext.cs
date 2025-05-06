using Microsoft.EntityFrameworkCore;
using razorpages.Models;
namespace razorpages.Data
{
 public class SchoolDbContext : DbContext
 {
 public SchoolDbContext(DbContextOptions<SchoolDbContext> options) : base(options)
 {
 }
 public DbSet<Class> Classes { get; set; }
 }
}