using DbApp.Application.UserSystem.Users;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DbApp.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpPost]
    public async Task<IActionResult> Create(CreateUserCommand command)
    {
        var newUserId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = newUserId }, null);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var user = await _mediator.Send(new GetUserByIdQuery(id));
        if (user == null)
            return NotFound();

        return Ok(user);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateUserCommand command)
    {
        if (id != command.UserId)
            return BadRequest("ID mismatch");

        await _mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _mediator.Send(new DeleteUserCommand(id));
        return NoContent();
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await _mediator.Send(new GetAllUsersQuery());
        return Ok(users);
    }
}
