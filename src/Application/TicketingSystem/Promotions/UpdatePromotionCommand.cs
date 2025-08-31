using DbApp.Domain.Interfaces;
using DbApp.Domain.Interfaces.TicketingSystem;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace DbApp.Application.TicketingSystem.Promotions;

public class UpdatePromotionCommand : IRequest<PromotionDetailDto?>
{
    public int PromotionId { get; }
    public UpdatePromotionRequest Dto { get; }

    public UpdatePromotionCommand(int promotionId, UpdatePromotionRequest dto)
    {
        PromotionId = promotionId;
        Dto = dto;
    }
}

public class UpdatePromotionCommandHandler : IRequestHandler<UpdatePromotionCommand, PromotionDetailDto?>
{
    private readonly IPromotionRepository _promotionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;

    public UpdatePromotionCommandHandler(IPromotionRepository promotionRepository, IUnitOfWork unitOfWork, IMediator mediator)
    {
        _promotionRepository = promotionRepository;
        _unitOfWork = unitOfWork;
        _mediator = mediator;
    }

    public async Task<PromotionDetailDto?> Handle(UpdatePromotionCommand request, CancellationToken cancellationToken)
    {
        var promotion = await _promotionRepository.GetByIdAsync(request.PromotionId);
        if (promotion == null)
        {
            return null;
        }

        // Note: This only updates top-level properties, as per your original code.
        // Updating child collections would require more complex logic.
        promotion.PromotionName = request.Dto.PromotionName;
        promotion.PromotionType = request.Dto.PromotionType;
        promotion.StartDatetime = request.Dto.StartDate;
        promotion.EndDatetime = request.Dto.EndDate;
        promotion.IsActive = request.Dto.IsActive;

        _promotionRepository.Update(promotion);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return await _mediator.Send(new GetPromotionDetailQuery(promotion.PromotionId), cancellationToken);
    }
}