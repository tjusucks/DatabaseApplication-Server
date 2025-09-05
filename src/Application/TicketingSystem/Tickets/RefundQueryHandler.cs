using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using DbApp.Domain.Interfaces.TicketingSystem;

namespace DbApp.Application.TicketingSystem.Tickets;

/// <summary>
/// 退票查询处理器
/// </summary>
public class RefundQueryHandler(
    IRefundRepository refundRepository,
    IMapper mapper,
    ILogger<RefundQueryHandler> logger) :
    IRequestHandler<SearchRefundRecordsQuery, RefundSearchResultDto>,
    IRequestHandler<GetRefundByIdQuery, RefundDetailsDto?>,
    IRequestHandler<GetRefundByTicketIdQuery, RefundDetailsDto?>,
    IRequestHandler<GetRefundStatsQuery, RefundStatsDto>
{
    private readonly IRefundRepository _refundRepository = refundRepository;
    private readonly IMapper _mapper = mapper;
    private readonly ILogger<RefundQueryHandler> _logger = logger;

    /// <summary>
    /// 搜索退款记录
    /// </summary>
    public async Task<RefundSearchResultDto> Handle(SearchRefundRecordsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Searching refund records with keyword: {Keyword}", request.Keyword);

            var refunds = await _refundRepository.SearchAsync(
                request.Keyword,
                request.StartDate,
                request.EndDate,
                request.VisitorId,
                request.TicketTypeId,
                request.Status,
                request.MinAmount,
                request.MaxAmount,
                request.SortBy,
                request.Descending,
                request.Page,
                request.PageSize);

            var totalCount = await _refundRepository.CountAsync(
                request.Keyword,
                request.StartDate,
                request.EndDate,
                request.VisitorId,
                request.TicketTypeId,
                request.Status,
                request.MinAmount,
                request.MaxAmount);

            var refundDtos = _mapper.Map<List<RefundDetailsDto>>(refunds);

            return new RefundSearchResultDto
            {
                Refunds = refundDtos,
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching refund records");
            throw new InvalidOperationException("Failed to search refund records", ex);
        }
    }

    /// <summary>
    /// 根据ID获取退款记录详情
    /// </summary>
    public async Task<RefundDetailsDto?> Handle(GetRefundByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var refund = await _refundRepository.GetByIdAsync(request.RefundId);
            return refund != null ? _mapper.Map<RefundDetailsDto>(refund) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting refund by ID {RefundId}", request.RefundId);
            throw new InvalidOperationException($"Failed to get refund by ID {request.RefundId}", ex);
        }
    }

    /// <summary>
    /// 根据票ID获取退款记录
    /// </summary>
    public async Task<RefundDetailsDto?> Handle(GetRefundByTicketIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var refund = await _refundRepository.GetByTicketIdAsync(request.TicketId);
            return refund != null ? _mapper.Map<RefundDetailsDto>(refund) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting refund by ticket ID {TicketId}", request.TicketId);
            throw new InvalidOperationException($"Failed to get refund by ticket ID {request.TicketId}", ex);
        }
    }

    /// <summary>
    /// 获取退款统计信息
    /// </summary>
    public async Task<RefundStatsDto> Handle(GetRefundStatsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Getting refund statistics");

            var stats = await _refundRepository.GetStatisticsAsync(
                request.StartDate,
                request.EndDate,
                request.TicketTypeId,
                request.Status);

            return _mapper.Map<RefundStatsDto>(stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting refund statistics");
            throw new InvalidOperationException("Failed to get refund statistics", ex);
        }
    }
}
