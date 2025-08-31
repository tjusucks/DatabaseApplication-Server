using DbApp.Domain.Entities.TicketingSystem;
using DbApp.Domain.Interfaces.TicketingSystem;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using DbApp.Domain.Interfaces;

namespace DbApp.Application.TicketingSystem.TicketTypes;

// The Command object
public class UpdateBasePriceCommand : IRequest<bool>
{
    public int TicketTypeId { get; set; }
    public decimal NewBasePrice { get; set; }
    public string Reason { get; set; }
    public int EmployeeId { get; set; }

    // Constructor to easily create the command in the controller
    public UpdateBasePriceCommand(int ticketTypeId, UpdateBasePriceRequest dto)
    {
        TicketTypeId = ticketTypeId;
        NewBasePrice = dto.NewBasePrice;
        Reason = dto.Reason;
        EmployeeId = dto.EmployeeId;
    }
}

// The Handler
public class UpdateBasePriceCommandHandler : IRequestHandler<UpdateBasePriceCommand, bool>
{
    private readonly ITicketTypeRepository _ticketTypeRepository;
    private readonly IPriceHistoryRepository _priceHistoryRepository; // Separate repository for history
    //private readonly IEmployeeRepository _employeeRepository; // Assuming you have one
    private readonly IUnitOfWork _unitOfWork; // To handle transactions

    public UpdateBasePriceCommandHandler(ITicketTypeRepository ticketTypeRepository, IPriceHistoryRepository priceHistoryRepository, /*IEmployeeRepository employeeRepository, */IUnitOfWork unitOfWork)
    {
        _ticketTypeRepository = ticketTypeRepository;
        _priceHistoryRepository = priceHistoryRepository;
        //_employeeRepository = employeeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateBasePriceCommand request, CancellationToken cancellationToken)
    {
        var ticketType = await _ticketTypeRepository.GetByIdAsync(request.TicketTypeId);
        if (ticketType == null || request.NewBasePrice < 0)
        {
            return false;
        }

        // var employee = await _employeeRepository.GetByIdAsync(request.EmployeeId);
        // if (employee == null)
        // {
        //     return false;
        // }
        
        // The business logic with transaction handling is now here.
        var oldPrice = ticketType.BasePrice;
        ticketType.BasePrice = request.NewBasePrice;

        var priceHistory = new PriceHistory
        {
            TicketTypeId = request.TicketTypeId,
            OldPrice = oldPrice,
            NewPrice = request.NewBasePrice,
            ChangeDatetime = DateTime.UtcNow,
            EmployeeId = request.EmployeeId,
            Reason = request.Reason,
            PriceRuleId = null
        };

        await _priceHistoryRepository.AddAsync(priceHistory);
        await _ticketTypeRepository.UpdateAsync(ticketType);

        // SaveChangesAsync will commit the transaction managed by Unit of Work.
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}