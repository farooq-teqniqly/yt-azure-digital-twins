using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SmartWineRack.Db
{
    public class WineRackDbContext : DbContext
    {
        public WineRackDbContext()
        {
        }

        public WineRackDbContext(DbContextOptions<WineRackDbContext> options)
            : base(options)
        {
        }

        public DbSet<WineRackConfig> Configs { get; set; }
        public DbSet<WineRack> WineRacks { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source={Path.Join(Directory.GetCurrentDirectory(), "swr.db")}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(WineRackDbContext).Assembly);
        }
    }

    public class WineRackConfig
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class WineRack
    {
        public WineRack()
        {
            this.WineRackSlots = new HashSet<WineRackSlot>();
        }

        public string Id { get; set; }
        public ICollection<WineRackSlot> WineRackSlots { get; set; }
        public Scanner Scanner { get; set; }
    }

    public class WineRackSlot
    {
        public string Id { get; set; }
        public int SlotNumber { get; set; }
        public Bottle Bottle { get; set; }
    }

    public class Scanner
    {
        public string Id { get; set; }
        public WineRack WineRack { get; set; }
    }

    public class Bottle
    {
        public string Id { get; set; }
        public string UpcCode { get; set; }

        public WineRackSlot WineRackSlot { get; set; }

        public BottleState BottleState { get; set; }
    }

    public enum BottleState
    {
        InPlace = 0,
        RemoveNotScanned
    }
    
}
