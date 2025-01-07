using Microsoft.EntityFrameworkCore;
using LiveScore.Server.Models;

namespace LiveScore.Server.Data
{
    public class BasketDbContext : DbContext
    {
        public BasketDbContext(DbContextOptions<BasketDbContext> options) : base(options) { }

        public DbSet<Equipe> Equipes { get; set; } = null!;
        public DbSet<Joueur> Joueurs { get; set; } = null!;
        public DbSet<Coach> Coachs { get; set; } = null!;
        public DbSet<Match> Matchs { get; set; } = null!;
        public DbSet<EvenementMatch> EvenementsMatch { get; set; } = null!;
        public DbSet<ConfigurationMatch> ConfigurationsMatch { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<ChronoMatch> ChronoMatchs { get; set; } = null!;
        public DbSet<CinqDeBase> CinqsDeBase { get; set; } = null!;
        public DbSet<MatchTimeout> Timeouts { get; set; } = null!;
        public DbSet<SubPlayer> SubPlayers { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ** Configuration de l'entité Equipe **
            modelBuilder.Entity<Equipe>(entity =>
            {
                entity.Property(e => e.Nom).HasColumnType("nvarchar(max)").IsRequired();
                entity.Property(e => e.Code).HasMaxLength(10);
                entity.Ignore(e => e.Matches); // Supprime les boucles potentielles avec Match
            });

            // ** Configuration de l'entité Joueur **
            modelBuilder.Entity<Joueur>(entity =>
            {
                entity.HasOne(j => j.Equipe)
                    .WithMany(e => e.Joueurs)
                    .HasForeignKey(j => j.EquipeId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ** Configuration de l'entité Coach **
            modelBuilder.Entity<Equipe>(entity =>
            {
                entity.HasOne(e => e.Coach)
                    .WithOne(c => c.Equipe)
                    .HasForeignKey<Coach>(c => c.EquipeId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ** Configuration de l'entité Match **
            modelBuilder.Entity<Match>(entity =>
            {
                // Relation avec les équipes
                entity.HasOne(m => m.EquipeDomicile)
                    .WithMany()
                    .HasForeignKey(m => m.EquipeDomicileId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(m => m.EquipeExterieur)
                    .WithMany()
                    .HasForeignKey(m => m.EquipeExterieurId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Relation 1:1 avec ChronoMatch
                entity.HasOne(m => m.ChronoMatch)
                    .WithOne(c => c.Match)
                    .HasForeignKey<ChronoMatch>(c => c.MatchId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Relation 1:1 avec ConfigurationMatch
                entity.HasOne(m => m.Configuration)
                    .WithOne(c => c.Match)
                    .HasForeignKey<ConfigurationMatch>(c => c.MatchId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Relations 1:N avec Timeouts et SubPlayers
                entity.HasMany(m => m.Timeouts)
                    .WithOne(t => t.Match)
                    .HasForeignKey(t => t.MatchId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(m => m.SubPlayers)
                    .WithOne(sp => sp.Match)
                    .HasForeignKey(sp => sp.MatchId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ** Configuration de l'entité ChronoMatch **
            modelBuilder.Entity<ChronoMatch>(entity =>
            {
             
                entity.Property(cm => cm.Etat)
                    .IsRequired()
                    .HasMaxLength(20);

                // Index unique pour garantir un ChronoMatch par match
                entity.HasIndex(cm => cm.MatchId).IsUnique();
            });

            // ** Configuration de l'entité ConfigurationMatch **
            modelBuilder.Entity<ConfigurationMatch>(entity =>
            {
                entity.HasOne(c => c.EquipeDomicile)
                    .WithMany()
                    .HasForeignKey(c => c.EquipeDomicileId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(c => c.EquipeExterieur)
                    .WithMany()
                    .HasForeignKey(c => c.EquipeExterieurId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Index unique pour garantir une ConfigurationMatch par match
                entity.HasIndex(c => c.MatchId).IsUnique();
            });

            // ** Configuration de l'entité EvenementMatch **
            modelBuilder.Entity<EvenementMatch>(entity =>
            {
                entity.HasOne(em => em.Match)
                    .WithMany()
                    .HasForeignKey(em => em.MatchId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(em => em.Joueur)
                    .WithMany()
                    .HasForeignKey(em => em.JoueurId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(em => em.Equipe)
                    .WithMany()
                    .HasForeignKey(em => em.EquipeId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(em => em.EncodageUser)
                    .WithMany()
                    .HasForeignKey(em => em.EncodageUserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(em => em.JoueurVictime)
                    .WithMany()
                    .HasForeignKey(em => em.JoueurVictimeId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ** Configuration de l'entité CinqDeBase **
            modelBuilder.Entity<CinqDeBase>(entity =>
            {
                entity.HasOne(c => c.Equipe)
                    .WithMany()
                    .HasForeignKey(c => c.EquipeId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(c => c.Joueur)
                    .WithMany()
                    .HasForeignKey(c => c.JoueurId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(c => c.Match)
                    .WithMany()
                    .HasForeignKey(c => c.MatchId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ** Configuration de l'entité MatchTimeout **
            modelBuilder.Entity<MatchTimeout>(entity =>
            {
                entity.HasOne(t => t.Match)
                    .WithMany(m => m.Timeouts)
                    .HasForeignKey(t => t.MatchId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ** Configuration de l'entité SubPlayer **
            modelBuilder.Entity<SubPlayer>(entity =>
            {
                entity.HasOne(sp => sp.Match)
                    .WithMany(m => m.SubPlayers)
                    .HasForeignKey(sp => sp.MatchId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(sp => sp.PlayerOut)
                    .WithMany()
                    .HasForeignKey(sp => sp.PlayerOutId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(sp => sp.PlayerIn)
                    .WithMany()
                    .HasForeignKey(sp => sp.PlayerInId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ** Configuration de l'entité User **
            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(u => u.Username).HasMaxLength(50).IsRequired();
                entity.Property(u => u.Password).HasMaxLength(100).IsRequired();
            });
        }
    }
}
