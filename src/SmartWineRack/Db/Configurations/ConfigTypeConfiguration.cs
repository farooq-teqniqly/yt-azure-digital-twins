// <copyright file="ConfigTypeConfiguration.cs" company="Teqniqly">
// Copyright (c) Teqniqly. All rights reserved.
// </copyright>

namespace SmartWineRack.Db.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class ConfigTypeConfiguration : WineRackEntityTypeConfiguration<WineRackConfig>
    {
        public override void FurtherConfiguration(EntityTypeBuilder<WineRackConfig> builder)
        {
            builder.ToTable("WineRackConfig");

            builder.HasKey(e => e.Id);

            builder.HasIndex(e => e.Name, "AK_Config")
                .IsUnique();

            builder.Property(e => e.Id)
                .HasMaxLength(10)
                .ValueGeneratedNever();

            builder.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(60);

            builder.Property(e => e.Value)
                .IsRequired()
                .HasMaxLength(256);
        }
    }
}