using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DbApp.Application.Interfaces.Services.TicketingSystem;
using DbApp.Application.DTOs;
using DbApp.Domain.Entities.TicketingSystem;
using DbApp.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace DbApp.Application.Features.TicketingSystem{

    public class PromotionService : IPromotionService
    {
        private readonly ApplicationDbContext _dbContext;

        public PromotionService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<PromotionSummaryDto>> GetAllPromotionsAsync()
        {
            return await _dbContext.Promotions
                .Select(p => new PromotionSummaryDto
                {
                    Id = p.PromotionId,
                    Name = p.PromotionName,
                    Type = p.PromotionType.ToString(),
                    StartDate = p.StartDatetime,
                    EndDate = p.EndDatetime,
                    IsActive = p.IsActive
                })
                .ToListAsync();
        }

        public async Task<PromotionDetailDto> GetPromotionDetailAsync(int id)
        {
            var promotion = await _dbContext.Promotions
                .Include(p => p.PromotionTicketTypes)
                    .ThenInclude(pt => pt.TicketType)
                .Include(p => p.PromotionConditions)
                .Include(p => p.PromotionActions)
                .FirstOrDefaultAsync(p => p.PromotionId == id);

            if (promotion == null) return null;

            return new PromotionDetailDto
            {
                Id = promotion.PromotionId,
                Name = promotion.PromotionName,
                Type = promotion.PromotionType.ToString(),
                StartDate = promotion.StartDatetime,
                EndDate = promotion.EndDatetime,
                IsActive = promotion.IsActive,
                ApplicableTickets = promotion.PromotionTicketTypes
                    .Select(pt => new TicketTypeSummaryDto
                    {
                        Id = pt.TicketType.TicketTypeId,
                        TypeName = pt.TicketType.TypeName,
                        BasePrice = pt.TicketType.BasePrice
                    }).ToList(),
                Conditions = promotion.PromotionConditions
                    .Select(c => new PromotionConditionDto
                    {
                        ConditionId = c.ConditionId,
                        ConditionName = c.ConditionName,
                        ConditionType = c.ConditionType,
                        TicketTypeId = c.TicketTypeId,
                        MinQuantity = c.MinQuantity,
                        MinAmount = c.MinAmount,
                        Priority = c.Priority
                    }).ToList(),
                Actions = promotion.PromotionActions
                    .Select(a => new PromotionActionDto
                    {
                        ActionId = a.ActionId,
                        ActionName = a.ActionName,
                        ActionType = a.ActionType,
                        DiscountPercentage = a.DiscountPercentage,
                        DiscountAmount = a.DiscountAmount
                    }).ToList()
            };
        }

        public async Task<PromotionDetailDto> CreatePromotionAsync(CreatePromotionRequest dto)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var promotion = new Promotion
                {
                    PromotionName = dto.PromotionName,
                    PromotionType = dto.PromotionType,
                    StartDatetime = dto.StartDate,
                    EndDatetime = dto.EndDate,
                    IsActive = true
                };
                await _dbContext.Promotions.AddAsync(promotion);
                await _dbContext.SaveChangesAsync();

                foreach (var ticketTypeId in dto.ApplicableTicketTypeIds)
                {
                    var pt = new PromotionTicketType
                    {
                        PromotionId = promotion.PromotionId,
                        TicketTypeId = ticketTypeId
                    };
                    await _dbContext.PromotionTicketTypes.AddAsync(pt);
                }

                if (dto.Conditions != null && dto.Conditions.Any())
                {
                    var newConditions = dto.Conditions.Select(cond => new PromotionCondition
                    {

                        PromotionId = promotion.PromotionId,
                        ConditionName = cond.ConditionName,
                        ConditionType = cond.ConditionType,
                        TicketTypeId = cond.TicketTypeId,
                        MinQuantity = cond.MinQuantity,
                        MinAmount = cond.MinAmount,
                        Priority = cond.Priority
                    }).ToList();
                    await _dbContext.PromotionConditions.AddRangeAsync(newConditions);
                }

                if (dto.Actions != null && dto.Actions.Any())
                {
                    var newActions = dto.Actions.Select(act => new PromotionAction
                    {
                        PromotionId = promotion.PromotionId,
                        ActionName = act.ActionName,
                        ActionType = act.ActionType,
                        DiscountPercentage = act.DiscountPercentage,
                        DiscountAmount = act.DiscountAmount

                    }).ToList();
                    await _dbContext.PromotionActions.AddRangeAsync(newActions);
                }

                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return await GetPromotionDetailAsync(promotion.PromotionId);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<PromotionDetailDto> UpdatePromotionAsync(int id, UpdatePromotionRequest dto)
        {
            var promotion = await _dbContext.Promotions.FindAsync(id);
            if (promotion == null) return null;

            promotion.PromotionName = dto.PromotionName;
            promotion.PromotionType = dto.PromotionType;
            promotion.StartDatetime = dto.StartDate;
            promotion.EndDatetime = dto.EndDate;
            promotion.IsActive = dto.IsActive;

            _dbContext.Promotions.Update(promotion);
            await _dbContext.SaveChangesAsync();

            return await GetPromotionDetailAsync(id);
        }

        public async Task<bool> DeletePromotionAsync(int id)
        {
            var promotion = await _dbContext.Promotions
                .Include(p => p.PromotionTicketTypes)
                .Include(p => p.PromotionConditions)
                .Include(p => p.PromotionActions)
                .FirstOrDefaultAsync(p => p.PromotionId == id);

            if (promotion == null) return false;

            _dbContext.PromotionTicketTypes.RemoveRange(promotion.PromotionTicketTypes);
            _dbContext.PromotionConditions.RemoveRange(promotion.PromotionConditions);
            _dbContext.PromotionActions.RemoveRange(promotion.PromotionActions);
            _dbContext.Promotions.Remove(promotion);

            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}