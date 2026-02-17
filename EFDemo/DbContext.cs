using Microsoft.EntityFrameworkCore;

namespace EFDemo
{
    public class PagilaContext : DbContext
    {
        public DbSet<Actor> Actors => Set<Actor>();
        public DbSet<Film> Films => Set<Film>();

        public PagilaContext(DbContextOptions<PagilaContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Actor>(entity =>
            {
                entity.ToTable("actor");

                entity.HasKey(e => e.ActorId);

                entity.Property(e => e.ActorId)
                      .HasColumnName("actor_id");

                entity.Property(e => e.FirstName)
                      .HasColumnName("first_name")
                      .HasMaxLength(45)
                      .IsRequired();

                entity.Property(e => e.LastName)
                      .HasColumnName("last_name")
                      .HasMaxLength(45)
                      .IsRequired();

                entity.Property(e => e.LastUpdate)
                      .HasColumnName("last_update");

                entity.HasMany(f => f.Films)
                      .WithMany(a => a.Actors);
            });

            modelBuilder.Entity<Film>(entity =>
            {
                entity.ToTable("film");

                entity.HasKey(f => f.FilmId);

                entity.Property(f => f.FilmId)
                      .HasColumnName("film_id");

                entity.Property(f => f.Title)
                      .HasColumnName("title")
                      .HasMaxLength(255)
                      .IsRequired();

                entity.Property(f => f.Description)
                      .HasColumnName("description");

                entity.Property(f => f.ReleaseYear)
                      .HasColumnName("release_year");

                entity.Property(f => f.LanguageId)
                      .HasColumnName("language_id")
                      .IsRequired();

                entity.Property(f => f.Rating)
                      .HasColumnName("rating")
                      .IsRequired();

                entity.Property(f => f.LastUpdate)
                      .HasColumnName("last_update")
                      .HasDefaultValueSql("now()");

                entity.HasMany(f => f.Actors)
                      .WithMany(a => a.Films);
            });

            modelBuilder.Entity<Actor>()
                .HasMany(a => a.Films)
                .WithMany(f => f.Actors)
                .UsingEntity<Dictionary<string, object>>(
                    "film_actor",
                    j => j.HasOne<Film>().WithMany().HasForeignKey("film_id"),
                    j => j.HasOne<Actor>().WithMany().HasForeignKey("actor_id"));
        }
    }
}
