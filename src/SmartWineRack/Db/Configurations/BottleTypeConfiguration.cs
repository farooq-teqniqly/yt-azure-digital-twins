// <copyright file="BottleTypeConfiguration.cs" company="Teqniqly">
// Copyright (c) Teqniqly. All rights reserved.
// </copyright>

namespace SmartWineRack.Db.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class BottleTypeConfiguration : WineRackEntityTypeConfiguration<Bottle>
    {
        public override void FurtherConfiguration(EntityTypeBuilder<Bottle> builder)
        {
            builder.ToTable("Bottle");
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                .HasMaxLength(10)
                .ValueGeneratedNever();
        }
    }
}