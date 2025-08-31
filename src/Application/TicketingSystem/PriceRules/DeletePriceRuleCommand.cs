using DbApp.Domain.Interfaces.TicketingSystem;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using DbApp.Domain.Interfaces;

namespace DbApp.Application.TicketingSystem.PriceRules;

public class DeletePriceRuleCommand : IRequest<bool>
{
    public int RuleId { get; }

    public DeletePriceRuleCommand(int ruleId)
    {
        RuleId = ruleId;
    }
}

public class DeletePriceRuleCommandHandler : IRequestHandler<DeletePriceRuleCommand, bool>
{
    private readonly IPriceRuleRepository _priceRuleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeletePriceRuleCommandHandler(IPriceRuleRepository priceRuleRepository, IUnitOfWork unitOfWork)
    {
        _priceRuleRepository = priceRuleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeletePriceRuleCommand request, CancellationToken cancellationToken)
    {
        var rule = await _priceRuleRepository.GetByIdAsync(request.RuleId);
        if (rule == null)
        {
            return false;
        }

        _priceRuleRepository.Delete(rule);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}