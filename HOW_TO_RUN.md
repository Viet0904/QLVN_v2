# HƯỚNG DẪN CHẠY DỰ ÁN QLVN

## 🚀 QUICK START

### 1. Build Solution
```powershell
cd D:\QLVN_Solution\QLVN_Solution
dotnet build
```

### 2. Chạy API
```powershell
cd Common.API
dotnet run
```

**Output:**
```
Now listening on: http://localhost:5084
```

### 3. Chạy Blazor (Terminal khác)
```powershell
cd WebBlazor
dotnet run
```

---

## ⚠️ XỬ LÝ LỖI THƯỜNG GẶP

### Lỗi: File đang bị lock
```
error MSB3027: Could not copy "Common.Library.dll"
The file is locked by: "Common.API (17088)"
```

**Nguyên nhân:** API đang chạy, lock file DLL

**Giải pháp 1:** Dừng process thủ công
```powershell
# Tìm process ID
Get-Process | Where-Object {$_.ProcessName -like "*Common.API*"}

# Dừng process
Stop-Process -Id [PID] -Force
```

**Giải pháp 2:** Dừng tất cả dotnet processes
```powershell
Stop-Process -Name "dotnet" -Force
```

**Giải pháp 3:** Sử dụng `dotnet watch` thay vì `dotnet run`
```powershell
cd Common.API
dotnet watch run
```
→ Tự động reload khi có thay đổi, không cần dừng process

---

## 📝 CÁC LỆNH HỮU ÍCH

### Build
```powershell
# Build toàn bộ solution
dotnet build

# Build project cụ thể
dotnet build Common.API\Common.API.csproj

# Build Release mode
dotnet build -c Release
```

### Clean
```powershell
# Clean build artifacts
dotnet clean

# Clean + Rebuild
dotnet clean && dotnet build
```

### Restore Packages
```powershell
# Restore NuGet packages
dotnet restore
```

### Run
```powershell
# Chạy API
cd Common.API
dotnet run

# Chạy với hot reload
dotnet watch run

# Chạy trong background (PowerShell)
Start-Process powershell -ArgumentList "dotnet run" -WorkingDirectory "Common.API"
```

### Stop Processes
```powershell
# Dừng tất cả dotnet processes
Get-Process dotnet | Stop-Process -Force

# Dừng process cụ thể
Stop-Process -Id [PID] -Force

# Dừng theo tên
Stop-Process -Name "Common.API" -Force
```

---

## 🔧 DEVELOPMENT WORKFLOW

### Workflow chuẩn

#### Bước 1: Start API (Terminal 1)
```powershell
cd D:\QLVN_Solution\QLVN_Solution\Common.API
dotnet watch run
```

#### Bước 2: Start Blazor (Terminal 2)
```powershell
cd D:\QLVN_Solution\QLVN_Solution\WebBlazor
dotnet watch run
```

#### Bước 3: Làm việc
- Sửa code trong Common.Library, Common.Service, Common.API
- `dotnet watch` tự động reload
- Không cần dừng process

#### Bước 4: Stop (khi xong)
```powershell
# Trong mỗi terminal
Ctrl + C

# Hoặc PowerShell command
Get-Process dotnet | Stop-Process -Force
```

---

## 🌐 PORTS

| Service | URL |
|---------|-----|
| **API** | http://localhost:5084 |
| **Blazor** | https://localhost:7096 |
| **Swagger** | http://localhost:5084/swagger |

### Kiểm tra ports đang sử dụng
```powershell
# Xem ports đang listen
netstat -ano | findstr ":5084"
netstat -ano | findstr ":7096"

# Kill process theo port
$processId = Get-NetTCPConnection -LocalPort 5084 | Select-Object -ExpandProperty OwningProcess
Stop-Process -Id $processId -Force
```

---

## 🗄️ DATABASE

### Connection String (Encrypted)

**File:** `Common.API/appsettings.json`

```json
{
  "ConnectionStrings": {
    "DatabaseIP": "DTHWq1N8IUXY/QNmqfIksw==",
    "DatabaseName": "GonGGGzs/xHlti2x9f6FAg==",
    "DatabaseUser": "nOHe3fScfSRRQOt8TUaprg==",
    "DatabasePassword": "p0+MSNLpL+6sb00xVRQuJw=="
  }
}
```

**Decrypted values:**
- IP: `127.0.0.1`
- Database: `IDI_QLVN`
- User: `sa`
- Password: `123456`

### Test Database Connection
```sql
-- Chạy trong SQL Server Management Studio
USE IDI_QLVN;
SELECT * FROM UsUser;
```

---

## 🧪 TESTING

### Test API với curl
```powershell
# Test health endpoint (nếu có)
curl http://localhost:5084/api/health

# Test login
curl -X POST http://localhost:5084/api/auth/login `
  -H "Content-Type: application/json" `
  -d '{"userName":"admin","password":"123456"}'

# Test với token
$token = "your_jwt_token_here"
curl http://localhost:5084/api/user `
  -H "Authorization: Bearer $token"
```

### Test với Swagger
1. Mở browser: `http://localhost:5084/swagger`
2. Click **"Authorize"**
3. Nhập token: `Bearer {your_token}`
4. Test các API endpoints

---

## 🛠️ TROUBLESHOOTING

### Lỗi 1: Port đã được sử dụng
```
Failed to bind to address http://127.0.0.1:5084
```

**Giải pháp:**
```powershell
# Tìm process đang dùng port
netstat -ano | findstr ":5084"

# Kill process
Stop-Process -Id [PID] -Force
```

### Lỗi 2: Connection String decrypt failed
```
CryptorEngineHelper.Decrypt failed: Invalid Base-64 string
```

**Giải pháp:**
1. Kiểm tra `appsettings.json` có đúng format không
2. Dùng **Common.Setting** để tạo lại encrypted string
3. Xem [BUGFIX_ENCRYPTION.md](./BUGFIX_ENCRYPTION.md)

### Lỗi 3: Database connection failed
```
Cannot connect to SQL Server
```

**Giải pháp:**
1. Kiểm tra SQL Server đang chạy
2. Kiểm tra connection string
3. Test với SSMS:
```sql
Server: 127.0.0.1
Database: IDI_QLVN
User: sa
Password: 123456
```

### Lỗi 4: File DLL locked
```
Could not copy "Common.Library.dll"
```

**Giải pháp:**
```powershell
# Dừng tất cả dotnet
Get-Process dotnet | Stop-Process -Force

# Build lại
dotnet build
```

### Lỗi 5: NuGet restore failed
```
error NU1101: Unable to find package
```

**Giải pháp:**
```powershell
# Clear NuGet cache
dotnet nuget locals all --clear

# Restore lại
dotnet restore
```

---

## 📦 PUBLISH

### Publish API
```powershell
cd Common.API
dotnet publish -c Release -o publish
```

### Publish Blazor
```powershell
cd WebBlazor
dotnet publish -c Release -o publish
```

### Publish với self-contained
```powershell
# Windows x64
dotnet publish -c Release -r win-x64 --self-contained

# Windows x86
dotnet publish -c Release -r win-x86 --self-contained
```

---

## 🐳 DOCKER (Optional)

### Build Docker Image
```dockerfile
# Dockerfile cho API
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY publish/ .
ENTRYPOINT ["dotnet", "Common.API.dll"]
```

```powershell
docker build -t qlvn-api .
docker run -p 5084:80 qlvn-api
```

---

## 📚 TÀI LIỆU THAM KHẢO

- [DOC_CAU_TRUC_DU_AN.md](./DOC_CAU_TRUC_DU_AN.md) - Cấu trúc dự án
- [DOC_QUY_TAC_DEV_API.md](./DOC_QUY_TAC_DEV_API.md) - Quy tắc dev
- [DOC_ENCRYPTION_GUIDE.md](./DOC_ENCRYPTION_GUIDE.md) - Hướng dẫn mã hóa
- [BUGFIX_ENCRYPTION.md](./BUGFIX_ENCRYPTION.md) - Bug fixes
- [Common.Setting/README.md](./Common.Setting/README.md) - Tool mã hóa

---

## 💡 TIPS & TRICKS

### 1. Sử dụng dotnet watch
```powershell
# Tự động reload khi code thay đổi
dotnet watch run
```

### 2. Multiple Terminals
- **Terminal 1:** API (`dotnet watch run`)
- **Terminal 2:** Blazor (`dotnet watch run`)
- **Terminal 3:** Commands (`dotnet build`, `git`, etc.)

### 3. VS Code Tasks
Tạo `.vscode/tasks.json`:
```json
{
  "version": "2.0.0",
  "tasks": [
    {
      "label": "Run API",
      "command": "dotnet",
      "args": ["watch", "run"],
      "options": {
        "cwd": "${workspaceFolder}/Common.API"
      }
    }
  ]
}
```

### 4. PowerShell Aliases
Thêm vào PowerShell profile:
```powershell
# notepad $PROFILE
function api { cd D:\QLVN_Solution\QLVN_Solution\Common.API; dotnet watch run }
function blazor { cd D:\QLVN_Solution\QLVN_Solution\WebBlazor; dotnet watch run }
function build { cd D:\QLVN_Solution\QLVN_Solution; dotnet build }
function stop-api { Get-Process dotnet | Stop-Process -Force }
```

Sử dụng:
```powershell
api      # Chạy API
blazor   # Chạy Blazor
build    # Build solution
stop-api # Dừng API
```

---

## ⌨️ KEYBOARD SHORTCUTS

| Shortcut | Action |
|----------|--------|
| `Ctrl + C` | Dừng process trong terminal |
| `Ctrl + Shift + `` | Mở terminal mới trong VS Code |
| `F5` | Debug trong Visual Studio |
| `Ctrl + F5` | Run without debugging |

---

**Cập nhật:** 30/12/2025  
**Version:** 1.0  
**Status:** ✅ Production Ready

