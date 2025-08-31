namespace DbApp.Application.TicketingSystem.TicketTypes;
    public class TicketTypeSummaryDto
    {
        public int Id { get; set; }
        public string TypeName { get; set; }
        public decimal BasePrice { get; set; }
    }

    public class UpdateBasePriceRequest
    {
        public decimal NewBasePrice { get; set; }
        public string Reason { get; set; }
        public int EmployeeId { get; set; }

    }
