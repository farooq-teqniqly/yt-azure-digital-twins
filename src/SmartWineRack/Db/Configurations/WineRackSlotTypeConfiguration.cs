// <copyright file="WineRackSlotTypeConfiguration.cs" company="Teqniqly">
// Copyright (c) Teqniqly. All rights reserved.
// </copyright>

namespace SmartWineRack.Db.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class WineRackSlotTypeConfiguration : WineRackEntityTypeConfiguration<WineRackSlot>
    {
        public override void FurtherConfiguration(EntityTypeBuilder<WineRackSlot> builder)
        {
            builder.ToTable("WineRackSlot");
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                .HasMaxLength(10)
                .ValueGeneratedNever();

            builder.HasOne(slot => slot.Bottle)
                .WithOne(bottle => bottle.WineRackSlot)
                .HasForeignKey<Bottle>(bottle => bottle.Id);
        }
    }
}