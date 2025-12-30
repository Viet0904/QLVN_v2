using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Common.Service;

namespace Common.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class GroupController : ControllerBase
{
    private readonly UsGroupService _groupService;
    private readonly ILogger<GroupController> _logger;

    public GroupController(UsGroupService groupService, ILogger<GroupController> logger)
    {
        _groupService = groupService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            _logger.LogInformation("GetAll groups called");
            var result = await _groupService.GetAllAsync();
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all groups");
            return StatusCode(500, new { message = "Lỗi khi lấy danh sách nhóm", error = ex.Message });
        }
    }
}