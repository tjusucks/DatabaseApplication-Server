using System;
using System.Linq;
using System.Threading.Tasks;
using DbApp.Domain.Entities.ResourceSystem;
using DbApp.Domain.Entities.TicketingSystem;
using DbApp.Domain.Enums.ResourceSystem;
using DbApp.Domain.Enums.TicketingSystem;
using DbApp.Domain.Interfaces.TicketingSystem;
using DbApp.Infrastructure;
using Microsoft.EntityFrameworkCore;
using static DbApp.Domain.Exceptions;

namespace DbApp.Infrastructure.Services.TicketingSystem
{
    public class PaymentService : IPaymentService
    {
        private readonly ApplicationDbContext _dbContext;

        public PaymentService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Pay(int reservationId, string paymentMethod)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var reservation = await _dbContext.Reservations
                    .Include(r => r.ReservationItems)
                    .ThenInclude(ri => ri.Tickets)
                    .FirstOrDefaultAsync(r => r.ReservationId == reservationId);

                if (reservation == null)
                {
                    throw new NotFoundException($"Reservation with ID {reservationId} not found.");
                }

                reservation.PaymentStatus = PaymentStatus.Paid;
                reservation.PaymentMethod = paymentMethod;
                reservation.UpdatedAt = DateTime.UtcNow;

                // Generate tickets if payment is successful
                GenerateTickets(reservation);

                var financialRecord = new FinancialRecord
                {
                    Amount = reservation.TotalAmount,
                    TransactionDate = DateTime.UtcNow,
                    TransactionType = TransactionType.Income,
                    Description = $"Payment for reservation {reservationId}",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                };

                _dbContext.FinancialRecords.Add(financialRecord);
                await _dbContext.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task Refund(int reservationId, string reason)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var reservation = await _dbContext.Reservations
                    .Include(r => r.ReservationItems)
                    .ThenInclude(ri => ri.Tickets)
                    .FirstOrDefaultAsync(r => r.ReservationId == reservationId);

                if (reservation == null)
                {
                    throw new NotFoundException($"Reservation with ID {reservationId} not found.");
                }

                reservation.PaymentStatus = PaymentStatus.Refunded;
                reservation.UpdatedAt = DateTime.UtcNow;

                decimal totalRefundAmount = 0;

                foreach (var item in reservation.ReservationItems)
                {
                    // The price for each ticket is stored in the ReservationItem
                    var pricePerTicket = item.TotalAmount / item.Quantity;

                    foreach (var ticket in item.Tickets)
                    {
                        totalRefundAmount += pricePerTicket;
                        var refundRecord = new RefundRecord
                        {
                            TicketId = ticket.TicketId,
                            VisitorId = reservation.VisitorId,
                            RefundAmount = pricePerTicket, // Use price from ReservationItem
                            RefundTime = DateTime.UtcNow,
                            RefundReason = reason,
                            RefundStatus = RefundStatus.Completed,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow,
                        };
                        _dbContext.RefundRecords.Add(refundRecord);
                    }
                }

                // Add a financial record for the expense
                var financialRecord = new FinancialRecord
                {
                    Amount = -totalRefundAmount, // Use a negative value for expenses
                    TransactionDate = DateTime.UtcNow,
                    TransactionType = TransactionType.Expense,
                    Description = $"Refund for reservation {reservationId}. Reason: {reason}",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                };
                _dbContext.FinancialRecords.Add(financialRecord);

                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        private void GenerateTickets(Reservation reservation)
        {
            foreach (var item in reservation.ReservationItems)
            {
                for (int i = 0; i < item.Quantity; i++)
                {
                    var ticket = new Ticket
                    {
                        ReservationItemId = item.ItemId,
                        TicketTypeId = item.TicketTypeId,
                        VisitorId = reservation.VisitorId,
                        SerialNumber = GenerateTicketSerialNumber(),
                        ValidFrom = reservation.VisitDate.Date,
                        ValidTo = reservation.VisitDate.Date.AddDays(1),
                        Status = TicketStatus.Issued
                    };

                    item.Tickets.Add(ticket);
                }
            }
        }

        private static string GenerateTicketSerialNumber()
        {
            return $"TKT{DateTime.UtcNow:yyyyMMdd}{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
        }
    }
}
