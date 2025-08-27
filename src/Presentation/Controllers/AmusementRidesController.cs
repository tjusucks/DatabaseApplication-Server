using DbApp.Application.ResourceSystem.AmusementRides;
using DbApp.Domain.Entities.ResourceSystem;
using DbApp.Domain.Enums.ResourceSystem;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DbApp.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AmusementRidesController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpPost]
    public async Task<ActionResult<int>> CreateAmusementRide([FromBody] CreateAmusementRideCommand command)
    {
        var rideId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetAmusementRide), new { id = rideId }, rideId);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AmusementRide>> GetAmusementRide(int id)
    {
        var ride = await _mediator.Send(new GetAmusementRideByIdQuery(id));
        return ride == null ? NotFound() : Ok(ride);
    }

    [HttpGet]
    public async Task<ActionResult<List<AmusementRide>>> GetAllAmusementRides()
    {
        var rides = await _mediator.Send(new GetAllAmusementRidesQuery());
        return Ok(rides);
    }

    [HttpGet("status/{status}")]
    public async Task<ActionResult<List<AmusementRide>>> GetAmusementRidesByStatus(RideStatus status)
    {
        var rides = await _mediator.Send(new GetAmusementRidesByStatusQuery(status));
        return Ok(rides);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateAmusementRide(int id, [FromBody] UpdateAmusementRideCommand command)
    {
        if (id != command.RideId)
            return BadRequest("ID mismatch");

        await _mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAmusementRide(int id)
    {
        await _mediator.Send(new DeleteAmusementRideCommand(id));
        return NoContent();
    }
}
