using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PuntoVenta.Application.Constants;
using PuntoVenta.Application.Interfaces.Services;

namespace PuntoVenta.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = AppRoles.Admin)]
public class ErrorLogsController : ControllerBase
{
    private readonly IErrorLogService _errorLogService;

    public ErrorLogsController(IErrorLogService errorLogService)
    {
        _errorLogService = errorLogService;
    }

    [HttpGet("paged")]
    public async Task<IActionResult> GetPaged(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 15)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 15;

        var pagedResult = await _errorLogService.GetPagedAsync(page, pageSize);
        return Ok(pagedResult);
    }
}
