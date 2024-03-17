using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Server.Models;

namespace Server.Data
{
    public class PhoneshopIdentityContext : IdentityDbContext<User>
    {
        public PhoneshopIdentityContext(DbContextOptions<PhoneshopIdentityContext> options) : base(options) { }

        public DbSet<Brand> Brands { get; set; }

        public DbSet<Cart> Carts { get; set; }

        public DbSet<Color> Colors { get; set; }

        public DbSet<Combo> Combos { get; set; }

        public DbSet<ComboDetail> ComboDetails { get; set; }

        public DbSet<Comment> Comments { get; set; }

        public DbSet<Image> Images { get; set; }

        public DbSet<Invoice> Invoices { get; set; }

        public DbSet<InvoiceDetail> InvoicesDetail { get; set; }

        public DbSet<Phone> Phones { get; set; }

        public DbSet<PhoneModel> PhoneModels { get; set; }

        public DbSet<ProductType> ProductTypes { get; set; }

        public DbSet<Promotion> Promotion { get; set; }

        public DbSet<PromotionDetail> PromotionDetail { get; set;}

        public DbSet<Ram> Ram { get; set; }

        public DbSet<Review> Reviews { get; set; }

        public DbSet<Storage> Storage { get; set; }

        public DbSet<Wishlist> Wishlist { get; set; }

        public DbSet<Server.Models.PaymentMethod> PaymentMethod { get; set; }

        //public IEnumerable<object> Set<T>()
        //{
        //    throw new NotImplementedException();
        //}
    }
}
