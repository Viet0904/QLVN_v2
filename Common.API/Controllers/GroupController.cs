using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Common.Service;
using Common.Library.Constant;
using Common.Model.UsGroup;
using Common.Model.UsUser;
using Common.Model.UsUserPermission;
using Common.Model.Common;
using Common.Library.Helper;
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
    // Lấy tất cả Group đang Active 
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
           
            var result = await _groupService.GetAll();
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = MessageConstant.NOT_EXIST, error = ex.Message });

        }
    }
    // Lấy tất cả Group cả Active và không Active
    [HttpGet("full")]
    public async Task<IActionResult> GetFull()
    {
        try
        {
            var result = await _groupService.GetFull();
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = MessageConstant.NOT_EXIST, error = ex.Message });

        }
    }
    // Lấy Group theo ID
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        try
        {
            var result = await _groupService.GetById(id);
            if (result == null)
            {
                return NotFound(new { Message = MessageConstant.NOT_EXIST });
            }
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = MessageConstant.NOT_EXIST, error = ex.Message });
        }
    }
    // Tạo Group
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] UsGroupCreateModel model)
    {
        try
        {
            var result = await _groupService.Create(model);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = MessageConstant.NOT_EXIST, error = ex.Message });
        }
    }

    // Update Group
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UsGroupUpdateModel model)
    {
        try
        {
            var result = await _groupService.Update(model);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = MessageConstant.NOT_EXIST, error = ex.Message });
        }
    }

    // Delete Group
    [HttpDelete]
    public async Task<IActionResult> Delete(string id)
    {
        try
        {
            var result = await _groupService.Delete(id);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = MessageConstant.NOT_EXIST, error = ex.Message });
        }
    }

    [HttpGet("paginated")]
    public async Task<IActionResult> GetPaginated([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? searchTerm = null, [FromQuery] string? sortColumn = null, [FromQuery] string? sortDirection = "asc")
    {
        try
        {
            // Handle DataTables parameters if present
            if (Request.Query.ContainsKey("start") && Request.Query.ContainsKey("length"))
            {
                int start = int.Parse(Request.Query["start"]!);
                int length = int.Parse(Request.Query["length"]!);
                pageNumber = (start / length) + 1;
                pageSize = length;
            }

            if (Request.Query.ContainsKey("search[value]"))
            {
                searchTerm = Request.Query["search[value]"];
            }

            var request = new PaginatedRequest
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                SearchTerm = searchTerm,
                SortColumn = sortColumn,
                SortDirection = sortDirection
            };
            var result = await _groupService.GetPaginated(request);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = MessageConstant.NOT_EXIST, error = ex.Message });
        }
    }

    [HttpGet("{id}/users")]
    public async Task<IActionResult> GetUsers(string id)
    {
        try
        {
            var result = await _groupService.GetUsersByGroupId(id);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = MessageConstant.NOT_EXIST, error = ex.Message });
        }
    }

    [HttpGet("{id}/permissions")]
    public async Task<IActionResult> GetPermissions(string id)
    {
        try
        {
            var result = await _groupService.GetPermissionMatrix(id);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = MessageConstant.NOT_EXIST, error = ex.Message });
        }
    }

    [HttpPost("permissions")]
    public async Task<IActionResult> UpdatePermissions([FromBody] IEnumerable<UsUserPermissionViewModel> models)
    {
        try
        {
            var result = await _groupService.UpdatePermissions(models);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = MessageConstant.NOT_EXIST, error = ex.Message });
        }
    }
}