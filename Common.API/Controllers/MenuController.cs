using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Common.Service;
using Common.Model.SysMenu;

namespace Common.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MenuController : ControllerBase
    {
        private readonly SysMenuService _service;
        private readonly ILogger<MenuController> _logger;

        public MenuController(SysMenuService service, ILogger<MenuController> logger)
        {
            _service = service;
            _logger = logger;
        }

        /// <summary>
        /// Lấy tất cả menu đang Active
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                var result = _service.GetAll();
                if (result.IsSuccess)
                    return Ok(result);
                else
                    return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all menus");
                return StatusCode(500, new { message = "Lỗi hệ thống", error = ex.Message });
            }
        }

        /// <summary>
        /// Lấy tất cả menu (bao gồm cả Inactive)
        /// </summary>
        /// <returns></returns>
        [HttpGet("full")]
        public IActionResult GetFull()
        {
            try
            {
                var result = _service.GetFull();
                if (result.IsSuccess)
                    return Ok(result);
                else
                    return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting full menus");
                return StatusCode(500, new { message = "Lỗi hệ thống", error = ex.Message });
            }
        }

        /// <summary>
        /// Lấy menu theo Name
        /// </summary>
        /// <param name="name">Tên menu (Primary Key)</param>
        /// <returns></returns>
        [HttpGet("{name}")]
        public IActionResult GetById(string name)
        {
            try
            {
                var result = _service.GetById(name);
                if (result.IsSuccess)
                    return Ok(result);
                else
                    return NotFound(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting menu by name: {Name}", name);
                return StatusCode(500, new { message = "Lỗi hệ thống", error = ex.Message });
            }
        }

        /// <summary>
        /// Lấy menu con theo ParentMenu
        /// </summary>
        /// <param name="parentName">Tên menu cha</param>
        /// <returns></returns>
        [HttpGet("parent/{parentName}")]
        public IActionResult GetByParent(string? parentName)
        {
            try
            {
                var result = _service.GetByParent(parentName);
                if (result.IsSuccess)
                    return Ok(result);
                else
                    return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting menus by parent: {ParentName}", parentName);
                return StatusCode(500, new { message = "Lỗi hệ thống", error = ex.Message });
            }
        }

        /// <summary>
        /// Lấy menu cấp 1 (root menus)
        /// </summary>
        /// <returns></returns>
        [HttpGet("root")]
        public IActionResult GetRootMenus()
        {
            try
            {
                var result = _service.GetRootMenus();
                if (result.IsSuccess)
                    return Ok(result);
                else
                    return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting root menus");
                return StatusCode(500, new { message = "Lỗi hệ thống", error = ex.Message });
            }
        }

        /// <summary>
        /// Lấy cây menu (tree structure)
        /// </summary>
        /// <returns></returns>
        [HttpGet("tree")]
        public IActionResult GetMenuTree()
        {
            try
            {
                var result = _service.GetMenuTree();
                if (result.IsSuccess)
                    return Ok(result);
                else
                    return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting menu tree");
                return StatusCode(500, new { message = "Lỗi hệ thống", error = ex.Message });
            }
        }

        /// <summary>
        /// Tạo menu mới
        /// </summary>
        /// <param name="model">Thông tin menu</param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Create([FromBody] SysMenuCreateModel model)
        {
            try
            {
                var result = _service.Create(model);
                if (result.IsSuccess)
                    return StatusCode(201, result);
                else
                    return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating menu");
                return StatusCode(500, new { message = "Lỗi hệ thống", error = ex.Message });
            }
        }

        /// <summary>
        /// Cập nhật menu
        /// </summary>
        /// <param name="name">Tên menu</param>
        /// <param name="model">Thông tin cập nhật</param>
        /// <returns></returns>
        [HttpPut("{name}")]
        public IActionResult Update(string name, [FromBody] SysMenuUpdateModel model)
        {
            try
            {
                if (name != model.Name)
                    return BadRequest(new { message = "Tên menu không khớp" });

                var result = _service.Update(model);
                if (result.IsSuccess)
                    return Ok(result);
                else
                    return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating menu: {Name}", name);
                return StatusCode(500, new { message = "Lỗi hệ thống", error = ex.Message });
            }
        }

        /// <summary>
        /// Xóa menu (Soft delete - Set IsActive = 0)
        /// </summary>
        /// <param name="name">Tên menu</param>
        /// <returns></returns>
        [HttpDelete("{name}")]
        public IActionResult Delete(string name)
        {
            try
            {
                var result = _service.Delete(name);
                if (result.IsSuccess)
                    return Ok(result);
                else
                    return NotFound(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting menu: {Name}", name);
                return StatusCode(500, new { message = "Lỗi hệ thống", error = ex.Message });
            }
        }
    }
}

