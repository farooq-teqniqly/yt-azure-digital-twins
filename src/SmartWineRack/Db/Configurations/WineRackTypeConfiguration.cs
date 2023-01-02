// <copyright file="WineRackTypeConfiguration.cs" company="Teqniqly">
// Copyright (c) Teqniqly. All rights reserved.
// </copyright>

namespace SmartWineRack.Db.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class WineRackTypeConfiguration : WineRackEntityTypeConfiguration<WineRack>
    {
        public override void FurtherConfiguration(EntityTypeBuilder<WineRack> builder)
        {
            builder.ToTable("WineRack");
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                .HasMaxLength(10)
                .ValueGeneratedNever();

            builder.HasOne(wineRack => wineRack.Scanner)
                .WithOne(scanner => scanner.WineRack)
                .HasForeignKey<Scanner>(scanner => scanner.Id);

            builder.HasMany(wineRack => wineRack.WineRackSlots).WithOne();
        }
    }
}