// <copyright file="WineRackDbContext.cs" company="Teqniqly">
// Copyright (c) Teqniqly. All rights reserved.
// </copyright>

namespace SmartWineRack.Db
{
    using Microsoft.EntityFrameworkCore;

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

    // TODO: Make these non-nullable per https://endjin.com/blog/2020/09/dotnet-csharp-8-nullable-references-serialization
}
