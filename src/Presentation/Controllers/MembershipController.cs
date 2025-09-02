using DbApp.Application.UserSystem.Visitors;
using DbApp.Application.UserSystem.Visitors.DTOs;
using DbApp.Application.UserSystem.Visitors.Services;
using DbApp.Domain.Enums.UserSystem;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DbApp.Presentation.Controllers;

/// <summary>
/// Controller for membership and visitor management operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class MembershipController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    /// <summary>
    /// Registers a new visitor.
    /// </summary>
    /// <param name="dto">The visitor registration data.</param>
    /// <returns>The created visitor ID.</returns>
    [HttpPost("register")]
    public async Task<IActionResult> RegisterVisitor([FromBody] RegisterVisitorDto dto)
    {
        try
        {
            var command = new RegisterVisitorCommand(dto.UserId, dto.Height);
            var visitorId = await _mediator.Send(command);
            
            return CreatedAtAction(nameof(GetVisitor), new { id = visitorId }, new { VisitorId = visitorId });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    /// <summary>
    /// Gets a visitor by ID.
    /// </summary>
    /// <param name="id">The visitor ID.</param>
    /// <returns>The visitor information.</returns>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetVisitor(int id)
    {
        var visitor = await _mediator.Send(new GetVisitorByIdQuery(id));
        if (visitor == null)
            return NotFound(new { Message = $"Visitor with ID {id} not found" });

        var response = new VisitorResponseDto
        {
            VisitorId = visitor.VisitorId,
            VisitorType = visitor.VisitorType,
            Points = visitor.Points,
            MemberLevel = visitor.MemberLevel,
            MemberSince = visitor.MemberSince,
            IsBlacklisted = visitor.IsBlacklisted,
            Height = visitor.Height,
            CreatedAt = visitor.CreatedAt,
            UpdatedAt = visitor.UpdatedAt,
            Username = visitor.User.Username,
            Email = visitor.User.Email,
            DisplayName = visitor.User.DisplayName,
            PhoneNumber = visitor.User.PhoneNumber,
            BirthDate = visitor.User.BirthDate,
            Gender = visitor.User.Gender
        };

        return Ok(response);
    }

    /// <summary>
    /// Gets a visitor by user ID.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <returns>The visitor information.</returns>
    [HttpGet("by-user/{userId:int}")]
    public async Task<IActionResult> GetVisitorByUserId(int userId)
    {
        var visitor = await _mediator.Send(new GetVisitorByUserIdQuery(userId));
        if (visitor == null)
            return NotFound(new { Message = $"Visitor for user ID {userId} not found" });

        var response = new VisitorResponseDto
        {
            VisitorId = visitor.VisitorId,
            VisitorType = visitor.VisitorType,
            Points = visitor.Points,
            MemberLevel = visitor.MemberLevel,
            MemberSince = visitor.MemberSince,
            IsBlacklisted = visitor.IsBlacklisted,
            Height = visitor.Height,
            CreatedAt = visitor.CreatedAt,
            UpdatedAt = visitor.UpdatedAt,
            Username = visitor.User.Username,
            Email = visitor.User.Email,
            DisplayName = visitor.User.DisplayName,
            PhoneNumber = visitor.User.PhoneNumber,
            BirthDate = visitor.User.BirthDate,
            Gender = visitor.User.Gender
        };

        return Ok(response);
    }

    /// <summary>
    /// Gets all visitors.
    /// </summary>
    /// <returns>List of all visitors.</returns>
    [HttpGet]
    public async Task<IActionResult> GetAllVisitors()
    {
        var visitors = await _mediator.Send(new GetAllVisitorsQuery());
        var response = visitors.Select(v => new VisitorResponseDto
        {
            VisitorId = v.VisitorId,
            VisitorType = v.VisitorType,
            Points = v.Points,
            MemberLevel = v.MemberLevel,
            MemberSince = v.MemberSince,
            IsBlacklisted = v.IsBlacklisted,
            Height = v.Height,
            CreatedAt = v.CreatedAt,
            UpdatedAt = v.UpdatedAt,
            Username = v.User.Username,
            Email = v.User.Email,
            DisplayName = v.User.DisplayName,
            PhoneNumber = v.User.PhoneNumber,
            BirthDate = v.User.BirthDate,
            Gender = v.User.Gender
        }).ToList();

        return Ok(response);
    }

    /// <summary>
    /// Gets visitors by type.
    /// </summary>
    /// <param name="type">The visitor type (Regular or Member).</param>
    /// <returns>List of visitors of the specified type.</returns>
    [HttpGet("by-type/{type}")]
    public async Task<IActionResult> GetVisitorsByType(VisitorType type)
    {
        var visitors = await _mediator.Send(new GetVisitorsByTypeQuery(type));
        var response = visitors.Select(v => new VisitorResponseDto
        {
            VisitorId = v.VisitorId,
            VisitorType = v.VisitorType,
            Points = v.Points,
            MemberLevel = v.MemberLevel,
            MemberSince = v.MemberSince,
            IsBlacklisted = v.IsBlacklisted,
            Height = v.Height,
            CreatedAt = v.CreatedAt,
            UpdatedAt = v.UpdatedAt,
            Username = v.User.Username,
            Email = v.User.Email,
            DisplayName = v.User.DisplayName,
            PhoneNumber = v.User.PhoneNumber,
            BirthDate = v.User.BirthDate,
            Gender = v.User.Gender
        }).ToList();

        return Ok(response);
    }

    /// <summary>
    /// Gets visitors by member level.
    /// </summary>
    /// <param name="level">The member level (Bronze, Silver, Gold, Platinum).</param>
    /// <returns>List of visitors with the specified member level.</returns>
    [HttpGet("by-level/{level}")]
    public async Task<IActionResult> GetVisitorsByMemberLevel(string level)
    {
        var visitors = await _mediator.Send(new GetVisitorsByMemberLevelQuery(level));
        var response = visitors.Select(v => new VisitorResponseDto
        {
            VisitorId = v.VisitorId,
            VisitorType = v.VisitorType,
            Points = v.Points,
            MemberLevel = v.MemberLevel,
            MemberSince = v.MemberSince,
            IsBlacklisted = v.IsBlacklisted,
            Height = v.Height,
            CreatedAt = v.CreatedAt,
            UpdatedAt = v.UpdatedAt,
            Username = v.User.Username,
            Email = v.User.Email,
            DisplayName = v.User.DisplayName,
            PhoneNumber = v.User.PhoneNumber,
            BirthDate = v.User.BirthDate,
            Gender = v.User.Gender
        }).ToList();

        return Ok(response);
    }

    /// <summary>
    /// Upgrades a visitor to member status.
    /// </summary>
    /// <param name="id">The visitor ID.</param>
    /// <returns>Success message.</returns>
    [HttpPost("{id:int}/upgrade")]
    public async Task<IActionResult> UpgradeToMember(int id)
    {
        try
        {
            await _mediator.Send(new UpgradeToMemberCommand(id));
            return Ok(new { Message = "Visitor successfully upgraded to member" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    /// <summary>
    /// Adds points to a visitor's account.
    /// </summary>
    /// <param name="dto">The points addition data.</param>
    /// <returns>Points operation result.</returns>
    [HttpPost("points/add")]
    public async Task<IActionResult> AddPoints([FromBody] AddPointsDto dto)
    {
        try
        {
            var visitor = await _mediator.Send(new GetVisitorByIdQuery(dto.VisitorId));
            if (visitor == null)
                return NotFound(new { Message = $"Visitor with ID {dto.VisitorId} not found" });

            var oldLevel = visitor.MemberLevel;
            await _mediator.Send(new AddPointsCommand(dto.VisitorId, dto.Points, dto.Reason));

            // Get updated visitor to check for level changes
            var updatedVisitor = await _mediator.Send(new GetVisitorByIdQuery(dto.VisitorId));
            var levelChanged = oldLevel != updatedVisitor?.MemberLevel;

            var result = new PointsOperationResultDto
            {
                Success = true,
                Message = $"Successfully added {dto.Points} points",
                CurrentPoints = updatedVisitor?.Points ?? 0,
                CurrentMemberLevel = updatedVisitor?.MemberLevel,
                LevelChanged = levelChanged,
                NewLevel = levelChanged ? updatedVisitor?.MemberLevel : null
            };

            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    /// <summary>
    /// Deducts points from a visitor's account.
    /// </summary>
    /// <param name="dto">The points deduction data.</param>
    /// <returns>Points operation result.</returns>
    [HttpPost("points/deduct")]
    public async Task<IActionResult> DeductPoints([FromBody] DeductPointsDto dto)
    {
        try
        {
            var visitor = await _mediator.Send(new GetVisitorByIdQuery(dto.VisitorId));
            if (visitor == null)
                return NotFound(new { Message = $"Visitor with ID {dto.VisitorId} not found" });

            var oldLevel = visitor.MemberLevel;
            var success = await _mediator.Send(new DeductPointsCommand(dto.VisitorId, dto.Points, dto.Reason));

            if (!success)
            {
                return BadRequest(new PointsOperationResultDto
                {
                    Success = false,
                    Message = "Insufficient points for this operation",
                    CurrentPoints = visitor.Points,
                    CurrentMemberLevel = visitor.MemberLevel,
                    LevelChanged = false
                });
            }

            // Get updated visitor to check for level changes
            var updatedVisitor = await _mediator.Send(new GetVisitorByIdQuery(dto.VisitorId));
            var levelChanged = oldLevel != updatedVisitor?.MemberLevel;

            var result = new PointsOperationResultDto
            {
                Success = true,
                Message = $"Successfully deducted {dto.Points} points",
                CurrentPoints = updatedVisitor?.Points ?? 0,
                CurrentMemberLevel = updatedVisitor?.MemberLevel,
                LevelChanged = levelChanged,
                NewLevel = levelChanged ? updatedVisitor?.MemberLevel : null
            };

            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    /// <summary>
    /// Updates visitor information.
    /// </summary>
    /// <param name="id">The visitor ID.</param>
    /// <param name="dto">The update data.</param>
    /// <returns>Success message.</returns>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateVisitor(int id, [FromBody] UpdateVisitorDto dto)
    {
        try
        {
            await _mediator.Send(new UpdateVisitorCommand(id, null, null, null, null, null, dto.Height, null, null));
            return Ok(new { Message = "Visitor information updated successfully" });
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { Message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    /// <summary>
    /// Blacklists a visitor.
    /// </summary>
    /// <param name="id">The visitor ID.</param>
    /// <param name="dto">The blacklist data.</param>
    /// <returns>Success message.</returns>
    [HttpPost("{id:int}/blacklist")]
    public async Task<IActionResult> BlacklistVisitor(int id, [FromBody] BlacklistVisitorDto dto)
    {
        try
        {
            await _mediator.Send(new BlacklistVisitorCommand(id, dto.Reason));
            return Ok(new { Message = "Visitor successfully blacklisted" });
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { Message = ex.Message });
        }
    }

    /// <summary>
    /// Removes a visitor from blacklist.
    /// </summary>
    /// <param name="id">The visitor ID.</param>
    /// <returns>Success message.</returns>
    [HttpPost("{id:int}/unblacklist")]
    public async Task<IActionResult> RemoveFromBlacklist(int id)
    {
        try
        {
            await _mediator.Send(new UnblacklistVisitorCommand(id));
            return Ok(new { Message = "Visitor successfully removed from blacklist" });
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { Message = ex.Message });
        }
    }

    /// <summary>
    /// Gets membership statistics.
    /// </summary>
    /// <returns>Membership statistics.</returns>
    [HttpGet("statistics")]
    public async Task<IActionResult> GetMembershipStatistics()
    {
        var statistics = await _mediator.Send(new GetMembershipStatisticsQuery());
        return Ok(statistics);
    }

    /// <summary>
    /// Deletes a visitor.
    /// </summary>
    /// <param name="id">The visitor ID.</param>
    /// <returns>Success message.</returns>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteVisitor(int id)
    {
        try
        {
            await _mediator.Send(new DeleteVisitorCommand(id));
            return Ok(new { Message = "Visitor successfully deleted" });
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { Message = ex.Message });
        }
    }
}
