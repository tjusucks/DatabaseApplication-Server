using DbApp.Domain.Interfaces;
using DbApp.Domain.Interfaces.TicketingSystem;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace DbApp.Application.TicketingSystem.Promotions;

public class DeletePromotionCommand : IRequest<bool>
{
    public int PromotionId { get; }

    public DeletePromotionCommand(int promotionId)
    {
        PromotionId = promotionId;
    }
}

public class DeletePromotionCommandHandler : IRequestHandler<DeletePromotionCommand, bool>
{
    private readonly IPromotionRepository _promotionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeletePromotionCommandHandler(IPromotionRepository promotionRepository, IUnitOfWork unitOfWork)
    {
        _promotionRepository = promotionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeletePromotionCommand request, CancellationToken cancellationToken)
    {
        // Get with details to ensure related entities are loaded for deletion if needed
        var promotion = await _promotionRepository.GetByIdWithDetailsAsync(request.PromotionId);
        if (promotion == null)
        {
            return false;
        }

        _promotionRepository.Delete(promotion);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}