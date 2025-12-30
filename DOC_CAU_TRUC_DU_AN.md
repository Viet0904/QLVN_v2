# TÀI LIỆU CẤU TRÚC DỰ ÁN QLVN (Quản Lý Vùng Nuôi)

## 📋 MỤC LỤC
1. [Tổng quan dự án](#1-tổng-quan-dự-án)
2. [Kiến trúc hệ thống](#2-kiến-trúc-hệ-thống)
3. [Cấu trúc thư mục](#3-cấu-trúc-thư-mục)
4. [Luồng hoạt động (Flow)](#4-luồng-hoạt-động-flow)
5. [Chi tiết từng Layer](#5-chi-tiết-từng-layer)
6. [Database Schema](#6-database-schema)
7. [Công nghệ sử dụng](#7-công-nghệ-sử-dụng)
8. [Hướng dẫn tạo Module mới](#8-hướng-dẫn-tạo-module-mới)

---

## 1. TỔNG QUAN DỰ ÁN

### 1.1. Thông tin dự án
- **Tên dự án:** QLVN (Quản Lý Vùng Nuôi)
- **Mô tả:** Hệ thống quản lý vùng nuôi thủy sản
- **Kiến trúc:** N-Layer Architecture (API-Service-Database)
- **Frontend:** Blazor WebAssembly
- **Backend:** ASP.NET Core Web API
- **Database:** SQL Server

### 1.2. Các Project trong Solution

```
QLVN_Solution.sln
├── Common.API          → Web API (Controllers, Endpoints)
├── Common.Service      → Business Logic Layer
├── Common.Database     → Database Entities & DbContext
├── Common.Model        → DTOs (CreateModel, UpdateModel, ViewModel)
├── Common.Library      → Utilities (Helpers, Constants, Extensions)
├── Common.Setting      → Tool cấu hình License/Encrypt
├── WebBlazor           → Blazor WebAssembly Frontend
└── LuuDATA             → Database Scripts (QLVN.sql)
```

---

## 2. KIẾN TRÚC HỆ THỐNG

### 2.1. Sơ đồ kiến trúc tổng quan

```
┌─────────────────────────────────────────────────────────────┐
│                      CLIENT LAYER                            │
│                   (Blazor WebAssembly)                       │
│              https://localhost:7096                          │
└──────────────────────┬──────────────────────────────────────┘
                       │ HTTP/HTTPS + JWT Token
                       ↓
┌─────────────────────────────────────────────────────────────┐
│                      API LAYER                               │
│                 (Common.API - Controllers)                   │
│              https://localhost:5000/api                      │
│  ┌──────────────────────────────────────────────────────┐   │
│  │ [Authorize] JWT Authentication Middleware            │   │
│  └──────────────────────────────────────────────────────┘   │
└──────────────────────┬──────────────────────────────────────┘
                       │ Dependency Injection
                       ↓
┌─────────────────────────────────────────────────────────────┐
│                   SERVICE LAYER                              │
│              (Common.Service - Business Logic)               │
│  ┌────────────────┐  ┌────────────────┐  ┌──────────────┐  │
│  │  UsUserService │  │  DvsdService   │  │ OtherService │  │
│  └────────┬───────┘  └────────┬───────┘  └──────┬───────┘  │
│           └──────────┬─────────┴────────────────┘           │
│                      │ Inherits from                         │
│                      ↓                                        │
│            ┌──────────────────┐                              │
│            │   BaseService    │                              │
│            │ - DbContext      │                              │
│            │ - Mapper         │                              │
│            │ - GenerateId()   │                              │
│            └─────────┬────────┘                              │
└──────────────────────┼──────────────────────────────────────┘
                       │
                       ↓
┌─────────────────────────────────────────────────────────────┐
│                   DATA ACCESS LAYER                          │
│              (Common.Database - EF Core)                     │
│  ┌──────────────────────────────────────────────────────┐   │
│  │              QLVN_DbContext                          │   │
│  │  - DbSet<UsUser> UsUsers                             │   │
│  │  - DbSet<DbDvsd> DbDvsds                             │   │
│  │  - DbSet<DbAoNuoi> DbAoNuois                         │   │
│  └──────────────────────────────────────────────────────┘   │
│                        │                                      │
│                        ↓                                      │
│  ┌──────────────────────────────────────────────────────┐   │
│  │              BaseEntity                              │   │
│  │  - Auto SaveChanges với Audit Fields                │   │
│  │  - CreatedAt, CreatedBy, UpdatedAt, UpdatedBy        │   │
│  └──────────────────────────────────────────────────────┘   │
└──────────────────────┬──────────────────────────────────────┘
                       │ ADO.NET / EF Core
                       ↓
┌─────────────────────────────────────────────────────────────┐
│                   DATABASE LAYER                             │
│           SQL Server (IDI_QLVN Database)                     │
│  ┌──────────────────────────────────────────────────────┐   │
│  │  Tables:                                             │   │
│  │  - UsUser, UsGroup, UsUserLog                        │   │
│  │  - DbDvsd, DbAoNuoi, DbHoaChat, ...                 │   │
│  │  - SysSetting, SysMenu, SysIdGenerated               │   │
│  └──────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────┘
```

### 2.2. Kiến trúc theo Layer

```
┌─────────────────────────────────────────────────┐
│  Presentation Layer (WebBlazor)                 │ ← UI Components
├─────────────────────────────────────────────────┤
│  API Layer (Common.API)                         │ ← HTTP Endpoints
├─────────────────────────────────────────────────┤
│  Business Logic Layer (Common.Service)          │ ← Business Rules
├─────────────────────────────────────────────────┤
│  Data Transfer Layer (Common.Model)             │ ← DTOs
├─────────────────────────────────────────────────┤
│  Data Access Layer (Common.Database)            │ ← EF Core
├─────────────────────────────────────────────────┤
│  Cross-Cutting Layer (Common.Library)           │ ← Utilities
├─────────────────────────────────────────────────┤
│  Database Layer (SQL Server)                    │ ← Persistence
└─────────────────────────────────────────────────┘
```

---

## 3. CẤU TRÚC THƯ MỤC CHI TIẾT

### 3.1. Common.API (Web API Layer)

```
Common.API/
├── Controllers/
│   ├── AuthController.cs          → [POST] /api/auth/login
│   ├── UserController.cs          → CRUD /api/user
│   ├── GroupController.cs         → CRUD /api/group
│   ├── ThemeController.cs         → CRUD /api/theme
│   └── DvsdController.cs          → CRUD /api/dvsd
├── Program.cs                     → Cấu hình Startup (DI, CORS, JWT)
├── appsettings.json               → ConnectionString, JWT Settings
└── appsettings.Development.json   → Development Config
```

**Nhiệm vụ:**
- Nhận HTTP Request từ Client
- Validate Request (Model Binding)
- Gọi Service Layer
- Trả về HTTP Response (JSON)

### 3.2. Common.Service (Business Logic Layer)

```
Common.Service/
├── Common/
│   ├── BaseService.cs             → Base class cho tất cả Service
│   ├── BaseEntity.cs              → DbContext với Auto Audit Fields
│   ├── UnitOfWork.cs              → Transaction Management
│   └── DataProvider.cs            → Database Provider
├── Interfaces/
│   ├── IUserService.cs
│   ├── IGroupService.cs
│   └── ...
├── UsUserService.cs               → ✅ FILE MẪU CHUẨN
├── DvsdService.cs
├── UsGroupService.cs
├── SysThemeService.cs
└── SP/                            → Stored Procedure Services
```

**Nhiệm vụ:**
- Xử lý Business Logic
- Validate Business Rules
- Gọi Database (qua DbContext)
- Transaction Management
- Generate Id tự động

### 3.3. Common.Database (Data Access Layer)

```
Common.Database/
├── Data/
│   └── QLVN_DbContext.cs          → EF Core DbContext
├── Entities/
│   ├── UsUser.cs                  → User Entity (Us prefix)
│   ├── UsGroup.cs
│   ├── DbDvsd.cs                  → Đơn vị sử dụng (Db prefix)
│   ├── DbAoNuoi.cs                → Ao nuôi
│   ├── DbHoaChat.cs               → Hóa chất
│   ├── SysIdGenerated.cs          → Auto ID Generator (Sys prefix)
│   ├── SysMenu.cs
│   └── SysSetting.cs
└── Common.Database.csproj
```

**Nhiệm vụ:**
- Định nghĩa Entity (Table mapping)
- DbContext configuration
- Entity Framework Core mapping

### 3.4. Common.Model (Data Transfer Objects)

```
Common.Model/
├── Common/
│   ├── ResModel.cs                → Response wrapper
│   ├── BaseViewModel.cs           → Base cho tất cả ViewModel
│   ├── PaginatedRequest.cs        → Phân trang request
│   ├── PaginatedResponse.cs       → Phân trang response
│   ├── ErrorModel.cs
│   └── SqlCommandModel.cs
├── Auth/
│   ├── LoginRequest.cs
│   └── LoginResponse.cs
├── UsUser/
│   ├── UsUserCreateModel.cs       → Tạo mới User
│   ├── UsUserUpdateModel.cs       → Cập nhật User
│   └── UsUserViewModel.cs         → Hiển thị User
├── Group/
│   └── GroupDto.cs
├── User/
│   ├── CreateUserRequest.cs
│   ├── UpdateUserRequest.cs
│   └── UserDto.cs
└── SysTheme/
    ├── ThemeSettingsUpdateModel.cs
    └── ThemeSettingsViewModel.cs
```

**Nhiệm vụ:**
- Data Transfer Objects (DTOs)
- Request/Response Models
- Tách biệt Entity và API Contract

### 3.5. Common.Library (Utilities & Helpers)

```
Common.Library/
├── Constant/
│   ├── DefaultCodeConstant.cs     → Cấu hình GenerateId
│   ├── MessageConstant.cs         → Thông báo chuẩn
│   ├── RowStatusConstant.cs       → Active=1, Deleted=2
│   ├── FormatConstant.cs
│   └── StoreProcedureConstant.cs
├── Helper/
│   ├── PasswordHelper.cs          → Mã hóa SHA1 password
│   ├── SessionHelper.cs           → UserId cho Audit Fields
│   ├── ExceptionHelper.cs         → Log exception
│   ├── DateTimeHelper.cs
│   ├── JsonHelper.cs
│   ├── CryptorEngineHelper.cs     → Encrypt/Decrypt config
│   ├── AppSettingHelper.cs
│   └── RemoveSign4VietnameseString.cs
├── Enum/
│   └── GenderEnum.cs
└── Extension/
    └── MappingExpressionExtension.cs
```

**Nhiệm vụ:**
- Utility functions
- Helper classes
- Constants
- Extensions

### 3.6. WebBlazor (Frontend)

```
WebBlazor/
├── Pages/
│   ├── Index.razor
│   ├── Login.razor
│   └── ...
├── Layout/
│   ├── MainLayout.razor
│   └── NavMenu.razor
├── Services/
│   ├── AuthService.cs
│   └── ApiService.cs
├── Models/
├── Handlers/
│   └── AuthenticationHandler.cs   → JWT Token Handler
└── Program.cs
```

---

## 4. LUỒNG HOẠT ĐỘNG (FLOW)

### 4.1. Flow tạo mới User (Create)

```
[1] CLIENT (Blazor)
    ↓ POST /api/user + JSON Body
    │ {
    │   "groupId": "ADM",
    │   "name": "Nguyễn Văn A",
    │   "userName": "user01",
    │   "password": "123456"
    │ }
    ↓
[2] API CONTROLLER (UserController.cs)
    │ → ValidateModel (Model Binding)
    │ → [Authorize] Check JWT Token
    │ → Call Service
    ↓
[3] SERVICE LAYER (UsUserService.cs)
    │ → Check trùng lặp UserName
    │ → UnitOfWork.TransactionOpen()
    │ → Mapper.Map(CreateModel → Entity)
    │ → GenerateId("UsUser", 5) → "00001"
    │ → item.Id = GroupId + "00001" → "ADM00001"
    │ → item.Password = PasswordHelper.CreatePassword("123456")
    │ → DbContext.UsUsers.Add(item)
    │ → DbContext.SaveChanges()
    │     └─→ BaseEntity.ApplyDefaultValues()
    │         ├─→ CreatedAt = DateTime.Now
    │         ├─→ CreatedBy = SessionHelper.UserId
    │         ├─→ UpdatedAt = DateTime.Now
    │         ├─→ UpdatedBy = SessionHelper.UserId
    │         └─→ RowStatus = 1 (Active)
    │ → UnitOfWork.TransactionCommit()
    │ → GetById(newId) → Return ViewModel
    ↓
[4] DATABASE (SQL Server)
    │ → Insert vào table UsUser
    │ → Update table SysIdGenerated (TotalRows++)
    ↓
[5] RESPONSE
    │ ← ResModel<UsUserViewModel>
    │ {
    │   "isSuccess": true,
    │   "data": {
    │     "id": "ADM00001",
    │     "name": "Nguyễn Văn A",
    │     "userName": "user01",
    │     "createdAt": "2025-12-30T10:00:00",
    │     "rowStatus": 1
    │   }
    │ }
    ↓
[6] CLIENT (Blazor)
    → Hiển thị thông báo "Tạo thành công"
```

### 4.2. Flow Login (Authentication)

```
[1] CLIENT
    ↓ POST /api/auth/login
    │ { "userName": "admin", "password": "123456" }
    ↓
[2] AuthController
    │ → [AllowAnonymous] (Không cần JWT)
    │ → Call UsUserService.Login()
    ↓
[3] UsUserService
    │ → password = PasswordHelper.CreatePassword("123456")
    │ → Query: WHERE UserName=? AND Password=? AND RowStatus=1
    │ → If found → Return UsUserViewModel
    │ → If not → ErrorMessage = "Tên đăng nhập hoặc mật khẩu không đúng"
    ↓
[4] AuthController
    │ → If Success:
    │   ├─→ Generate JWT Token
    │   │   ├─ Claims: UserId, UserName, GroupId
    │   │   ├─ Expiration: 24 hours
    │   │   └─ SigningKey: From appsettings.json
    │   └─→ Return { token, user }
    ↓
[5] CLIENT
    │ → Save Token to localStorage
    │ → Add Token to HTTP Header:
    │   Authorization: Bearer {token}
    │ → Redirect to Home Page
```

### 4.3. Flow Update (Cập nhật)

```
[1] CLIENT
    ↓ PUT /api/user/ADM00001
    │ {
    │   "id": "ADM00001",
    │   "name": "Nguyễn Văn B",
    │   "email": "updated@email.com"
    │ }
    ↓
[2] UserController
    │ → [Authorize] Validate JWT Token
    │ → Check: id (URL) == model.Id (Body)
    │ → Call UsUserService.Update()
    ↓
[3] UsUserService
    │ → Query: WHERE Id = "ADM00001"
    │ → If found:
    │   ├─→ Mapper.Map(UpdateModel → Entity)
    │   │   (Chỉ update các field được phép)
    │   ├─→ DbContext.SaveChanges()
    │   │   └─→ BaseEntity auto update:
    │   │       ├─ UpdatedAt = DateTime.Now
    │   │       └─ UpdatedBy = SessionHelper.UserId
    │   └─→ Return ResModel { Data = true }
    ↓
[4] DATABASE
    │ → UPDATE UsUser SET Name=?, Email=?, UpdatedAt=?, UpdatedBy=?
    │   WHERE Id = "ADM00001"
    ↓
[5] RESPONSE
    │ ← { "isSuccess": true, "data": true }
```

### 4.4. Flow Delete (Xóa mềm)

```
[1] CLIENT
    ↓ DELETE /api/user/ADM00001
    ↓
[2] UserController
    │ → [Authorize] Validate JWT Token
    │ → Call UsUserService.Delete("ADM00001")
    ↓
[3] UsUserService
    │ → Query: WHERE Id = "ADM00001"
    │ → If found:
    │   ├─→ result.RowStatus = RowStatusConstant.Deleted (=2)
    │   ├─→ DbContext.SaveChanges()
    │   │   └─→ Auto update UpdatedAt, UpdatedBy
    │   └─→ Return ResModel { Data = true }
    │ → If not found:
    │   └─→ ErrorMessage = "Không tồn tại"
    ↓
[4] DATABASE
    │ → UPDATE UsUser SET RowStatus=2, UpdatedAt=?, UpdatedBy=?
    │   WHERE Id = "ADM00001"
    │ (KHÔNG xóa vật lý)
    ↓
[5] RESPONSE
    │ ← { "isSuccess": true, "data": true }
```

---

## 5. CHI TIẾT TỪNG LAYER

### 5.1. BaseService - Core của Service Layer

```csharp
public class BaseService
{
    // DbContext: Truy cập database
    public BaseEntity DbContext
    {
        get { return UnitOfWork.Ins.DB; }
    }
    
    // Mapper: AutoMapper
    public IMapper Mapper
    {
        get { return UnitOfWork.Ins.Mapper; }
    }
    
    // GenerateId: Tạo ID tự động
    public string GenerateId(string tableName, int length)
    {
        // Query SysIdGenerated
        // TotalRows++ 
        // Return "00001", "00002", ...
    }
    
    // ExecuteReader: Gọi Stored Procedure
    public T ExecuteReader<T>(SqlCommandModel model, Func<DbDataReader, T> customReader)
}
```

**Cách sử dụng:**

```csharp
public class DvsdService : BaseService
{
    public ResModel<DbDvsdViewModel> GetById(string id)
    {
        // Sử dụng DbContext
        var entity = DbContext.DbDvsds.FirstOrDefault(x => x.Ma == id);
        
        // Sử dụng Mapper
        var viewModel = Mapper.Map<DbDvsdViewModel>(entity);
        
        return new ResModel<DbDvsdViewModel> { Data = viewModel };
    }
}
```

### 5.2. BaseEntity - Auto Audit Fields

```csharp
public class BaseEntity : QLVN_DbContext
{
    public override int SaveChanges()
    {
        ApplyDefaultValues();  // Auto thêm audit fields
        return base.SaveChanges();
    }
    
    private void ApplyDefaultValues()
    {
        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.State == EntityState.Added)
            {
                // Khi INSERT
                entry.Property("RowStatus").CurrentValue = 1;
                entry.Property("CreatedAt").CurrentValue = DateTime.Now;
                entry.Property("CreatedBy").CurrentValue = SessionHelper.UserId;
                entry.Property("UpdatedAt").CurrentValue = DateTime.Now;
                entry.Property("UpdatedBy").CurrentValue = SessionHelper.UserId;
            }
            else if (entry.State == EntityState.Modified)
            {
                // Khi UPDATE
                entry.Property("UpdatedAt").CurrentValue = DateTime.Now;
                entry.Property("UpdatedBy").CurrentValue = SessionHelper.UserId;
            }
        }
    }
}
```

### 5.3. UnitOfWork - Transaction Management

```csharp
public class UnitOfWork
{
    public static DataProvider Ins { get; }
    
    // Sử dụng trong Service:
    public ResModel<DbDvsdViewModel> Create(DbDvsdCreateModel model)
    {
        try
        {
            UnitOfWork.Ins.TransactionOpen();      // Mở transaction
            
            // ... business logic ...
            
            UnitOfWork.Ins.TransactionCommit();    // Commit
            UnitOfWork.Ins.RenewDB();              // Renew DbContext
        }
        catch (Exception)
        {
            UnitOfWork.Ins.TransactionRollback();  // Rollback nếu lỗi
            throw;
        }
    }
}
```

### 5.4. ResModel - Response Wrapper

```csharp
public class ResModel<T>
{
    public ResModel()
    {
        IsSuccess = true;
        Errors = new List<ErrorModel>();
    }
    
    public bool IsSuccess
    {
        get
        {
            // Auto = false nếu có ErrorMessage
            if (!string.IsNullOrEmpty(ErrorMessage))
                return false;
            return isSuccess;
        }
        set { isSuccess = value; }
    }
    
    public T Data { get; set; }
    public string ErrorMessage { get; set; }
    public string Message { get; set; }
    public List<ErrorModel> Errors { get; set; }
}
```

**Ví dụ sử dụng:**

```csharp
// Thành công
return new ResModel<List<DbDvsdViewModel>>
{
    Data = list,
    Message = "Lấy dữ liệu thành công"
};

// Lỗi
return new ResModel<DbDvsdViewModel>
{
    ErrorMessage = "Tên đã tồn tại",
    Data = null
};
// IsSuccess tự động = false
```

---

## 6. DATABASE SCHEMA

### 6.1. Quy tắc Prefix Tables

| Prefix | Mục đích | Ví dụ |
|--------|----------|-------|
| **Us** | User System | UsUser, UsGroup, UsUserLog, UsUserPermission |
| **Db** | Business Data | DbDvsd, DbAoNuoi, DbHoaChat, DbKhachHang |
| **Sys** | System Config | SysSetting, SysMenu, SysIdGenerated, SysSystemInfo |

### 6.2. Table chuẩn - Audit Fields

**Tất cả table phải có 5 trường audit:**

```sql
CREATE TABLE DbDvsd
(
    Ma NVARCHAR(5) PRIMARY KEY,         -- Primary Key
    Ten NVARCHAR(50) NOT NULL,          -- Business Fields
    DiaChi NVARCHAR(500),
    
    -- AUDIT FIELDS (BẮT BUỘC)
    RowStatus INT NOT NULL DEFAULT 1,   -- 1=Active, 2=Deleted
    CreatedAt DATETIME NOT NULL,        -- Thời gian tạo
    CreatedBy NVARCHAR(8) NOT NULL,     -- Người tạo
    UpdatedAt DATETIME NOT NULL,        -- Thời gian cập nhật
    UpdatedBy NVARCHAR(8) NOT NULL,     -- Người cập nhật
    
    -- Foreign Keys
    CONSTRAINT FK_DbDvsd_UsUser FOREIGN KEY (CreatedBy) REFERENCES UsUser(Id),
    CONSTRAINT FK_DbDvsd_UsUser1 FOREIGN KEY (UpdatedBy) REFERENCES UsUser(Id)
)
```

### 6.3. SysIdGenerated - Auto ID Generator

```sql
CREATE TABLE SysIdGenerated
(
    [Table] NVARCHAR(20) PRIMARY KEY,   -- Tên table
    TotalRows INT NOT NULL,              -- Số thứ tự hiện tại
    UpdatedAt DATETIME,
    UpdatedBy NVARCHAR(8)
)

-- Dữ liệu mẫu
INSERT INTO SysIdGenerated VALUES ('UsUser', 15, GETDATE(), 'SYSTEM')
INSERT INTO SysIdGenerated VALUES ('DbDvsd', 100, GETDATE(), 'SYSTEM')
```

**Cách hoạt động:**
1. Service gọi `GenerateId("DbDvsd", 5)`
2. Query `SELECT TotalRows FROM SysIdGenerated WHERE Table='DbDvsd'` → 100
3. TotalRows++ → 101
4. Format: `101.ToString().PadLeft(5, '0')` → "00101"
5. Return "00101"

### 6.4. UsUser Table - Bảng User

```sql
CREATE TABLE UsUser
(
    Id NVARCHAR(8) PRIMARY KEY,          -- ADM00001, USER00001
    GroupId NVARCHAR(3) NOT NULL,        -- ADM, USER
    Name NVARCHAR(100) NOT NULL,
    Gender INT,
    UserName NVARCHAR(10) NOT NULL,
    Password NVARCHAR(50) NOT NULL,      -- SHA1 Encrypted
    Email NVARCHAR(30),
    Phone NVARCHAR(50),
    CMND NVARCHAR(20),
    Address NVARCHAR(200),
    Image NVARCHAR(200),
    Theme NVARCHAR(50),
    Note NVARCHAR(100),
    
    RowStatus INT NOT NULL,
    CreatedAt DATETIME NOT NULL,
    CreatedBy NVARCHAR(8) NOT NULL,
    UpdatedAt DATETIME NOT NULL,
    UpdatedBy NVARCHAR(8) NOT NULL,
    
    CONSTRAINT FK_UsUser_UsGroup FOREIGN KEY (GroupId) REFERENCES UsGroup(Id)
)
```

### 6.5. UsGroup Table - Nhóm người dùng

```sql
CREATE TABLE UsGroup
(
    Id NVARCHAR(3) PRIMARY KEY,          -- ADM, MGR, USER
    Name NVARCHAR(100) NOT NULL,
    Note NVARCHAR(200),
    
    RowStatus INT NOT NULL,
    CreatedAt DATETIME NOT NULL,
    CreatedBy NVARCHAR(8),
    UpdatedAt DATETIME NOT NULL,
    UpdatedBy NVARCHAR(8)
)
```

---

## 7. CÔNG NGHỆ SỬ DỤNG

### 7.1. Backend Stack

| Công nghệ | Version | Mục đích |
|-----------|---------|----------|
| .NET | 8.0 | Framework chính |
| ASP.NET Core | 8.0 | Web API |
| Entity Framework Core | 8.0 | ORM |
| SQL Server | 2019+ | Database |
| AutoMapper | Latest | Object Mapping |
| JWT Bearer | Latest | Authentication |
| Swagger/OpenAPI | Latest | API Documentation |

### 7.2. Frontend Stack

| Công nghệ | Version | Mục đích |
|-----------|---------|----------|
| Blazor WebAssembly | .NET 8 | SPA Framework |
| Bootstrap | 5.x | CSS Framework |
| HttpClient | .NET 8 | API Communication |

### 7.3. Thư viện quan trọng

```xml
<!-- Common.Database -->
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="8.0.0" />

<!-- Common.API -->
<PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="8.0.0" />
<PackageReference Include="Swashbuckle.AspNetCore" Version="6.5.0" />

<!-- Common.Service -->
<PackageReference Include="AutoMapper" Version="12.0.0" />
```

---

## 8. HƯỚNG DẪN TẠO MODULE MỚI

### Ví dụ: Tạo module quản lý Khách hàng (DbKhachHang)

### BƯỚC 1: Tạo Table trong Database

```sql
CREATE TABLE DbKhachHang
(
    Ma NVARCHAR(5) PRIMARY KEY,
    Ten NVARCHAR(50) NOT NULL,
    DiaChi NVARCHAR(500),
    Phone NVARCHAR(50),
    Email NVARCHAR(50),
    
    RowStatus INT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL,
    CreatedBy NVARCHAR(8) NOT NULL,
    UpdatedAt DATETIME NOT NULL,
    UpdatedBy NVARCHAR(8) NOT NULL,
    
    CONSTRAINT FK_DbKhachHang_UsUser FOREIGN KEY (CreatedBy) REFERENCES UsUser(Id),
    CONSTRAINT FK_DbKhachHang_UsUser1 FOREIGN KEY (UpdatedBy) REFERENCES UsUser(Id)
)
```

### BƯỚC 2: Tạo Entity (Common.Database/Entities/)

**File: `DbKhachHang.cs`**
```csharp
using System;

namespace Common.Database.Entities;

public partial class DbKhachHang
{
    public string Ma { get; set; } = null!;
    public string Ten { get; set; } = null!;
    public string? DiaChi { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    
    public int RowStatus { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = null!;
    public DateTime UpdatedAt { get; set; }
    public string UpdatedBy { get; set; } = null!;
    
    public virtual UsUser CreatedByNavigation { get; set; } = null!;
    public virtual UsUser UpdatedByNavigation { get; set; } = null!;
}
```

### BƯỚC 3: Thêm vào DbContext

**File: `QLVN_DbContext.cs`**
```csharp
public virtual DbSet<DbKhachHang> DbKhachHangs { get; set; }

protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<DbKhachHang>(entity =>
    {
        entity.HasKey(e => e.Ma);
        entity.ToTable("DbKhachHang");
        
        entity.Property(e => e.Ma).HasMaxLength(5);
        entity.Property(e => e.Ten).HasMaxLength(50);
        
        entity.HasOne(d => d.CreatedByNavigation)
            .WithMany(p => p.DbKhachHangCreatedByNavigations)
            .HasForeignKey(d => d.CreatedBy)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_DbKhachHang_UsUser");
    });
}
```

### BƯỚC 4: Tạo Models (Common.Model/DbKhachHang/)

**File: `DbKhachHangCreateModel.cs`**
```csharp
namespace Common.Model.DbKhachHang
{
    public class DbKhachHangCreateModel
    {
        public string Ten { get; set; } = null!;
        public string? DiaChi { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
    }
}
```

**File: `DbKhachHangUpdateModel.cs`**
```csharp
namespace Common.Model.DbKhachHang
{
    public class DbKhachHangUpdateModel
    {
        public string Id { get; set; } = null!;
        public string Ten { get; set; } = null!;
        public string? DiaChi { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
    }
}
```

**File: `DbKhachHangViewModel.cs`**
```csharp
using Common.Model.Common;

namespace Common.Model.DbKhachHang
{
    public class DbKhachHangViewModel : BaseViewModel
    {
        public string Ma { get; set; } = null!;
        public string Ten { get; set; } = null!;
        public string? DiaChi { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
    }
}
```

### BƯỚC 5: Thêm vào DefaultCodeConstant

**File: `Common.Library/Constant/DefaultCodeConstant.cs`**
```csharp
public struct DbKhachHang
{
    public const string Name = "DbKhachHang";
    public const int Length = 5;
}
```

### BƯỚC 6: Tạo Service (Common.Service/)

**File: `DbKhachHangService.cs`** - Copy từ `UsUserService.cs` và điều chỉnh

```csharp
using Common.Database.Entities;
using Common.Library.Constant;
using Common.Model.Common;
using Common.Model.DbKhachHang;
using Common.Service.Common;

namespace Common.Service
{
    public class DbKhachHangService : BaseService
    {
        public ResModel<List<DbKhachHangViewModel>> GetAll()
        {
            ResModel<List<DbKhachHangViewModel>> res = new ResModel<List<DbKhachHangViewModel>>();
            
            var results = DbContext.DbKhachHangs
                .Where(x => x.RowStatus == RowStatusConstant.Active)
                .ToList();
            res.Data = Mapper.Map<List<DbKhachHangViewModel>>(results);
            
            return res;
        }
        
        public ResModel<DbKhachHangViewModel> GetById(string id)
        {
            ResModel<DbKhachHangViewModel> res = new ResModel<DbKhachHangViewModel>();
            
            var result = DbContext.DbKhachHangs
                .Where(x => x.Ma == id && x.RowStatus == RowStatusConstant.Active)
                .FirstOrDefault();
                
            if (result != null) 
                res.Data = Mapper.Map<DbKhachHangViewModel>(result);
            else
                res.ErrorMessage = MessageConstant.NOT_EXIST;
            
            return res;
        }
        
        public ResModel<DbKhachHangViewModel> Create(DbKhachHangCreateModel model)
        {
            ResModel<DbKhachHangViewModel> res = new ResModel<DbKhachHangViewModel>();
            
            try
            {
                UnitOfWork.Ins.TransactionOpen();
                
                var item = Mapper.Map<DbKhachHang>(model);
                
                generateId:
                item.Ma = GenerateId(DefaultCodeConstant.DbKhachHang.Name, DefaultCodeConstant.DbKhachHang.Length);
                
                var resultIdExist = DbContext.DbKhachHangs.Where(x => x.Ma == item.Ma).FirstOrDefault();
                if (resultIdExist != null)
                    goto generateId;
                
                DbContext.DbKhachHangs.Add(item);
                DbContext.SaveChanges();
                
                UnitOfWork.Ins.TransactionCommit();
                UnitOfWork.Ins.RenewDB();
                
                res = GetById(item.Ma);
            }
            catch (Exception)
            {
                UnitOfWork.Ins.TransactionRollback();
                throw;
            }
            
            return res;
        }
        
        public ResModel<bool> Update(DbKhachHangUpdateModel model)
        {
            ResModel<bool> res = new ResModel<bool>();
            
            try
            {
                var result = DbContext.DbKhachHangs.Where(x => x.Ma == model.Id).FirstOrDefault();
                if (result != null)
                {
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
            }
            
            return res;
        }
        
        public ResModel<bool> Delete(string id)
        {
            ResModel<bool> res = new ResModel<bool>();
            
            var result = DbContext.DbKhachHangs.Where(x => x.Ma == id).FirstOrDefault();
            if (result != null)
            {
                result.RowStatus = RowStatusConstant.Deleted;
                DbContext.SaveChanges();
                res.Data = true;
            }
            else
            {
                res.ErrorMessage = MessageConstant.NOT_EXIST;
            }
            
            return res;
        }
    }
}
```

### BƯỚC 7: Tạo Controller (Common.API/Controllers/)

**File: `KhachHangController.cs`**

```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Common.Service;
using Common.Model.DbKhachHang;

namespace Common.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class KhachHangController : ControllerBase
    {
        private readonly DbKhachHangService _service;
        private readonly ILogger<KhachHangController> _logger;

        public KhachHangController(DbKhachHangService service, ILogger<KhachHangController> logger)
        {
            _service = service;
            _logger = logger;
        }

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
                _logger.LogError(ex, "Error getting all");
                return StatusCode(500, new { message = "Lỗi hệ thống", error = ex.Message });
            }
        }

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
                _logger.LogError(ex, "Error getting by id: {Id}", id);
                return StatusCode(500, new { message = "Lỗi hệ thống", error = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult Create([FromBody] DbKhachHangCreateModel model)
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
                _logger.LogError(ex, "Error creating");
                return StatusCode(500, new { message = "Lỗi hệ thống", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public IActionResult Update(string id, [FromBody] DbKhachHangUpdateModel model)
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
                _logger.LogError(ex, "Error updating: {Id}", id);
                return StatusCode(500, new { message = "Lỗi hệ thống", error = ex.Message });
            }
        }

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
                _logger.LogError(ex, "Error deleting: {Id}", id);
                return StatusCode(500, new { message = "Lỗi hệ thống", error = ex.Message });
            }
        }
    }
}
```

### BƯỚC 8: Đăng ký Service trong Program.cs

**File: `Common.API/Program.cs`**
```csharp
// Đăng ký Services
builder.Services.AddScoped<DbKhachHangService>();
```

### BƯỚC 9: Test API

**Swagger URL:** `https://localhost:5000/swagger`

**Test các endpoint:**
- GET /api/khachhang - Lấy tất cả
- GET /api/khachhang/{id} - Lấy theo Id
- POST /api/khachhang - Tạo mới
- PUT /api/khachhang/{id} - Cập nhật
- DELETE /api/khachhang/{id} - Xóa mềm

---

## 📚 KẾT LUẬN

### Điểm mạnh của kiến trúc:
✅ **Tách biệt rõ ràng:** API - Service - Database  
✅ **Dễ bảo trì:** Mỗi layer có trách nhiệm riêng  
✅ **Tái sử dụng:** BaseService, BaseEntity, ResModel  
✅ **Transaction Management:** UnitOfWork pattern  
✅ **Auto Audit:** CreatedAt, CreatedBy, UpdatedAt, UpdatedBy  
✅ **Soft Delete:** Không xóa vật lý, chỉ đánh dấu RowStatus  
✅ **Auto Generate ID:** SysIdGenerated table  

### Quy trình làm việc chuẩn:
1. Tạo Table trong Database (có đủ audit fields)
2. Tạo Entity trong Common.Database
3. Thêm DbSet vào DbContext
4. Tạo 3 Model: CreateModel, UpdateModel, ViewModel
5. Tạo Service (copy từ UsUserService.cs)
6. Tạo Controller (copy từ UserController.cs)
7. Đăng ký Service trong Program.cs
8. Test qua Swagger

---

**Cập nhật lần cuối:** 30/12/2025  
**Người tạo:** QLVN Development Team  
**File tham khảo:** `UsUserService.cs`, `UserController.cs`

