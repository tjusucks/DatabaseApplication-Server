using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DbApp.Application.Interfaces.TicketingSystem;
using DbApp.Application.DTOs;
using DbApp.Domain.Entities.TicketingSystem;
using DbApp.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace DbApp.Application.Features.TicketingSystem;

public class PriceRepository : IPriceRepository
{
    private readonly ApplicationDbContext _dbContext;

    public PriceRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<TicketTypeSummaryDto>> GetAllTicketTypesAsync()
    {
        return await _dbContext.TicketTypes
            .Select(t => new TicketTypeSummaryDto
            {
                Id = t.TicketTypeId,
                TypeName = t.TypeName,
                BasePrice = t.BasePrice
            })
            .ToListAsync();
    }

    public async Task<TicketTypeSummaryDto> GetTicketTypeByIdAsync(int id)
    {
        var ticketType = await _dbContext.TicketTypes.FindAsync(id);
        if (ticketType == null) return null;
        return new TicketTypeSummaryDto
        {
            Id = ticketType.TicketTypeId,
            TypeName = ticketType.TypeName,
            BasePrice = ticketType.BasePrice
        };
    }

    public async Task<bool> UpdateBasePriceAsync(int ticketTypeId, UpdateBasePriceRequest dto)
    {
        var ticketType = await _dbContext.TicketTypes.FindAsync(ticketTypeId);
        if (ticketType == null || dto.NewBasePrice < 0) return false;

        var employee = await _dbContext.Employees.FindAsync(dto.EmployeeId);
        if (employee == null) return false;

        using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var oldPrice = ticketType.BasePrice;
            ticketType.BasePrice = dto.NewBasePrice;
            _dbContext.TicketTypes.Update(ticketType);

            var priceHistory = new PriceHistory
            {
                TicketTypeId = ticketTypeId,
                OldPrice = oldPrice,
                NewPrice = dto.NewBasePrice,
                ChangeDatetime = DateTime.UtcNow,
                EmployeeId = dto.EmployeeId,
                Reason = dto.Reason,
                PriceRuleId = null
            };
            await _dbContext.PriceHistories.AddAsync(priceHistory);

            await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
            return true;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<List<PriceRuleDto>> GetPriceRulesAsync(int ticketTypeId)
    {
        return await _dbContext.PriceRules
            .Where(r => r.TicketTypeId == ticketTypeId)
            .Select(r => new PriceRuleDto
            {
                Id = r.PriceRuleId,
                RuleName = r.RuleName,
                Priority = r.Priority,
                Price = r.Price,
                EffectiveStartDate = r.EffectiveStartDate,
                EffectiveEndDate = r.EffectiveEndDate
            })
            .ToListAsync();
    }

    public async Task<PriceRuleDto> CreatePriceRuleAsync(int ticketTypeId, CreatePriceRuleRequest dto)
    {
        var ticketType = await _dbContext.TicketTypes.FindAsync(ticketTypeId);
        if (ticketType == null || dto.Price < 0 || dto.EffectiveStartDate >= dto.EffectiveEndDate)
            return null;

        var priceRule = new PriceRule
        {
            TicketTypeId = ticketTypeId,
            RuleName = dto.RuleName,
            Priority = dto.Priority,
            Price = dto.Price,
            EffectiveStartDate = dto.EffectiveStartDate,
            EffectiveEndDate = dto.EffectiveEndDate
        };

        await _dbContext.PriceRules.AddAsync(priceRule);
        await _dbContext.SaveChangesAsync();

        return new PriceRuleDto
        {
            Id = priceRule.PriceRuleId,
            RuleName = priceRule.RuleName,
            Priority = priceRule.Priority,
            Price = priceRule.Price,
            EffectiveStartDate = priceRule.EffectiveStartDate,
            EffectiveEndDate = priceRule.EffectiveEndDate
        };
    }

    public async Task<PriceRuleDto> UpdatePriceRuleAsync(int ruleId, UpdatePriceRuleRequest dto)
    {
        var rule = await _dbContext.PriceRules.FindAsync(ruleId);
        if (rule == null || dto.Price < 0 || dto.EffectiveStartDate >= dto.EffectiveEndDate)
            return null;

        rule.RuleName = dto.RuleName;
        rule.Priority = dto.Priority;
        rule.Price = dto.Price;
        rule.EffectiveStartDate = dto.EffectiveStartDate;
        rule.EffectiveEndDate = dto.EffectiveEndDate;

        _dbContext.PriceRules.Update(rule);
        await _dbContext.SaveChangesAsync();

        return new PriceRuleDto
        {
            Id = rule.PriceRuleId,
            RuleName = rule.RuleName,
            Priority = rule.Priority,
            Price = rule.Price,
            EffectiveStartDate = rule.EffectiveStartDate,
            EffectiveEndDate = rule.EffectiveEndDate
        };
    }

    public async Task<bool> DeletePriceRuleAsync(int ruleId)
    {
        var rule = await _dbContext.PriceRules.FindAsync(ruleId);
        if (rule == null) return false;
        _dbContext.PriceRules.Remove(rule);
        await _dbContext.SaveChangesAsync();
        return true;
    }
}
