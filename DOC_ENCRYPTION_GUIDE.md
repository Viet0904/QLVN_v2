# HƯỚNG DẪN SỬ DỤNG MÃ HÓA TRONG DỰ ÁN QLVN

## 📋 MỤC LỤC
1. [Tổng quan](#1-tổng-quan)
2. [CryptorEngineHelper](#2-cryptorenginehelper)
3. [Common.Setting Tool](#3-commonsetting-tool)
4. [Mã hóa Connection String](#4-mã-hóa-connection-string)
5. [Best Practices](#5-best-practices)

---

## 1. TỔNG QUAN

### 1.1. Mục đích
Dự án QLVN sử dụng mã hóa AES để bảo vệ:
- ✅ Connection strings trong `appsettings.json`
- ✅ Thông tin license khách hàng
- ✅ Dữ liệu nhạy cảm khác

### 1.2. Thư viện mã hóa
**Location:** `Common.Library/Helper/CryptorEngineHelper.cs`

```csharp
public class CryptorEngineHelper
{
    // Mã hóa
    public static string Encrypt(string plainText)
    
    // Giải mã
    public static string Decrypt(string cipherText)
}
```

---

## 2. CRYPTORENGINEHELPER

### 2.1. Thông số kỹ thuật
```
Algorithm:  AES (Advanced Encryption Standard)
Mode:       CBC (Cipher Block Chaining)
Key:        HR$2pIjHR$2pIj12
IV:         HR$2pIjHR$2pIj12
Encoding:   UTF-8 (input/output)
Format:     Base64 (encrypted output)
```

### 2.2. Cách sử dụng

#### ✅ Mã hóa
```csharp
using Common.Library.Helper;

string plainText = "127.0.0.1";
string encrypted = CryptorEngineHelper.Encrypt(plainText);
// Output: "DTHWq1N8IUXY/QNmqfIksw=="
```

#### ✅ Giải mã
```csharp
using Common.Library.Helper;

string encrypted = "DTHWq1N8IUXY/QNmqfIksw==";
string decrypted = CryptorEngineHelper.Decrypt(encrypted);
// Output: "127.0.0.1"
```

### 2.3. Error Handling
```csharp
try
{
    string decrypted = CryptorEngineHelper.Decrypt(encryptedText);
}
catch (Exception ex)
{
    // Decrypt tự động return string.Empty nếu lỗi
    // Đã có ExceptionHelper.HandleException(ex) bên trong
}
```

---

## 3. COMMON.SETTING TOOL

### 3.1. Giới thiệu
**Common.Setting** là ứng dụng Windows Forms .NET 8 hỗ trợ:
- Mã hóa/Giải mã nhanh
- Tạo file license cho khách hàng

### 3.2. Chạy ứng dụng
```bash
cd Common.Setting
dotnet run
```

### 3.3. Chức năng "Mã hóa"

**Bước 1:** Click nút **"Mã hóa"** trên form chính

**Bước 2:** Nhập dữ liệu cần mã hóa vào ô **Data**

**Bước 3:** Click **Encrypt** hoặc **Decrypt**

![Screenshot](screenshot_encrypt.png)

**Ví dụ:**
```
Data:  127.0.0.1
Value: DTHWq1N8IUXY/QNmqfIksw==
```

### 3.4. Chức năng "Kích hoạt" (License)

**Bước 1:** Click nút **"Kích hoạt"** trên form chính

**Bước 2:** Điền thông tin:
- **ID**: Mã khách hàng (VD: KH001)
- **Tên công ty**: Tên doanh nghiệp
- **Server IP**: 127.0.0.1
- **Database Name**: IDI_QLVN
- **Database User**: sa
- **Password**: ********
- **Mã số FL**: XXXXX
- **SL Máy Cân**: 5

**Bước 3:** Click **"Xuất license"**

**Output:** File `.lic` được lưu tại vị trí chọn
```
License-KH001-30122025.lic
```

**Cấu trúc file license:**
```
[Base64 Encrypted String]
```

Nội dung đã mã hóa bao gồm:
```
IDKH001 CTCÔNG_TY_ABC SV127.0.0.1 DBIDI_QLVN USsa PS123456 FLXXXXX SL5
```

---

## 4. MÃ HÓA CONNECTION STRING

### 4.1. Vấn đề
Connection string trong `appsettings.json` chứa thông tin nhạy cảm:
```json
{
  "ConnectionStrings": {
    "SystemDBConnection": "Server=127.0.0.1;Database=IDI_QLVN;User=sa;Password=123456;"
  }
}
```

❌ **Rủi ro:** Password bị lộ khi commit lên Git

### 4.2. Giải pháp - Mã hóa từng phần

#### BƯỚC 1: Mã hóa từng giá trị

Sử dụng **Common.Setting** để mã hóa:

| Giá trị gốc | Mã hóa |
|-------------|--------|
| `127.0.0.1` | `DTHWq1N8IUXY/QNmqfIksw==` |
| `IDI_QLVN` | `GonGGGzs/xHlti2x9f6FAg==` |
| `sa` | `nOHe3fScfSRRQOt8TUaprg==` |
| `123456` | `p0+MSNLpL+6sb00xVRQuJw==` |

#### BƯỚC 2: Cập nhật `appsettings.json`

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

#### BƯỚC 3: Giải mã trong `Program.cs`

```csharp
using Common.Library.Helper;

// Decrypt connection string
string server = CryptorEngineHelper.Decrypt(
    builder.Configuration.GetConnectionString("DatabaseIP") ?? ""
);
string database = CryptorEngineHelper.Decrypt(
    builder.Configuration.GetConnectionString("DatabaseName") ?? ""
);
string userName = CryptorEngineHelper.Decrypt(
    builder.Configuration.GetConnectionString("DatabaseUser") ?? ""
);
string password = CryptorEngineHelper.Decrypt(
    builder.Configuration.GetConnectionString("DatabasePassword") ?? ""
);

// Build connection string
var connectionString = $"Data Source={server};Initial Catalog={database};User ID={userName};Password={password};Trust Server Certificate=True;";

// Register DbContext
builder.Services.AddDbContext<QLVN_DbContext>(options => 
    options.UseSqlServer(connectionString)
);
```

### 4.3. Development vs Production

#### Development (appsettings.Development.json)
```json
{
  "ConnectionStrings": {
    "DatabaseIP": "localhost",
    "DatabaseName": "IDI_QLVN_DEV",
    "DatabaseUser": "sa",
    "DatabasePassword": "dev_password"
  }
}
```
✅ Có thể dùng plain text cho dev environment

#### Production (appsettings.json)
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
✅ **BẮT BUỘC** mã hóa cho production

---

## 5. BEST PRACTICES

### 5.1. ✅ NÊN LÀM

1. **Mã hóa tất cả connection string trong production**
```csharp
// ✅ ĐÚNG
string password = CryptorEngineHelper.Decrypt(encryptedPassword);
```

2. **Sử dụng Common.Setting tool để mã hóa**
```
Common.Setting.exe → Mã hóa → Nhập giá trị → Copy output
```

3. **Không commit plain text connection string**
```json
// ✅ ĐÚNG
"DatabasePassword": "p0+MSNLpL+6sb00xVRQuJw=="

// ❌ SAI
"DatabasePassword": "123456"
```

4. **Validate decrypt result**
```csharp
string decrypted = CryptorEngineHelper.Decrypt(encrypted);
if (string.IsNullOrEmpty(decrypted))
{
    throw new Exception("Failed to decrypt connection string");
}
```

### 5.2. ❌ KHÔNG NÊN LÀM

1. **Không hard-code Key/IV trong nhiều nơi**
```csharp
// ❌ SAI - Duplicate key
private static string key = "HR$2pIjHR$2pIj12";

// ✅ ĐÚNG - Dùng CryptorEngineHelper
CryptorEngineHelper.Encrypt(plainText);
```

2. **Không log connection string đã decrypt**
```csharp
// ❌ SAI
Console.WriteLine($"Password: {decryptedPassword}");

// ✅ ĐÚNG
Console.WriteLine("Connection established successfully");
```

3. **Không expose encrypted data trong API response**
```csharp
// ❌ SAI
return Ok(new { connectionString = encryptedConnString });

// ✅ ĐÚNG
// Không trả về connection string
```

### 5.3. 🔐 Bảo mật Key & IV

**Key và IV hiện tại:**
```csharp
private static string cipherCode = "HR$2pIjHR$2pIj12";
private static string initVector = "HR$2pIjHR$2pIj12";
```

**⚠️ CHÚ Ý:**
- Key/IV này được dùng cho **TẤT CẢ** khách hàng
- **KHÔNG thay đổi** key/IV sau khi đã deploy production
- Nếu cần thay đổi, phải **RE-ENCRYPT** tất cả dữ liệu cũ

**Nâng cao:** Nếu cần key riêng cho từng khách hàng:
```csharp
public static string Encrypt(string plainText, string customKey, string customIV)
{
    // Implementation với custom key/IV
}
```

---

## 6. WORKFLOW MÃ HÓA CHO DỰ ÁN MỚI

### BƯỚC 1: Setup môi trường
```bash
# Build Common.Setting
cd Common.Setting
dotnet build -c Release
```

### BƯỚC 2: Mã hóa connection string
1. Chạy `Common.Setting.exe`
2. Click **"Mã hóa"**
3. Mã hóa lần lượt: IP, Database, User, Password

### BƯỚC 3: Cập nhật appsettings.json
```json
{
  "ConnectionStrings": {
    "DatabaseIP": "[ENCRYPTED_IP]",
    "DatabaseName": "[ENCRYPTED_DB]",
    "DatabaseUser": "[ENCRYPTED_USER]",
    "DatabasePassword": "[ENCRYPTED_PASS]"
  }
}
```

### BƯỚC 4: Tạo License cho khách hàng
1. Chạy `Common.Setting.exe`
2. Click **"Kích hoạt"**
3. Điền thông tin khách hàng
4. Click **"Xuất license"**
5. Gửi file `.lic` cho khách hàng

### BƯỚC 5: Verify
```bash
# Chạy API
cd Common.API
dotnet run

# Check connection
curl https://localhost:5000/api/health
```

---

## 7. TROUBLESHOOTING

### Lỗi: "Decrypt failed"
**Nguyên nhân:** Encrypted string không hợp lệ hoặc Key/IV sai

**Giải pháp:**
1. Kiểm tra Base64 string có hợp lệ không
2. Re-encrypt lại bằng Common.Setting
3. Đảm bảo Key/IV giống nhau giữa Encrypt và Decrypt

### Lỗi: "Connection failed"
**Nguyên nhân:** Connection string sau decrypt không đúng

**Giải pháp:**
```csharp
// Debug: Log decrypted values (CHỈ DÙNG TRONG DEV)
Console.WriteLine($"Server: {server}");
Console.WriteLine($"Database: {database}");
// Xóa log này sau khi debug xong
```

### Lỗi: "License invalid"
**Nguyên nhân:** File license bị sửa đổi hoặc format sai

**Giải pháp:**
1. Tạo lại license bằng Common.Setting
2. Kiểm tra file không bị corrupt
3. Đảm bảo khách hàng dùng đúng file `.lic`

---

## 📚 TÀI LIỆU THAM KHẢO

- **Source code:** `Common.Library/Helper/CryptorEngineHelper.cs`
- **Tool:** `Common.Setting/`
- **API Integration:** `Common.API/Program.cs` (dòng 30-48)
- **AES Encryption:** https://learn.microsoft.com/en-us/dotnet/api/system.security.cryptography.aes

---

## 📞 HỖ TRỢ

Nếu gặp vấn đề về mã hóa, liên hệ:
- **Team Lead:** [Tên]
- **Email:** [Email]
- **Issue Tracker:** [Link]

---

**Cập nhật lần cuối:** 30/12/2025  
**Phiên bản:** 2.0 (.NET 8)  
**Người tạo:** QLVN Development Team

