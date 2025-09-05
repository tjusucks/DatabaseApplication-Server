using DbApp.Application.UserSystem.Employees;
using DbApp.Domain.Enums.UserSystem;
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
        // 检查模型验证
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

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
        // 检查模型验证
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // 创建一个新的命令对象，其中EmployeeId来自URL参数
        var updateCommand = new UpdateEmployeeCommand(
            id, // 使用URL中的id作为EmployeeId
            command.StaffNumber,
            command.Position,
            command.DepartmentName,
            command.TeamId,
            command.ManagerId,
            command.Certification,
            command.ResponsibilityArea);

        await _mediator.Send(updateCommand);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _mediator.Send(new DeleteEmployeeCommand(id));
        return NoContent();
    }

    // 统一的查询端点，通过查询参数区分不同查询条件
    [HttpGet]
    public async Task<IActionResult> GetEmployees(
        [FromQuery] string? keyword = null,
        [FromQuery] string? departmentName = null,
        [FromQuery] StaffType? staffType = null)
    {
        // 根据提供的查询参数决定使用哪种查询
        if (!string.IsNullOrEmpty(keyword))
        {
            var employees = await _mediator.Send(new SearchEmployeesQuery(keyword));
            return Ok(employees);
        }

        if (!string.IsNullOrEmpty(departmentName))
        {
            var employees = await _mediator.Send(new GetEmployeesByDepartmentQuery(departmentName));
            return Ok(employees);
        }

        if (staffType.HasValue)
        {
            var employees = await _mediator.Send(new GetEmployeesByStaffTypeQuery(staffType.Value));
            return Ok(employees);
        }

        // 默认情况：获取所有员工
        var allEmployees = await _mediator.Send(new GetAllEmployeesQuery());
        return Ok(allEmployees);
    }
}
