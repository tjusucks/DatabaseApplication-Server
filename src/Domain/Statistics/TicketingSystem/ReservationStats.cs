namespace DbApp.Domain.Statistics.TicketingSystem;

/// <summary>
/// Domain layer reservation statistics entity, used for aggregation and calculation.
/// </summary>
public class ReservationStats
{
    public int TotalReservations { get; set; }
    public decimal TotalSpent { get; set; }
    public decimal TotalRefunded { get; set; }
    public decimal NetSpent { get; set; }
    public int TotalTickets { get; set; }
    public decimal AverageOrderValue { get; set; }
    public DateTime? FirstReservation { get; set; }
    public DateTime? LastReservation { get; set; }
}
