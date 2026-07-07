using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceTemplate.Api.Models;
using ServiceTemplate.Ports.Input;

namespace ServiceTemplate.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/widgets")]
public class WidgetsController(IWidgetManager widgetManager) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateWidgetRequest request)
    {
        var result = await widgetManager.CreateAsync(request.Name);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(result.Value);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(string id)
    {
        if (!Guid.TryParse(id, out var parsedId))
            return BadRequest(new { error = $"'{id}' is not a valid widget id." });

        var result = await widgetManager.GetAsync(parsedId);

        if (!result.IsSuccess)
            return NotFound(new { error = result.Error });

        return Ok(result.Value);
    }

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int skip = 0, [FromQuery] int take = 10)
    {
        if (skip < 0)
            return BadRequest(new { error = "Skip parameter cannot be negative." });

        if (take <= 0 || take > 100)
            return BadRequest(new { error = "Take parameter must be between 1 and 100." });

        var result = await widgetManager.ListAsync(skip, take);

        if (!result.IsSuccess)
            return StatusCode(500, new { error = result.Error });

        return Ok(result.Value);
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateWidgetRequest request)
    {
        if (!Guid.TryParse(id, out var parsedId))
            return BadRequest(new { error = $"'{id}' is not a valid widget id." });

        var result = await widgetManager.UpdateAsync(parsedId, request.Name);

        if (!result.IsSuccess)
            return NotFound(new { error = result.Error });

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        if (!Guid.TryParse(id, out var parsedId))
            return BadRequest(new { error = $"'{id}' is not a valid widget id." });

        var result = await widgetManager.DeleteAsync(parsedId);

        if (!result.IsSuccess)
            return NotFound(new { error = result.Error });

        return NoContent();
    }
}
