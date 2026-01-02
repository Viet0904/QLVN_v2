using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Common.Service;
using Common.Library.Constant;
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
           
            var result = await _groupService.GetAllAsync();
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { MessageConstant.NOT_EXIST, error = ex.Message });

        }
    }
    [HttpGet]
    public async Task<IActionResult> GetById(string id)
    {
        try
        {
            var result = _groupService.GetById(id);
            if (!result.IsSuccess)
            {
                return NotFound(result);
            }
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { MessageConstant.NOT_EXIST, error = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Common.Model.UsGroup.UsGroupViewModel model)
    {
        try
        {
            var result = _groupService.Create(model);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { MessageConstant.NOT_EXIST, error = ex.Message });
        }
    }

}