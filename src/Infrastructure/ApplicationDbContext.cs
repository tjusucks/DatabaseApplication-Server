using DbApp.Domain.Entities.UserSystem;
using Microsoft.EntityFrameworkCore;

using DbApp.Domain.Entities.TicketRelated;
namespace DbApp.Infrastructure;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    // DbSet properties for each entity.
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Blacklist> Blacklists { get; set; }
    public DbSet<Visitor> Visitors { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<StaffTeam> StaffTeams { get; set; }
    public DbSet<TeamMember> TeamMembers { get; set; }
    public DbSet<EntryRecord> EntryRecords { get; set; }
    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<TicketType> TicketTypes { get; set; }
    public DbSet<PriceRule> PriceRules { get; set; }
    public DbSet<PriceHistory> PriceHistories { get; set; }
    public DbSet<Promotion> Promotions { get; set; }
    public DbSet<PromotionTicketType> PromotionTicketTypes { get; set; }
    public DbSet<PromotionCondition> PromotionConditions { get; set; }
    public DbSet<PromotionAction> PromotionActions { get; set; }
    public DbSet<Reservation> Reservations { get; set; }
    public DbSet<ReservationItem> ReservationItems { get; set; }
    public DbSet<RefundRecord> RefundRecords { get; set; }
    public DbSet<Coupon> Coupons { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Apply all configurations.
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
