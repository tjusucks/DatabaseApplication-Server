using DbApp.Domain.Entities.UserSystem;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DbApp.Infrastructure.Configurations.UserSystem;

/// <summary>
/// Entity Framework configuration for the RideEntryRecord entity.
/// Configures the ride_entry_records table structure and relationships.
/// </summary>
public class RideEntryRecordConfiguration : IEntityTypeConfiguration<RideEntryRecord>
{
    public void Configure(EntityTypeBuilder<RideEntryRecord> builder)
    {
        // Table name.
        builder.ToTable("ride_entry_records");

        // Primary key.
        builder.HasKey(er => er.RideEntryRecordId);

        builder.Property(er => er.RideEntryRecordId)
            .HasColumnName("ride_entry_record_id")
            .HasColumnType("NUMBER(10)")
            .ValueGeneratedOnAdd();

        // Visitor ID - foreign key.
        builder.Property(er => er.VisitorId)
            .HasColumnName("visitor_id")
            .HasColumnType("NUMBER(10)")
            .IsRequired();

        // Ride ID - foreign key.
        builder.Property(er => er.RideId)
            .HasColumnName("ride_id")
            .HasColumnType("NUMBER(10)")
            .IsRequired();

        // Entry time.
        builder.Property(er => er.EntryTime)
            .HasColumnName("entry_time")
            .HasColumnType("TIMESTAMP")
            .IsRequired();

        // Exit time - optional.
        builder.Property(er => er.ExitTime)
            .HasColumnName("exit_time")
            .HasColumnType("TIMESTAMP");

        // Entry gate.
        builder.Property(er => er.EntryGate)
            .HasColumnName("entry_gate")
            .HasColumnType("VARCHAR2(30 CHAR)")
            .IsRequired();

        // Exit gate - optional.
        builder.Property(er => er.ExitGate)
            .HasColumnName("exit_gate")
            .HasColumnType("VARCHAR2(30 CHAR)");

        // Ticket ID - optional foreign key.
        builder.Property(er => er.TicketId)
            .HasColumnName("ticket_id")
            .HasColumnType("NUMBER(10)");

        // Audit fields.
        builder.Property(er => er.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("TIMESTAMP")
            .IsRequired()
            .HasDefaultValueSql("SYSTIMESTAMP");

        builder.Property(er => er.UpdatedAt)
            .HasColumnName("updated_at")
            .HasColumnType("TIMESTAMP")
            .HasDefaultValueSql("SYSTIMESTAMP");

        // Foreign key relationship to visitors table.
        builder.HasOne(er => er.Visitor)
            .WithMany(v => v.RideEntryRecords)
            .HasForeignKey(er => er.VisitorId)
            .OnDelete(DeleteBehavior.Cascade);

        // Foreign key relationship to rides table.
        builder.HasOne(er => er.Ride)
            .WithMany(r => r.RideEntryRecords)
            .HasForeignKey(er => er.RideId)
            .OnDelete(DeleteBehavior.Cascade);

        // Foreign key relationship to tickets table.
        builder.HasOne(er => er.Ticket)
            .WithMany()
            .HasForeignKey(er => er.TicketId)
            .OnDelete(DeleteBehavior.SetNull);

        // Indexes.
        builder.HasIndex(er => er.VisitorId);
        builder.HasIndex(er => er.RideId);
        builder.HasIndex(er => er.EntryTime);
        builder.HasIndex(er => er.ExitTime);
        builder.HasIndex(er => er.TicketId);
    }
}
