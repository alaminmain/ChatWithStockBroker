
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StockMarket.Api.Models;

namespace StockMarket.Api.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Comp> Comps { get; set; }
        public DbSet<SectMaj> SectMajs { get; set; }
        public DbSet<SectMin> SectMins { get; set; }
        public DbSet<MarPrice> MarPrices { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Comp>(entity =>
            {
                entity.Property(e => e.MarFloat).HasColumnType("decimal(18, 2)");
                entity.Property(e => e.PeRatio).HasColumnType("decimal(18, 2)");
            });

            builder.Entity<MarPrice>(entity =>
            {
                entity.Property(e => e.Open).HasColumnType("decimal(18, 2)");
                entity.Property(e => e.High).HasColumnType("decimal(18, 2)");
                entity.Property(e => e.Low).HasColumnType("decimal(18, 2)");
                entity.Property(e => e.Close).HasColumnType("decimal(18, 2)");
                entity.Property(e => e.Chg).HasColumnType("decimal(18, 2)");
                entity.Property(e => e.Vol).HasColumnType("decimal(18, 2)");
                entity.Property(e => e.Val).HasColumnType("decimal(18, 2)");
                entity.Property(e => e.IndxChg).HasColumnType("decimal(18, 2)");
                entity.Property(e => e.MarkCap).HasColumnType("decimal(18, 2)");
            });
        }
    }
}
