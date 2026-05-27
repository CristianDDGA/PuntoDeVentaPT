using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PuntoVenta.Application.Constants;
using PuntoVenta.Application.DTOs.Role;
using PuntoVenta.Application.Interfaces.Services;

namespace PuntoVenta.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = AppRoles.Admin)]
public class RolesController : ControllerBase
{
    private readonly IRoleService _roleService;
    private readonly IValidator<CreateRoleDto> _createRoleValidator;
    private readonly IValidator<UpdateRoleDto> _updateRoleValidator;

    public RolesController(
        IRoleService roleService,
        IValidator<CreateRoleDto> createRoleValidator,
        IValidator<UpdateRoleDto> updateRoleValidator)
    {
        _roleService = roleService;
        _createRoleValidator = createRoleValidator;
        _updateRoleValidator = updateRoleValidator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _roleService.GetAllAsync());

    [HttpGet("{roleId:int}")]
    public async Task<IActionResult> GetById(int roleId)
    {
        var role = await _roleService.GetByIdAsync(roleId);
        return role is null ? NotFound($"Role with id {roleId} not found.") : Ok(role);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRoleDto dto)
    {
        var validationResult = await _createRoleValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors.Select(error => error.ErrorMessage));

        var savedRole = await _roleService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { roleId = savedRole.RoleId }, savedRole);
    }

    [HttpPut("{roleId:int}")]
    public async Task<IActionResult> Update(int roleId, [FromBody] UpdateRoleDto dto)
    {
        var validationResult = await _updateRoleValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors.Select(error => error.ErrorMessage));

        var success = await _roleService.UpdateAsync(roleId, dto);
        return success ? NoContent() : NotFound($"Role with id {roleId} not found.");
    }

    [HttpPut("{roleId:int}/activate")]
    public async Task<IActionResult> Activate(int roleId)
        => await _roleService.ActivateAsync(roleId) ? NoContent() : NotFound($"Role with id {roleId} not found.");

    [HttpPut("{roleId:int}/deactivate")]
    public async Task<IActionResult> Deactivate(int roleId)
        => await _roleService.DeactivateAsync(roleId) ? NoContent() : NotFound($"Role with id {roleId} not found.");
}