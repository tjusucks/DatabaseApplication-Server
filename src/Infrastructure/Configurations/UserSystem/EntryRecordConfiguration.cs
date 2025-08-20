using DbApp.Domain.Entities.UserSystem;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DbApp.Infrastructure.Configurations.UserSystem;

/// <summary>
/// Entity Framework configuration for the EntryRecord entity.
/// Configures the entry_records table structure and relationships.
/// </summary>
public class EntryRecordConfiguration : IEntityTypeConfiguration<EntryRecord>
{
    public void Configure(EntityTypeBuilder<EntryRecord> builder)
    {
        // Table name.
        builder.ToTable("entry_records");

        // Primary key.
        builder.HasKey(er => er.EntryRecordId);

        builder.Property(er => er.EntryRecordId)
            .HasColumnName("entry_record_id")
            .HasColumnType("NUMBER(10)")
            .ValueGeneratedOnAdd();

        // Visitor ID - foreign key.
        builder.Property(er => er.VisitorId)
            .HasColumnName("visitor_id")
            .HasColumnType("NUMBER(10)")
            .IsRequired();

        // Entry time.
        builder.Property(er => er.EntryTime)
            .HasColumnName("entry_time")
            .HasColumnType("TIMESTAMP(0)")
            .IsRequired();

        // Exit time - optional.
        builder.Property(er => er.ExitTime)
            .HasColumnName("exit_time")
            .HasColumnType("TIMESTAMP(0)");

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
            .HasColumnType("TIMESTAMP(0)")
            .IsRequired()
            .HasDefaultValueSql("SYSTIMESTAMP");

        builder.Property(er => er.UpdatedAt)
            .HasColumnName("updated_at")
            .HasColumnType("TIMESTAMP(0)")
            .HasDefaultValueSql("SYSTIMESTAMP");

        // Foreign key relationship to visitors table.
        builder.HasOne<Visitor>()
            .WithMany()
            .HasForeignKey(er => er.VisitorId)
            .OnDelete(DeleteBehavior.Cascade);

#pragma warning disable S125
        // Foreign key relationship to tickets table (will be defined in ticket system)
        // This will be configured when the ticket entities are created
        // builder.HasOne<Ticket>()
        //     .WithMany()
        //     .HasForeignKey(er => er.TicketId)
        //     .OnDelete(DeleteBehavior.SetNull);
#pragma warning restore S125

        // Indexes.
        builder.HasIndex(er => er.VisitorId);

        builder.HasIndex(er => er.EntryTime);

        builder.HasIndex(er => er.ExitTime);

        builder.HasIndex(er => er.TicketId);
    }
}
