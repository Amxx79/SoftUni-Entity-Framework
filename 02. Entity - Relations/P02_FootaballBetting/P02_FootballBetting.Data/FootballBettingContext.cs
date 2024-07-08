using Microsoft.EntityFrameworkCore;
using P02_FootballBetting.Data.Models;
using P02_FootballBetting.Models;

namespace P02_FootballBetting.Data
{
    public class FootballBettingContext : DbContext
    {
        private const string ConnectionString =
            "Server=DESKTOP-P0HOVAK;Database=FootballBookmakerSystem;Integrated Security=True;";

        public FootballBettingContext()
        {
        }

        public FootballBettingContext(DbContextOptions dbo) : base (dbo)
        {
        }

        public DbSet<Country> Countries { get; set; }
        public DbSet<Town> Towns { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Color> Colors { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<PlayerStatistic> PlayerStatistics { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Bet> Bets { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(ConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Defining complex key for PlayersStatistics
            modelBuilder.Entity<PlayerStatistic>(entity =>
            {
                entity.HasKey(pk => new { pk.GameId, pk.PlayerId });
            });

            modelBuilder.Entity<Team>(entity =>
            {
                entity.HasOne(t => t.PrimaryColor)
                    .WithMany(c => c.PrimaryKitTeams)
                    .HasForeignKey(t => t.PrimaryColorId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(t => t.SecondaryColor)
                    .WithMany(c => c.SecondaryKitTeams)
                    .HasForeignKey(t => t.SecondaryColorId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<Game>(entity =>
            {
                entity.HasOne(g => g.HomeTeam)
                    .WithMany(t => t.HomeGame)
                    .HasForeignKey(g => g.HomeTeamId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(g => g.AwayTeam)
                    .WithMany(t => t.AwayGame)
                    .HasForeignKey(g => g.AwayTeamId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<Player>(entity =>
            {
                entity.HasOne(p => p.Town)
                    .WithMany(t => t.Players)
                    .HasForeignKey(p => p.TownId)
                    .OnDelete(DeleteBehavior.NoAction);
            });
        }
    }
}