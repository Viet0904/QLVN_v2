# TÀI LIỆU MODULE SYSMENU

## 📋 TỔNG QUAN

Module **SysMenu** quản lý menu hệ thống cho ứng dụng Blazor. Menu hỗ trợ cấu trúc phân cấp (Parent-Child) và lưu trữ Icon để hiển thị giao diện.

---

## 🗂️ CẤU TRÚC DATABASE

### Bảng SysMenu

```sql
CREATE TABLE [dbo].[SysMenu] (
    [Name]       NVARCHAR(50) NOT NULL PRIMARY KEY,  -- Tên menu (duy nhất)
    [ParentMenu] NVARCHAR(50) NULL,                  -- Tên menu cha (NULL = menu gốc)
    [Note]       NVARCHAR(100) NOT NULL,             -- Mô tả menu
    [Icon]       NVARCHAR(100) NULL,                 -- Icon Bootstrap Icons
    [IsActive]   INT NULL                            -- 1=Active, 0=Inactive
);
```

### Dữ liệu mẫu

```sql
-- Menu gốc
INSERT INTO SysMenu (Name, ParentMenu, Note, Icon, IsActive)
VALUES ('dashboard', NULL, N'Trang chủ', 'bi bi-house-door', 1);

-- Menu con
INSERT INTO SysMenu (Name, ParentMenu, Note, Icon, IsActive)
VALUES ('system-user', 'system', N'Quản lý người dùng', 'bi bi-people', 1);
```

---

## 📁 CẤU TRÚC FILES

### 1. Entity
**File:** `Common.Database/Entities/SysMenu.cs`
```csharp
public partial class SysMenu
{
    public string Name { get; set; } = null!;
    public string? ParentMenu { get; set; }
    public string Note { get; set; } = null!;
    public string? Icon { get; set; }
    public int? IsActive { get; set; }
}
```

### 2. Models

#### CreateModel
**File:** `Common.Model/SysMenu/SysMenuCreateModel.cs`
```csharp
public class SysMenuCreateModel
{
    public string Name { get; set; } = null!;
    public string? ParentMenu { get; set; }
    public string Note { get; set; } = null!;
    public string? Icon { get; set; }
    public int? IsActive { get; set; }
}
```

#### UpdateModel
**File:** `Common.Model/SysMenu/SysMenuUpdateModel.cs`
```csharp
public class SysMenuUpdateModel
{
    public string Name { get; set; } = null!;
    public string? ParentMenu { get; set; }
    public string Note { get; set; } = null!;
    public string? Icon { get; set; }
    public int? IsActive { get; set; }
}
```

#### ViewModel
**File:** `Common.Model/SysMenu/SysMenuViewModel.cs`
```csharp
public class SysMenuViewModel
{
    public string Name { get; set; } = null!;
    public string? ParentMenu { get; set; }
    public string Note { get; set; } = null!;
    public string? Icon { get; set; }
    public int? IsActive { get; set; }
}
```

### 3. Service
**File:** `Common.Service/SysMenuService.cs`

**Các phương thức:**
- `GetAll()` - Lấy tất cả menu Active
- `GetFull()` - Lấy tất cả menu (bao gồm Inactive)
- `GetById(string name)` - Lấy menu theo Name
- `GetByParent(string? parentMenu)` - Lấy menu con
- `GetRootMenus()` - Lấy menu cấp 1
- `GetMenuTree()` - Lấy cây menu
- `Create(SysMenuCreateModel model)` - Tạo menu mới
- `Update(SysMenuUpdateModel model)` - Cập nhật menu
- `Delete(string name)` - Xóa menu (Soft delete)

### 4. Controller
**File:** `Common.API/Controllers/MenuController.cs`

---

## 🔌 API ENDPOINTS

### 1. Lấy tất cả menu Active
```http
GET /api/menu
Authorization: Bearer {token}
```

**Response:**
```json
{
  "isSuccess": true,
  "data": [
    {
      "name": "dashboard",
      "parentMenu": null,
      "note": "Trang chủ",
      "icon": "bi bi-house-door",
      "isActive": 1
    }
  ]
}
```

### 2. Lấy tất cả menu (bao gồm Inactive)
```http
GET /api/menu/full
Authorization: Bearer {token}
```

### 3. Lấy menu theo Name
```http
GET /api/menu/{name}
Authorization: Bearer {token}
```

**Example:**
```http
GET /api/menu/dashboard
```

### 4. Lấy menu con theo Parent
```http
GET /api/menu/parent/{parentName}
Authorization: Bearer {token}
```

**Example:**
```http
GET /api/menu/parent/system
```

### 5. Lấy menu gốc (cấp 1)
```http
GET /api/menu/root
Authorization: Bearer {token}
```

### 6. Lấy cây menu
```http
GET /api/menu/tree
Authorization: Bearer {token}
```

### 7. Tạo menu mới
```http
POST /api/menu
Authorization: Bearer {token}
Content-Type: application/json

{
  "name": "report-new",
  "parentMenu": "report",
  "note": "Báo cáo mới",
  "icon": "bi bi-file-bar-graph",
  "isActive": 1
}
```

### 8. Cập nhật menu
```http
PUT /api/menu/{name}
Authorization: Bearer {token}
Content-Type: application/json

{
  "name": "report-new",
  "parentMenu": "report",
  "note": "Báo cáo mới (updated)",
  "icon": "bi bi-file-earmark-bar-graph",
  "isActive": 1
}
```

### 9. Xóa menu (Soft delete)
```http
DELETE /api/menu/{name}
Authorization: Bearer {token}
```

**Example:**
```http
DELETE /api/menu/report-new
```

---

## 🎨 ICON BOOTSTRAP

Module sử dụng **Bootstrap Icons**. Danh sách icon phổ biến:

| Icon Class | Mô tả | Ví dụ |
|------------|-------|-------|
| `bi bi-house-door` | Trang chủ | 🏠 |
| `bi bi-people` | Người dùng | 👥 |
| `bi bi-gear` | Cài đặt | ⚙️ |
| `bi bi-file-bar-graph` | Báo cáo | 📊 |
| `bi bi-water` | Ao nuôi | 💧 |
| `bi bi-fish` | Cá | 🐟 |
| `bi bi-droplet` | Hóa chất | 💧 |
| `bi bi-map` | Khu vực | 🗺️ |
| `bi bi-truck` | Vận chuyển | 🚚 |

**Tham khảo đầy đủ:** https://icons.getbootstrap.com/

---

## 📝 VALIDATION RULES

### Tạo mới menu
- ✅ `Name` không được trống
- ✅ `Name` phải duy nhất
- ✅ `Note` không được trống
- ✅ `ParentMenu` phải tồn tại (nếu có)
- ✅ `IsActive` mặc định = 1 nếu null

### Cập nhật menu
- ✅ `Name` không được trống
- ✅ `Note` không được trống
- ✅ `ParentMenu` phải tồn tại (nếu có)

### Xóa menu
- ✅ Menu không được có menu con Active

---

## 🔧 CÀI ĐẶT

### 1. Chạy SQL Script
```bash
# Trong SQL Server Management Studio
1. Mở file: LuuDATA/Update_SysMenu_AddIcon.sql
2. Thực thi script
3. Kiểm tra kết quả: SELECT * FROM SysMenu
```

### 2. Đăng ký Service trong Program.cs
```csharp
// Common.API/Program.cs
builder.Services.AddScoped<SysMenuService>();
```

### 3. Test API qua Swagger
```
URL: https://localhost:5000/swagger
Endpoint: /api/menu
```

---

## 🧪 TESTING

### Test bằng Swagger

1. **Login để lấy token:**
   ```http
   POST /api/auth/login
   {
     "userName": "admin",
     "password": "123456"
   }
   ```

2. **Copy token và Authorize:**
   - Click "Authorize" button
   - Nhập: `Bearer {token}`

3. **Test các endpoint:**
   - GET /api/menu
   - GET /api/menu/dashboard
   - POST /api/menu
   - PUT /api/menu/{name}
   - DELETE /api/menu/{name}

---

## 💡 SỬ DỤNG TRONG BLAZOR

### Lấy danh sách menu

```csharp
// MenuService.cs
public async Task<List<SysMenuViewModel>> GetMenusAsync()
{
    var response = await _httpClient.GetFromJsonAsync<ResModel<List<SysMenuViewModel>>>("api/menu");
    return response?.Data ?? new List<SysMenuViewModel>();
}
```

### Hiển thị menu trong NavMenu.razor

```razor
@foreach (var menu in Menus.Where(x => x.ParentMenu == null))
{
    <li class="nav-item">
        <a class="nav-link" href="@menu.Name">
            <i class="@menu.Icon"></i>
            <span>@menu.Note</span>
        </a>
        
        @* Menu con *@
        @if (HasChildren(menu.Name))
        {
            <ul class="submenu">
                @foreach (var child in Menus.Where(x => x.ParentMenu == menu.Name))
                {
                    <li>
                        <a href="@child.Name">
                            <i class="@child.Icon"></i>
                            @child.Note
                        </a>
                    </li>
                }
            </ul>
        }
    </li>
}

@code {
    List<SysMenuViewModel> Menus = new();
    
    protected override async Task OnInitializedAsync()
    {
        Menus = await MenuService.GetMenusAsync();
    }
    
    bool HasChildren(string menuName)
    {
        return Menus.Any(x => x.ParentMenu == menuName);
    }
}
```

---

## ⚠️ LƯU Ý QUAN TRỌNG

1. **Primary Key là Name:** 
   - Name phải duy nhất
   - Không thể thay đổi Name sau khi tạo

2. **Soft Delete:**
   - Không xóa vật lý
   - Chỉ set `IsActive = 0`

3. **Menu Hierarchy:**
   - Menu cha phải tồn tại trước khi tạo menu con
   - Không thể xóa menu cha khi còn menu con Active

4. **Icon:**
   - Sử dụng Bootstrap Icons
   - Format: `bi bi-icon-name`
   - Icon là optional (có thể null)

---

## 📚 TÀI LIỆU THAM KHẢO

- **File mẫu:** `UsUserService.cs`
- **Cấu trúc dự án:** `DOC_CAU_TRUC_DU_AN.md`
- **Quy tắc dev:** `DOC_QUY_TAC_DEV_API.md`
- **Bootstrap Icons:** https://icons.getbootstrap.com/

---

**Ngày tạo:** 01/01/2025  
**Người tạo:** QLVN Development Team

