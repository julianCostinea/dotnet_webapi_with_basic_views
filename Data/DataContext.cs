using dotnet_rpg.Models;
using Microsoft.EntityFrameworkCore;

namespace dotnet_rpg.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<Character> Characters { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Weapon> Weapons { get; set; }

        public DbSet<Skill> Skills { get; set; }
        // public DbSet<CharacterSkill> CharacterSkills { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Skill>()
                .HasData(new Skill
                    {
                        Id = 1,
                        Name = "Fireball",
                        Damage = 30
                    },
                    new Skill
                    {
                        Id = 2,
                        Name = "Frostbolt",
                        Damage = 25
                    }, new Skill
                    {
                        Id = 3,
                        Name = "Lightning",
                        Damage = 20
                    });
        }

        // protected override void OnModelCreating(ModelBuilder modelBuilder)
        // {   
        //     modelBuilder.Entity<CharacterSkill>()
        //         .HasKey(cs => new { cs.CharacterId, cs.SkillId });
        // }
    }
}