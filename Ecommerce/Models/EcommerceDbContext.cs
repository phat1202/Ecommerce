using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Models
{
    public class EcommerceDbContext : DbContext
    {
        public EcommerceDbContext() { }
        public EcommerceDbContext(DbContextOptions<EcommerceDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
    }
}
