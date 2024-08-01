namespace Boardgames.Data
{
    using Boardgames.Data.Models;
    using Microsoft.EntityFrameworkCore;

    public class BoardgamesContext : DbContext
    {
        public BoardgamesContext()
        {
        }

        public BoardgamesContext(DbContextOptions options)
            : base(options)
        {
        }

        public virtual DbSet<Creator> Creators { get; set; }
        public virtual DbSet<Boardgame> Boardgames { get; set; }
        public virtual DbSet<Seller> Sellers { get; set; }
        public virtual DbSet<BoardgameSeller> BoardgamesSellers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder
                    .UseLazyLoadingProxies()
                    .UseSqlServer(Configuration.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BoardgameSeller>()
                .HasKey(bgs => new { bgs.BoardgameId, bgs.SellerId });
        }
    }
}
