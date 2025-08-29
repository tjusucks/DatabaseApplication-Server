using DbApp.Application.UserSystem.Employees;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DbApp.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpPost]
    public async Task<IActionResult> Create(CreateEmployeeCommand command)
    {
        var newEmployeeId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = newEmployeeId }, null);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var employee = await _mediator.Send(new GetEmployeeByIdQuery(id));
        if (employee == null)
            return NotFound();

        return Ok(employee);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateEmployeeCommand command)
    {
        if (id != command.EmployeeId)
            return BadRequest("ID mismatch");

        await _mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _mediator.Send(new DeleteEmployeeCommand(id));
        return NoContent();
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var employees = await _mediator.Send(new GetAllEmployeesQuery());
        return Ok(employees);
    }
}
