using Microsoft.EntityFrameworkCore;
using SmurfBL.Entities;

namespace SmurfDAL
{
    public class SmurfDbContext : DbContext
    {
        public SmurfDbContext(DbContextOptions<SmurfDbContext> options)
            : base(options)
        {
        }
        // Ces DbSet = les "tables" dans la base de données

        public DbSet<Forest> Forests { get; set; }
        public DbSet<Creature> Creatures { get; set; }
        public DbSet<Item> Items { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Héritage Creature (TPH)
            modelBuilder.Entity<Creature>()
                .HasDiscriminator<string>("CreatureType")
                .HasValue<Smurf>("Smurf")
                .HasValue<Spider>("Spider")
                .HasValue<BzzFly>("BzzFly");

            // Héritage Item (TPH)
            modelBuilder.Entity<Item>()
                .HasDiscriminator<string>("ItemType")
                .HasValue<Berry>("Berry")
                .HasValue<RedPotion>("RedPotion")
                .HasValue<BluePotion>("BluePotion")
                .HasValue<Sarsaparilla>("Sarsaparilla");

            // Relations
            modelBuilder.Entity<Creature>()
                .HasOne(c => c.Forest)
                .WithMany(f => f.Creatures)
                .HasForeignKey(c => c.ForestId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Item>()
                .HasOne(i => i.Forest)
                .WithMany(f => f.Items)
                .HasForeignKey(i => i.ForestId)
                .OnDelete(DeleteBehavior.Cascade);

            // Index pour les positions
            modelBuilder.Entity<Creature>()
                .HasIndex(c => new { c.X, c.Y });

            modelBuilder.Entity<Item>()
                .HasIndex(i => new { i.X, i.Y });

            base.OnModelCreating(modelBuilder);
        }
    }
}