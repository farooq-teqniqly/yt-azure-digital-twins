// <copyright file="WineRackEntityTypeConfiguration.cs" company="Teqniqly">
// Copyright (c) Teqniqly. All rights reserved.
// </copyright>

namespace SmartWineRack.Db.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public abstract class WineRackEntityTypeConfiguration<TEntity> : IEntityTypeConfiguration<TEntity> where TEntity : class
    {
        public void Configure(EntityTypeBuilder<TEntity> builder)
        {
            builder.Property<DateTime>("Created")
                .IsRequired()
                .HasDefaultValueSql("current_timestamp")
                .ValueGeneratedOnAdd();

            builder.Property<DateTime>("Modified")
                .IsRequired()
                .HasDefaultValueSql("current_timestamp")
                .ValueGeneratedOnAddOrUpdate();

            this.FurtherConfiguration(builder);
        }

        public abstract void FurtherConfiguration(EntityTypeBuilder<TEntity> builder);
    }
}
