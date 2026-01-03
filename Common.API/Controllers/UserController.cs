using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Common.Service;
using Common.Model.Common;
using Common.Model.UsUser;
using Common.Library.Constant;

namespace Common.API.Controllers
{
    [Route("api/[controller]")] // Chỉ định route cho controller
    [ApiController] // tự động check Model State, [FromQuery], [FromBody], [FromForm], [FromRoute], [FromHeader], [FromServices].
    [Authorize] // Chỉ cho phép người dùng đã đăng nhập truy cập
    public class UserController : ControllerBase
    {
        private readonly UsUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(UsUserService uservice, ILogger<UserController> logger)
        {
            _userService = uservice;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                return Ok(await _userService.GetAll());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all users");
                return StatusCode(500, new { message = MessageConstant.DATA_NOT_FOUND, error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                var user = await _userService.GetById(id);
                if (user == null)
                    return NotFound(new { message = MessageConstant.NOT_EXIST });
                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by id: {Id}", id);
                return StatusCode(500, new { message = "Lỗi khi lấy thông tin người dùng", error = ex.Message });
            }
        }

        [HttpGet("paginated")]
        public async Task<IActionResult> GetPaginated([FromQuery] PaginatedRequest request)
        {
            try
            {
                _logger.LogInformation("GetPaginated called with PageNumber={PageNumber}, PageSize={PageSize}",
                    request.PageNumber, request.PageSize);

                var result = await _userService.GetPaginated(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting paginated users");
                return StatusCode(500, new { message = "Lỗi khi lấy danh sách người dùng phân trang", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UsUserCreateModel request)
        {
            try
            {
                var newUser = await _userService.Create(request);
                return StatusCode(201, newUser);
            }
            catch (InvalidOperationException ex)
            {
                // Trả về BadRequest với message cụ thể
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user");
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi tạo người dùng", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UsUserUpdateModel request)
        {
            try
            {
                request.Id = id;
                var updatedUser = await _userService.Update(request);
                return Ok(updatedUser);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user: {Id}", id);
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi cập nhật người dùng", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                await _userService.Delete(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user: {Id}", id);
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi xóa người dùng", error = ex.Message });
            }
        }
    }
}