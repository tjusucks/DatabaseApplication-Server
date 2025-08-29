using DbApp.Domain.Entities.ResourceSystem;  
using Microsoft.EntityFrameworkCore;  
using Microsoft.EntityFrameworkCore.Metadata.Builders;  
  
namespace DbApp.Infrastructure.Configurations.ResourceSystem;  
  
public class RideEntryRecordConfiguration : IEntityTypeConfiguration<RideEntryRecord>  
{  
    public void Configure(EntityTypeBuilder<RideEntryRecord> builder)  
    {  
        builder.ToTable("ride_entry_records");  
  
        // 主键  
        builder.HasKey(r => r.RideEntryRecordId);  
  
        builder.Property(r => r.RideEntryRecordId)  
            .HasColumnName("ride_entry_record_id")  
            .HasColumnType("NUMBER(10)")  
            .ValueGeneratedOnAdd();  
  
        // 外键 
        builder.Property(r => r.RideId)  
            .HasColumnName("ride_id")  
            .HasColumnType("NUMBER(10)")  
            .IsRequired();  
  
        builder.Property(r => r.VisitorId)  
            .HasColumnName("visitor_id")  
            .HasColumnType("NUMBER(10)")  
            .IsRequired();  
  
        // 时间字段  
        builder.Property(r => r.EntryTime)  
            .HasColumnName("entry_time")  
            .HasColumnType("TIMESTAMP")  
            .IsRequired();  
  
        builder.Property(r => r.ExitTime)  
            .HasColumnName("exit_time")  
            .HasColumnType("TIMESTAMP");  
  
        builder.Property(r => r.TicketId)  
            .HasColumnName("ticket_id")  
            .HasColumnType("NUMBER(10)");  
  
        // 审计字段  
        builder.Property(r => r.CreatedAt)  
            .HasColumnName("created_at")  
            .HasColumnType("TIMESTAMP")  
            .IsRequired()  
            .HasDefaultValueSql("SYSTIMESTAMP");  
  
        builder.Property(r => r.UpdatedAt)  
            .HasColumnName("updated_at")  
            .HasColumnType("TIMESTAMP")  
            .HasDefaultValueSql("SYSTIMESTAMP");  
  
        // 外键关系  
        builder.HasOne(r => r.Ride)  
            .WithMany()  
            .HasForeignKey(r => r.RideId)  
            .OnDelete(DeleteBehavior.Restrict);  
  
        builder.HasOne(r => r.Visitor)  
            .WithMany()  
            .HasForeignKey(r => r.VisitorId)  
            .OnDelete(DeleteBehavior.Cascade);  
  
        builder.HasOne(r => r.Ticket)  
            .WithMany()  
            .HasForeignKey(r => r.TicketId)  
            .OnDelete(DeleteBehavior.SetNull);  
  
        // 索引  
        builder.HasIndex(r => r.RideId);  
        builder.HasIndex(r => r.VisitorId);  
        builder.HasIndex(r => r.EntryTime);  
        builder.HasIndex(r => r.ExitTime);  
    }  
}