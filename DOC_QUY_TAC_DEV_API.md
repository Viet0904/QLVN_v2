# QUY TẮC PHÁT TRIỂN - DỰ ÁN QLVN (Quản Lý Vùng Nuôi)

## 📋 MỤC LỤC
1. [Quy tắc đặt tên](#1-quy-tắc-đặt-tên)
2. [Cấu trúc Entity & Model](#2-cấu-trúc-entity--model)
3. [Quy tắc Service Layer](#3-quy-tắc-service-layer)
4. [Quy tắc API Controller](#4-quy-tắc-api-controller)
5. [Quy tắc xử lý Database](#5-quy-tắc-xử-lý-database)
6. [Quy tắc xử lý lỗi](#6-quy-tắc-xử-lý-lỗi)
7. [Quy tắc bảo mật](#7-quy-tắc-bảo-mật)
8. [Checklist trước khi commit](#8-checklist-trước-khi-commit)

---

## 1. QUY TẮC ĐẶT TÊN

### 1.1. Prefix cho các Entity/Table
Tất cả Entity/Table phải có **PREFIX** rõ ràng:

| Prefix | Ý nghĩa | Ví dụ |
|--------|---------|-------|
| **Us** | User System - Hệ thống người dùng | `UsUser`, `UsGroup`, `UsUserLog` |
| **Db** | Database - Dữ liệu nghiệp vụ | `DbAoNuoi`, `DbDvsd`, `DbHoaChat` |
| **Sys** | System - Cấu hình hệ thống | `SysSetting`, `SysMenu`, `SysSystemInfo` |

**❌ SAI:**
```csharp
public class User { }        // Thiếu prefix
public class AoNuoi { }      // Thiếu prefix
```

**✅ ĐÚNG:**
```csharp
public class UsUser { }      // Có prefix Us
public class DbAoNuoi { }    // Có prefix Db
```

### 1.2. Quy tắc tên file

#### Entity (trong `Common.Database/Entities/`)
- Format: `{Prefix}{EntityName}.cs`
- Ví dụ: `UsUser.cs`, `DbDvsd.cs`, `SysIdGenerated.cs`

#### Model (trong `Common.Model/{EntityName}/`)
- Tạo folder riêng cho mỗi Entity
- 3 file model chuẩn:
  - `{EntityName}CreateModel.cs` - Tạo mới
  - `{EntityName}UpdateModel.cs` - Cập nhật
  - `{EntityName}ViewModel.cs` - Hiển thị

**Ví dụ cấu trúc folder Model:**
```
Common.Model/
├── UsUser/
│   ├── UsUserCreateModel.cs
│   ├── UsUserUpdateModel.cs
│   └── UsUserViewModel.cs
├── DbDvsd/
│   ├── DbDvsdCreateModel.cs
│   ├── DbDvsdUpdateModel.cs
│   └── DbDvsdViewModel.cs
```

#### Service (trong `Common.Service/`)
- Format: `{EntityName}Service.cs`
- Ví dụ: `UsUserService.cs`, `DvsdService.cs`

#### Controller (trong `Common.API/Controllers/`)
- Format: `{EntityName}Controller.cs`
- Ví dụ: `UserController.cs`, `DvsdController.cs`

---

## 2. CẤU TRÚC ENTITY & MODEL

### 2.1. Entity - PHẢI CÓ các trường chuẩn

**Tất cả Entity PHẢI CÓ các trường sau:**

```csharp
public partial class DbDvsd
{
    // Primary Key
    public string Ma { get; set; } = null!;
    
    // Business Fields
    public string Ten { get; set; } = null!;
    
    // AUDIT FIELDS - BẮT BUỘC
    public int RowStatus { get; set; }           // Trạng thái bản ghi (1: Active, 2: Deleted)
    public DateTime CreatedAt { get; set; }      // Thời gian tạo
    public string CreatedBy { get; set; } = null!;   // Người tạo
    public DateTime UpdatedAt { get; set; }      // Thời gian cập nhật
    public string UpdatedBy { get; set; } = null!;   // Người cập nhật
    
    // Navigation Properties
    public virtual UsUser CreatedByNavigation { get; set; } = null!;
    public virtual UsUser UpdatedByNavigation { get; set; } = null!;
}
```

### 2.2. CreateModel - Không có Id, không có Audit fields

```csharp
public class DbDvsdCreateModel
{
    public string Ten { get; set; } = null!;
    public string? DiaChi { get; set; }
    public string? Phone { get; set; }
    public string? Note { get; set; }
    
    // ❌ KHÔNG CÓ: Id, RowStatus, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy
}
```

### 2.3. UpdateModel - Có Id, không có Audit fields

```csharp
public class DbDvsdUpdateModel
{
    public string Id { get; set; } = null!;  // ✅ CÓ Id
    public string Ten { get; set; } = null!;
    public string? DiaChi { get; set; }
    public string? Phone { get; set; }
    public string? Note { get; set; }
    
    // ❌ KHÔNG CÓ: RowStatus, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy
}
```

### 2.4. ViewModel - Kế thừa BaseViewModel

```csharp
public class DbDvsdViewModel : BaseViewModel
{
    public string Ma { get; set; } = null!;
    public string Ten { get; set; } = null!;
    public string? DiaChi { get; set; }
    public string? Phone { get; set; }
    public string? Note { get; set; }
    
    // BaseViewModel tự động cung cấp:
    // RowStatus, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy
    // + CreatedName, UpdatedName (nếu có JOIN)
}
```

---

## 3. QUY TẮC SERVICE LAYER

### 3.1. Service phải kế thừa BaseService

```csharp
public class DbDvsdService : BaseService
{
    // BaseService cung cấp:
    // - DbContext: Truy cập database
    // - Mapper: AutoMapper
    // - GenerateId(): Tạo ID tự động
}
```

### 3.2. Các phương thức CHUẨN trong Service

**File mẫu chuẩn: `UsUserService.cs`**

#### ✅ 1. GetAll() - Lấy danh sách Active

```csharp
public ResModel<List<DbDvsdViewModel>> GetAll()
{
    ResModel<List<DbDvsdViewModel>> res = new ResModel<List<DbDvsdViewModel>>();
    
    var results = DbContext.DbDvsds
        .Where(x => x.RowStatus == RowStatusConstant.Active)
        .ToList();
        
    res.Data = Mapper.Map<List<DbDvsdViewModel>>(results);
    
    return res;
}
```

#### ✅ 2. GetFull() - Lấy tất cả (bao gồm Deleted)

```csharp
public ResModel<List<DbDvsdViewModel>> GetFull()
{
    ResModel<List<DbDvsdViewModel>> res = new ResModel<List<DbDvsdViewModel>>();
    
    var results = DbContext.DbDvsds.ToList();
    res.Data = Mapper.Map<List<DbDvsdViewModel>>(results);
    
    return res;
}
```

#### ✅ 3. GetById(string id) - Lấy theo Id

```csharp
public ResModel<DbDvsdViewModel> GetById(string id)
{
    ResModel<DbDvsdViewModel> res = new ResModel<DbDvsdViewModel>();
    
    var result = DbContext.DbDvsds
        .Where(x => x.Ma == id && x.RowStatus == RowStatusConstant.Active)
        .FirstOrDefault();
        
    if (result != null) 
        res.Data = Mapper.Map<DbDvsdViewModel>(result);
    else
        res.ErrorMessage = MessageConstant.NOT_EXIST;
    
    return res;
}
```

#### ✅ 4. Create(CreateModel model) - Tạo mới

**⚠️ QUAN TRỌNG: Xử lý Transaction + GenerateId**

```csharp
public ResModel<DbDvsdViewModel> Create(DbDvsdCreateModel model)
{
    ResModel<DbDvsdViewModel> res = new ResModel<DbDvsdViewModel>();
    
    // 1. Kiểm tra trùng lặp (nếu cần)
    var existingItem = DbContext.DbDvsds
        .Where(x => x.Ten == model.Ten && x.RowStatus == RowStatusConstant.Active)
        .FirstOrDefault();
        
    if (existingItem != null)
    {
        res.ErrorMessage = "Tên đã tồn tại.";
        return res;
    }
    
    try
    {
        // 2. Mở Transaction
        UnitOfWork.Ins.TransactionOpen();
        
        // 3. Map model sang Entity
        var item = Mapper.Map<DbDvsd>(model);
        
        // 4. Generate Id với retry nếu bị trùng
        generateId:
        item.Ma = GenerateId(DefaultCodeConstant.DbDvsd.Name, DefaultCodeConstant.DbDvsd.Length);
        
        var resultIdExist = DbContext.DbDvsds.Where(x => x.Ma == item.Ma).FirstOrDefault();
        if (resultIdExist != null)
            goto generateId;
        
        // 5. Thêm vào DB và Save
        DbContext.DbDvsds.Add(item);
        DbContext.SaveChanges();
        
        // 6. Commit Transaction
        UnitOfWork.Ins.TransactionCommit();
        UnitOfWork.Ins.RenewDB();
        
        // 7. Lấy lại dữ liệu mới tạo
        res = GetById(item.Ma);
    }
    catch (Exception)
    {
        UnitOfWork.Ins.TransactionRollback();
        throw;
    }
    
    return res;
}
```

#### ✅ 5. Update(UpdateModel model) - Cập nhật

```csharp
public ResModel<bool> Update(DbDvsdUpdateModel model)
{
    ResModel<bool> res = new ResModel<bool>();
    
    try
    {
        var result = DbContext.DbDvsds.Where(x => x.Ma == model.Id).FirstOrDefault();
        if (result != null)
        {
            // Map chỉ những field được phép update
            Mapper.Map(model, result);
            DbContext.SaveChanges();
            res.Data = true;
        }
        else
        {
            res.ErrorMessage = MessageConstant.NOT_EXIST;
        }
    }
    catch (Exception e)
    {
        res.ErrorMessage = e.Message;
        ExceptionHelper.HandleException(e);
    }
    
    return res;
}
```

#### ✅ 6. Delete(string id) - Xóa mềm (Soft Delete)

**⚠️ QUAN TRỌNG: Không được xóa vật lý, chỉ xóa mềm bằng RowStatus**

```csharp
public ResModel<bool> Delete(string id)
{
    ResModel<bool> res = new ResModel<bool>();
    
    var result = DbContext.DbDvsds.Where(x => x.Ma == id).FirstOrDefault();
    if (result != null)
    {
        result.RowStatus = RowStatusConstant.Deleted;  // Xóa mềm
        DbContext.SaveChanges();
        res.Data = true;
    }
    else
    {
        res.ErrorMessage = MessageConstant.NOT_EXIST;
    }
    
    return res;
}
```

### 3.3. GenerateId - Tạo Id tự động

**Phải đăng ký trong `DefaultCodeConstant.cs`:**

```csharp
public class DefaultCodeConstant
{
    public struct DbDvsd
    {
        public const string Name = "DbDvsd";    // Tên table
        public const int Length = 5;             // Độ dài mã (00001)
    }
}
```

**Cách sử dụng:**
```csharp
item.Ma = GenerateId(DefaultCodeConstant.DbDvsd.Name, DefaultCodeConstant.DbDvsd.Length);
// Kết quả: "00001", "00002", "00003"...
```

**Đối với UsUser (có GroupId):**
```csharp
item.Id = item.GroupId + GenerateId(DefaultCodeConstant.UsUser.Name, DefaultCodeConstant.UsUser.Length);
// Kết quả: "ADM00001", "ADM00002" (ADM là GroupId)
```

---

## 4. QUY TẮC API CONTROLLER

### 4.1. Cấu trúc Controller chuẩn

```csharp
[Route("api/[controller]")]
[ApiController]
[Authorize]  // ⚠️ BẮT BUỘC: Tất cả API phải xác thực (trừ Login)
public class DvsdController : ControllerBase
{
    private readonly DbDvsdService _service;
    private readonly ILogger<DvsdController> _logger;

    public DvsdController(DbDvsdService service, ILogger<DvsdController> logger)
    {
        _service = service;
        _logger = logger;
    }
    
    // ... các endpoint
}
```

### 4.2. Các endpoint CHUẨN

#### ✅ GET /api/dvsd - Lấy tất cả

```csharp
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
        _logger.LogError(ex, "Error getting all items");
        return StatusCode(500, new { message = "Lỗi hệ thống", error = ex.Message });
    }
}
```

#### ✅ GET /api/dvsd/{id} - Lấy theo Id

```csharp
[HttpGet("{id}")]
public IActionResult GetById(string id)
{
    try
    {
        var result = _service.GetById(id);
        if (result.IsSuccess)
            return Ok(result);
        else
            return NotFound(result);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error getting item by id: {Id}", id);
        return StatusCode(500, new { message = "Lỗi hệ thống", error = ex.Message });
    }
}
```

#### ✅ POST /api/dvsd - Tạo mới

```csharp
[HttpPost]
public IActionResult Create([FromBody] DbDvsdCreateModel model)
{
    try
    {
        var result = _service.Create(model);
        if (result.IsSuccess)
            return StatusCode(201, result);  // 201 Created
        else
            return BadRequest(result);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error creating item");
        return StatusCode(500, new { message = "Lỗi hệ thống", error = ex.Message });
    }
}
```

#### ✅ PUT /api/dvsd/{id} - Cập nhật

```csharp
[HttpPut("{id}")]
public IActionResult Update(string id, [FromBody] DbDvsdUpdateModel model)
{
    try
    {
        if (id != model.Id)
            return BadRequest(new { message = "Id không khớp" });
            
        var result = _service.Update(model);
        if (result.IsSuccess)
            return Ok(result);
        else
            return BadRequest(result);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error updating item: {Id}", id);
        return StatusCode(500, new { message = "Lỗi hệ thống", error = ex.Message });
    }
}
```

#### ✅ DELETE /api/dvsd/{id} - Xóa mềm

```csharp
[HttpDelete("{id}")]
public IActionResult Delete(string id)
{
    try
    {
        var result = _service.Delete(id);
        if (result.IsSuccess)
            return Ok(result);
        else
            return NotFound(result);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error deleting item: {Id}", id);
        return StatusCode(500, new { message = "Lỗi hệ thống", error = ex.Message });
    }
}
```

### 4.3. Đăng ký Service trong Program.cs

**⚠️ BẮT BUỘC: Phải đăng ký Service trước khi sử dụng**

```csharp
// Program.cs
builder.Services.AddScoped<DbDvsdService>();
```

---

## 5. QUY TẮC XỬ LÝ DATABASE

### 5.1. Sử dụng Transaction cho Create

**❌ SAI:**
```csharp
public ResModel<DbDvsdViewModel> Create(DbDvsdCreateModel model)
{
    var item = Mapper.Map<DbDvsd>(model);
    DbContext.DbDvsds.Add(item);
    DbContext.SaveChanges();  // Không có Transaction
}
```

**✅ ĐÚNG:**
```csharp
public ResModel<DbDvsdViewModel> Create(DbDvsdCreateModel model)
{
    try
    {
        UnitOfWork.Ins.TransactionOpen();      // Mở Transaction
        
        var item = Mapper.Map<DbDvsd>(model);
        DbContext.DbDvsds.Add(item);
        DbContext.SaveChanges();
        
        UnitOfWork.Ins.TransactionCommit();    // Commit
        UnitOfWork.Ins.RenewDB();              // Renew Context
    }
    catch (Exception)
    {
        UnitOfWork.Ins.TransactionRollback();  // Rollback nếu lỗi
        throw;
    }
}
```

### 5.2. Xóa mềm (Soft Delete) - KHÔNG xóa vật lý

**❌ SAI:**
```csharp
DbContext.DbDvsds.Remove(item);  // XÓA VẬT LÝ - KHÔNG ĐƯỢC PHÉP
```

**✅ ĐÚNG:**
```csharp
item.RowStatus = RowStatusConstant.Deleted;  // XÓA MỀM
DbContext.SaveChanges();
```

### 5.3. Audit Fields tự động cập nhật

**BaseEntity.cs** tự động thêm audit fields khi SaveChanges():
- **Khi Add:** Tự động set `RowStatus=1`, `CreatedAt`, `CreatedBy`, `UpdatedAt`, `UpdatedBy`
- **Khi Update:** Tự động update `UpdatedAt`, `UpdatedBy`

**⚠️ CreatedBy/UpdatedBy lấy từ `SessionHelper.UserId`**

---

## 6. QUY TẮC XỬ LÝ LỖI

### 6.1. ResModel - Response chuẩn

```csharp
public class ResModel<T>
{
    public bool IsSuccess { get; set; }     // Tự động = false nếu có ErrorMessage
    public T Data { get; set; }             // Dữ liệu trả về
    public string ErrorMessage { get; set; } // Thông báo lỗi
    public string Message { get; set; }      // Thông báo thành công
}
```

**Ví dụ sử dụng:**

```csharp
// Thành công
var res = new ResModel<DbDvsdViewModel>();
res.Data = viewModel;
res.Message = "Tạo thành công";
return res;

// Lỗi
var res = new ResModel<DbDvsdViewModel>();
res.ErrorMessage = "Tên đã tồn tại";
return res;
```

### 6.2. HTTP Status Code chuẩn

| Code | Ý nghĩa | Khi nào dùng |
|------|---------|--------------|
| 200 OK | Thành công | GET, PUT, DELETE thành công |
| 201 Created | Tạo mới thành công | POST thành công |
| 400 Bad Request | Dữ liệu không hợp lệ | Validation lỗi, dữ liệu trùng |
| 404 Not Found | Không tìm thấy | GET/PUT/DELETE với Id không tồn tại |
| 500 Internal Server Error | Lỗi hệ thống | Exception không xử lý được |

### 6.3. Message Constants

**Sử dụng MessageConstant.cs cho thông báo chuẩn:**

```csharp
public class MessageConstant
{
    public static string NOT_EXIST = "Không tồn tại.";
    public static string EXIST = "Đã tồn tại.";
    public static string CONTROL_REQUIRE = "Không được bỏ trống.";
    public static string USERNAME_PASSWORD_NOT_CORRECT = "Tên đăng nhập hoặc mật khẩu không đúng!";
}
```

---

## 7. QUY TẮC BẢO MẬT

### 7.1. Mã hóa mật khẩu

**⚠️ BẮT BUỘC: Dùng PasswordHelper để mã hóa**

```csharp
// Khi tạo user
item.Password = PasswordHelper.CreatePassword(model.Password);

// Khi login
password = PasswordHelper.CreatePassword(password);
var user = DbContext.UsUsers
    .Where(x => x.UserName == userName && x.Password == password)
    .FirstOrDefault();
```

### 7.2. JWT Authentication

**Tất cả API phải có [Authorize]:**

```csharp
[Route("api/[controller]")]
[ApiController]
[Authorize]  // ⚠️ BẮT BUỘC
public class DvsdController : ControllerBase
{
    // ...
}
```

**Chỉ Login API được phép [AllowAnonymous]:**

```csharp
[HttpPost("login")]
[AllowAnonymous]
public IActionResult Login([FromBody] LoginRequest request)
{
    // ...
}
```

---

## 8. CHECKLIST TRƯỚC KHI COMMIT

### ✅ Đặt tên file/class
- [ ] Entity có prefix đúng (Us/Db/Sys)
- [ ] Model có đủ 3 file: CreateModel, UpdateModel, ViewModel
- [ ] Service có tên đúng: `{EntityName}Service.cs`
- [ ] Controller có tên đúng: `{EntityName}Controller.cs`

### ✅ Entity/Model
- [ ] Entity có đủ audit fields: RowStatus, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy
- [ ] CreateModel KHÔNG có Id và audit fields
- [ ] UpdateModel CÓ Id, KHÔNG có audit fields
- [ ] ViewModel kế thừa BaseViewModel

### ✅ Service
- [ ] Kế thừa BaseService
- [ ] Có đủ 6 phương thức: GetAll, GetFull, GetById, Create, Update, Delete
- [ ] Create có Transaction + GenerateId
- [ ] Delete dùng Soft Delete (RowStatus = Deleted)
- [ ] GenerateId đã đăng ký trong DefaultCodeConstant

### ✅ API Controller
- [ ] Có [Authorize] attribute
- [ ] Có đủ 5 endpoint: GET All, GET ById, POST, PUT, DELETE
- [ ] Trả về đúng HTTP Status Code
- [ ] Có try-catch và log lỗi
- [ ] Service đã đăng ký trong Program.cs

### ✅ Bảo mật
- [ ] Password được mã hóa bằng PasswordHelper
- [ ] API có JWT authentication
- [ ] Không expose thông tin nhạy cảm

---

## 📚 TÀI LIỆU THAM KHẢO

- **File mẫu chuẩn:** `Common.Service/UsUserService.cs`
- **Cấu trúc Database:** `LuuDATA/QLVN.sql`
- **Hướng dẫn chi tiết:** Xem file `DOC_CAU_TRUC_DU_AN.md`

---

**Cập nhật lần cuối:** 30/12/2025  
**Người tạo:** QLVN Development Team

