using DbApp.Domain.Entities.ResourceSystem;
using DbApp.Domain.Entities.TicketingSystem;
using DbApp.Domain.Entities.UserSystem;
using Microsoft.EntityFrameworkCore;

namespace DbApp.Infrastructure;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{

    // DbSet properties for each entity.
    // User System.
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Blacklist> Blacklists { get; set; }
    public DbSet<Visitor> Visitors { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<StaffTeam> StaffTeams { get; set; }
    public DbSet<TeamMember> TeamMembers { get; set; }
    public DbSet<EntryRecord> EntryRecords { get; set; }

    // Ticketing System.
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

    // Resource System.
    public DbSet<SalaryRecord> SalaryRecords { get; set; }
    public DbSet<Attendance> Attendances { get; set; }
    public DbSet<EmployeeReview> EmployeeReviews { get; set; }
    public DbSet<AmusementRide> AmusementRides { get; set; }
    public DbSet<InspectionRecord> InspectionRecords { get; set; }
    public DbSet<MaintenanceRecord> MaintenanceRecords { get; set; }
    public DbSet<RideTrafficStat> RideTrafficStats { get; set; }
    public DbSet<SeasonalEvent> SeasonalEvents { get; set; }
    public DbSet<FinancialRecord> FinancialRecords { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Apply all configurations.
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSeeding((context, _) =>
        {
            var roles = context.Set<Role>();

            // 检查是否已存在角色数据
            if (!roles.Any(r => r.RoleName == "Admin"))
            {
                roles.Add(new Role
                {
                    RoleName = "Admin",
                    RoleDescription = "System administrator with full access",
                    IsSystemRole = true
                });
            }

            if (!roles.Any(r => r.RoleName == "Manager"))
            {
                roles.Add(new Role
                {
                    RoleName = "Manager",
                    RoleDescription = "Employee manager with management access",
                    IsSystemRole = true
                });
            }

            if (!roles.Any(r => r.RoleName == "Employee"))
            {
                roles.Add(new Role
                {
                    RoleName = "Employee",
                    RoleDescription = "Regular employee with standard access",
                    IsSystemRole = true
                });
            }

            if (!roles.Any(r => r.RoleName == "Visitor"))
            {
                roles.Add(new Role
                {
                    RoleName = "Visitor",
                    RoleDescription = "Visitor or customer with limited access",
                    IsSystemRole = true
                });
            }

            if (!roles.Any(r => r.RoleName == "Blacklisted"))
            {
                roles.Add(new Role
                {
                    RoleName = "Blacklisted",
                    RoleDescription = "Blacklisted user with restricted access",
                    IsSystemRole = true
                });
            }

            context.SaveChanges();
        })
        .UseAsyncSeeding(async (context, _, cancellationToken) =>
        {
            var roles = context.Set<Role>();

            // 检查是否已存在角色数据
            if (!await roles.AnyAsync(r => r.RoleName == "Admin", cancellationToken))
            {
                roles.Add(new Role
                {
                    RoleName = "Admin",
                    RoleDescription = "System administrator with full access",
                    IsSystemRole = true
                });
            }

            if (!await roles.AnyAsync(r => r.RoleName == "Manager", cancellationToken))
            {
                roles.Add(new Role
                {
                    RoleName = "Manager",
                    RoleDescription = "Employee manager with management access",
                    IsSystemRole = true
                });
            }

            if (!await roles.AnyAsync(r => r.RoleName == "Employee", cancellationToken))
            {
                roles.Add(new Role
                {
                    RoleName = "Employee",
                    RoleDescription = "Regular employee with standard access",
                    IsSystemRole = true
                });
            }

            if (!await roles.AnyAsync(r => r.RoleName == "Visitor", cancellationToken))
            {
                roles.Add(new Role
                {
                    RoleName = "Visitor",
                    RoleDescription = "Visitor or customer with limited access",
                    IsSystemRole = true
                });
            }

            if (!await roles.AnyAsync(r => r.RoleName == "Blacklisted", cancellationToken))
            {
                roles.Add(new Role
                {
                    RoleName = "Blacklisted",
                    RoleDescription = "Blacklisted user with restricted access",
                    IsSystemRole = true
                });
            }

            await context.SaveChangesAsync(cancellationToken);
        });

        base.OnConfiguring(optionsBuilder);
    }
}
