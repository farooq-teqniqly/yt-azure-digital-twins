using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SmartWineRack.Db.Configurations
{
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
