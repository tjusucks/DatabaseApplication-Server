using AutoMapper;
using DbApp.Domain.Entities.TicketingSystem;
using DbApp.Domain.Enums.TicketingSystem;
using DbApp.Domain.Interfaces.TicketingSystem;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DbApp.Application.TicketingSystem.Refunds;

/// <summary>
/// 退票命令处理器
/// </summary>
public class RefundCommandHandler(
    ITicketRepository ticketRepository,
    IRefundRepository refundRepository,
    ITransactionManagerService transactionManagerService,
    ILogger<RefundCommandHandler> logger) :
    IRequestHandler<RequestRefundCommand, RefundResultDto>,
    IRequestHandler<ProcessRefundCommand, RefundResultDto>,
    IRequestHandler<BatchRefundCommand, BatchRefundResultDto>
{
    private readonly ITicketRepository _ticketRepository = ticketRepository;
    private readonly IRefundRepository _refundRepository = refundRepository;
    private readonly ITransactionManagerService _transactionManagerService = transactionManagerService;
    private readonly ILogger<RefundCommandHandler> _logger = logger;

    /// <summary>
    /// 处理退票申请
    /// </summary>
    public async Task<RefundResultDto> Handle(RequestRefundCommand request, CancellationToken cancellationToken)
    {
        return await _transactionManagerService.ExecuteInTransactionAsync(async () =>
        {
            _logger.LogInformation("Processing refund request for ticket {TicketId} by visitor {VisitorId}",
                request.TicketId, request.RequestingVisitorId);

            // 获取票据信息
            var ticket = await _ticketRepository.GetByIdAsync(request.TicketId);
            if (ticket == null)
            {
                return new RefundResultDto
                {
                    IsSuccess = false,
                    Message = $"Ticket {request.TicketId} not found"
                };
            }

            // 验证访客权限（非管理员请求需要验证）
            if (!request.IsAdminRequest && ticket.VisitorId != request.RequestingVisitorId)
            {
                return new RefundResultDto
                {
                    IsSuccess = false,
                    Message = "You can only refund your own tickets"
                };
            }

            // 检查是否可以退票
            if (!await _ticketRepository.CanRefundAsync(request.TicketId))
            {
                return new RefundResultDto
                {
                    IsSuccess = false,
                    Message = "This ticket cannot be refunded based on current business rules"
                };
            }

            // 检查是否已经退票
            if (await _refundRepository.IsTicketRefundedAsync(request.TicketId))
            {
                return new RefundResultDto
                {
                    IsSuccess = false,
                    Message = "This ticket has already been refunded"
                };
            }

            // 计算退款金额
            var refundAmount = ticket.TicketType?.BasePrice ?? 0;

            // 创建退款记录
            var refundRecord = new RefundRecord
            {
                TicketId = request.TicketId,
                VisitorId = ticket.VisitorId ?? request.RequestingVisitorId,
                RefundAmount = refundAmount,
                RefundTime = DateTime.UtcNow,
                RefundReason = request.RefundReason,
                RefundStatus = request.IsAdminRequest ? RefundStatus.Approved : RefundStatus.Pending,
                ProcessorId = request.ProcessorId
            };

            var savedRefund = await _refundRepository.AddAsync(refundRecord);

            // 如果是管理员直接批准，更新票据状态
            if (request.IsAdminRequest)
            {
                ticket.Status = TicketStatus.Refunded;
                await _ticketRepository.UpdateAsync(ticket);

                // 更新退款状态为已完成
                savedRefund.RefundStatus = RefundStatus.Completed;
                savedRefund.ProcessingNotes = "Auto-approved by admin";
                await _refundRepository.UpdateAsync(savedRefund);
            }

            _logger.LogInformation("Refund request created successfully. RefundId: {RefundId}, Amount: {Amount}",
                savedRefund.RefundId, refundAmount);

            return new RefundResultDto
            {
                IsSuccess = true,
                RefundId = savedRefund.RefundId,
                RefundAmount = refundAmount,
                Status = savedRefund.RefundStatus,
                Message = request.IsAdminRequest ? "Refund completed successfully" : "Refund request submitted successfully",
                ProcessedAt = request.IsAdminRequest ? DateTime.UtcNow : null,
                RefundReference = GenerateRefundReference(savedRefund.RefundId)
            };
        }, cancellationToken);
    }

    /// <summary>
    /// 处理退票申请审批
    /// </summary>
    public async Task<RefundResultDto> Handle(ProcessRefundCommand request, CancellationToken cancellationToken)
    {
        return await _transactionManagerService.ExecuteInTransactionAsync(async () =>
        {
            _logger.LogInformation("Processing refund decision {Decision} for refund {RefundId} by processor {ProcessorId}",
                request.Decision, request.RefundId, request.ProcessorId);

            // 获取退款记录
            var refundRecord = await _refundRepository.GetByIdAsync(request.RefundId);
            if (refundRecord == null)
            {
                return new RefundResultDto
                {
                    IsSuccess = false,
                    Message = $"Refund record {request.RefundId} not found"
                };
            }

            // 检查状态是否可以处理
            if (refundRecord.RefundStatus != RefundStatus.Pending)
            {
                return new RefundResultDto
                {
                    IsSuccess = false,
                    Message = "This refund request has already been processed"
                };
            }

            // 更新退款记录
            refundRecord.RefundStatus = request.Decision;
            refundRecord.ProcessorId = request.ProcessorId;
            refundRecord.ProcessingNotes = request.ProcessingNotes;

            // 如果批准，更新票据状态
            if (request.Decision == RefundStatus.Approved)
            {
                var ticket = refundRecord.Ticket ?? await _ticketRepository.GetByIdAsync(refundRecord.TicketId);
                if (ticket != null)
                {
                    ticket.Status = TicketStatus.Refunded;
                    await _ticketRepository.UpdateAsync(ticket);
                }

                // 更新为已完成状态
                refundRecord.RefundStatus = RefundStatus.Completed;
            }

            await _refundRepository.UpdateAsync(refundRecord);

            _logger.LogInformation("Refund {RefundId} processed successfully with decision {Decision}",
                request.RefundId, request.Decision);

            return new RefundResultDto
            {
                IsSuccess = true,
                RefundId = refundRecord.RefundId,
                RefundAmount = request.Decision == RefundStatus.Approved ? refundRecord.RefundAmount : 0,
                Status = refundRecord.RefundStatus,
                Message = $"Refund request {(request.Decision == RefundStatus.Approved ? "approved" : "rejected")} successfully",
                ProcessedAt = DateTime.UtcNow,
                RefundReference = request.Decision == RefundStatus.Approved ? GenerateRefundReference(refundRecord.RefundId) : null
            };
        }, cancellationToken);
    }

    /// <summary>
    /// 处理批量退票
    /// </summary>
    public async Task<BatchRefundResultDto> Handle(BatchRefundCommand request, CancellationToken cancellationToken)
    {
        var result = new BatchRefundResultDto
        {
            TotalRequested = request.TicketIds.Count
        };

        return await _transactionManagerService.ExecuteInTransactionAsync(async () =>
        {
            _logger.LogInformation("Processing batch refund for {Count} tickets by processor {ProcessorId}",
                request.TicketIds.Count, request.ProcessorId);

            foreach (var ticketId in request.TicketIds)
            {
                try
                {
                    var refundCommand = new RequestRefundCommand
                    {
                        TicketId = ticketId,
                        RequestingVisitorId = 0, // Not applicable for admin batch processing
                        RefundReason = request.RefundReason,
                        IsAdminRequest = true,
                        ProcessorId = request.ProcessorId
                    };

                    var refundResult = await Handle(refundCommand, cancellationToken);

                    result.Results.Add(refundResult);

                    if (refundResult.IsSuccess)
                    {
                        result.SuccessfulRefunds++;
                        result.TotalRefundAmount += refundResult.RefundAmount;
                    }
                    else
                    {
                        result.FailedRefunds++;
                        result.Errors.Add($"Ticket {ticketId}: {refundResult.Message}");
                    }
                }
                catch (Exception ex)
                {
                    result.FailedRefunds++;
                    result.Errors.Add($"Ticket {ticketId}: {ex.Message}");
                    _logger.LogError(ex, "Error processing refund for ticket {TicketId} in batch", ticketId);
                }
            }

            _logger.LogInformation("Batch refund completed. Success: {Success}, Failed: {Failed}, Total Amount: {Amount}",
                result.SuccessfulRefunds, result.FailedRefunds, result.TotalRefundAmount);

            return result;
        }, cancellationToken);
    }

    /// <summary>
    /// 生成退款参考号
    /// </summary>
    private static string GenerateRefundReference(int refundId)
    {
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        return $"REF{timestamp}{refundId:D6}";
    }
}
