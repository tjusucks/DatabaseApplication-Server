using DbApp.Application.ResourceSystem.RideEntryRecords;  
using DbApp.Domain.Entities.ResourceSystem;  
using MediatR;  
using Microsoft.AspNetCore.Mvc;  
  
namespace DbApp.Presentation.Controllers;  
  
[ApiController]  
[Route("api/[controller]")]  
public class RideEntryRecordsController(IMediator mediator) : ControllerBase  
{  
    private readonly IMediator _mediator = mediator;  
  
    /// <summary>   
    /// </summary>  
    [HttpPost]  
    public async Task<ActionResult<RideEntryRecord>> CreateRideEntryRecord([FromBody] CreateRideEntryRecordCommand command)  
    {  
        var result = await _mediator.Send(command);  
        return CreatedAtAction(nameof(GetRideEntryRecord), new { id = result.RideEntryRecordId }, result);  
    }  
  
    /// <summary>  
    /// 根据ID获取单个进出记录  
    /// </summary>  
    [HttpGet("{id}")]  
    public async Task<ActionResult<RideEntryRecord>> GetRideEntryRecord(int id)  
    {  
        var record = await _mediator.Send(new GetRideEntryRecordByIdQuery(id));  
        return record == null ? NotFound() : Ok(record);  
    }  
  
    /// <summary>  
    /// 获取所有进出记录
    /// </summary>  
    [HttpGet]  
    public async Task<ActionResult<List<RideEntryRecord>>> GetRideEntryRecords(  
        [FromQuery] int? rideId = null,  
        [FromQuery] int? visitorId = null,  
        [FromQuery] DateTime? startDate = null,  
        [FromQuery] DateTime? endDate = null,  
        [FromQuery] int page = 1,  
        [FromQuery] int pageSize = 50)  
    {  
        var query = new GetRideEntryRecordsQuery(rideId, visitorId, startDate, endDate, page, pageSize);  
        var records = await _mediator.Send(query);  
        return Ok(records);  
    }  
  
    /// <summary>  
    /// 根据游乐设施ID获取进出记录  
    /// </summary>  
    [HttpGet("ride/{rideId}")]  
    public async Task<ActionResult<List<RideEntryRecord>>> GetRideEntryRecordsByRide(int rideId)  
    {  
        var records = await _mediator.Send(new GetRideEntryRecordsByRideQuery(rideId));  
        return Ok(records);  
    }  
  
    /// <summary>  
    /// 根据游客ID获取进出记录  
    /// </summary>  
    [HttpGet("visitor/{visitorId}")]  
    public async Task<ActionResult<List<RideEntryRecord>>> GetRideEntryRecordsByVisitor(int visitorId)  
    {  
        var records = await _mediator.Send(new GetRideEntryRecordsByVisitorQuery(visitorId));  
        return Ok(records);  
    }  
  
    /// <summary>  
    /// 更新进出记录 
    /// </summary>  
    [HttpPut("{id}")]  
    public async Task<ActionResult> UpdateRideEntryRecord(int id, [FromBody] UpdateRideEntryRecordCommand command)  
    {  
        if (id != command.RideEntryRecordId)  
            return BadRequest("ID mismatch");  
  
        await _mediator.Send(command);  
        return NoContent();  
    }  
  
    /// <summary>  
    /// 设置游客退出时间  
    /// </summary>  
    [HttpPatch("{id}/exit")]  
    public async Task<ActionResult> SetExitTime(int id, [FromBody] SetExitTimeCommand command)  
    {  
        if (id != command.RideEntryRecordId)  
            return BadRequest("ID mismatch");  
  
        await _mediator.Send(command);  
        return NoContent();  
    }  
  
    /// <summary>  
    /// 删除进出记录  
    /// </summary>  
    [HttpDelete("{id}")]  
    public async Task<ActionResult> DeleteRideEntryRecord(int id)  
    {  
        await _mediator.Send(new DeleteRideEntryRecordCommand(id));  
        return NoContent();  
    }  
  
    /// <summary>  
    /// 获取当前在园游客统计  
    /// </summary>  
    [HttpGet("current-visitors")]  
    public async Task<ActionResult<List<RideEntryRecord>>> GetCurrentVisitorsInRides()  
    {  
        var records = await _mediator.Send(new GetCurrentVisitorsInRidesQuery());  
        return Ok(records);  
    }  
  
    /// <summary>  
    /// 获取指定时间段内的游客流量统计  
    /// </summary>  
    [HttpGet("traffic-summary")]  
    public async Task<ActionResult> GetTrafficSummary(  
        [FromQuery] DateTime startDate,  
        [FromQuery] DateTime endDate,  
        [FromQuery] int? rideId = null)  
    {  
        var summary = await _mediator.Send(new GetTrafficSummaryQuery(startDate, endDate, rideId));  
        return Ok(summary);  
    }  
}