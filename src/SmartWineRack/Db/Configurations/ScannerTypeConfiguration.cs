// <copyright file="ScannerTypeConfiguration.cs" company="Teqniqly">
// Copyright (c) Teqniqly. All rights reserved.
// </copyright>

namespace SmartWineRack.Db.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class ScannerTypeConfiguration : WineRackEntityTypeConfiguration<Scanner>
    {
        public override void FurtherConfiguration(EntityTypeBuilder<Scanner> builder)
        {
            builder.ToTable("Scanner");
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                .HasMaxLength(10)
                .ValueGeneratedNever();
        }
    }
}