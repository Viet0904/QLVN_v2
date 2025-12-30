# Common.Setting - Công cụ Mã hóa & License

## 📋 Mô tả
Ứng dụng Windows Forms .NET 8 để:
- Mã hóa/Giải mã dữ liệu bằng AES encryption
- Tạo file license cho khách hàng

## 🚀 Công nghệ
- **.NET 8.0** (Windows Forms)
- **AES Encryption** (CryptorEngineHelper)
- **Common.Library** (Shared utilities)

## 📁 Cấu trúc
```
Common.Setting/
├── Program.cs                    # Entry point
├── frmMain.cs                    # Form chính
├── frmDecryptEncrypt.cs          # Form mã hóa/giải mã
├── frmLicense.cs                 # Form tạo license
└── Common.Setting.csproj         # .NET 8 SDK-style project
```

## 🔧 Chức năng

### 1. Mã hóa/Giải mã (Encrypt/Decrypt)
- **Encrypt**: Mã hóa chuỗi văn bản thành Base64
- **Decrypt**: Giải mã chuỗi Base64 về văn bản gốc
- Sử dụng: `CryptorEngineHelper.Encrypt()` và `CryptorEngineHelper.Decrypt()`

### 2. Tạo License
Tạo file `.lic` chứa thông tin mã hóa:
- **ID**: Mã khách hàng
- **Tên công ty**: Tên doanh nghiệp
- **Server IP**: IP server database
- **Database Name**: Tên database
- **Database User**: User database
- **Password**: Mật khẩu database
- **Mã số FL**: Mã đăng ký
- **SL Máy Cân**: Số lượng máy cân

**Format file license:**
```
[Encrypted(ID + CT + SV + DB + US + PS + FL + SL)]
```

## 💻 Sử dụng

### Build & Run
```bash
cd Common.Setting
dotnet build
dotnet run
```

### Tạo file thực thi
```bash
dotnet publish -c Release -r win-x64 --self-contained
```

## 🔐 Mã hóa Connection String trong API

Connection string trong `appsettings.json` được mã hóa:

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

Trong `Program.cs` của API:
```csharp
string server = CryptorEngineHelper.Decrypt(builder.Configuration.GetConnectionString("DatabaseIP"));
string database = CryptorEngineHelper.Decrypt(builder.Configuration.GetConnectionString("DatabaseName"));
string userName = CryptorEngineHelper.Decrypt(builder.Configuration.GetConnectionString("DatabaseUser"));
string password = CryptorEngineHelper.Decrypt(builder.Configuration.GetConnectionString("DatabasePassword"));
```

## 📝 Cách tạo Connection String mã hóa

1. Mở **Common.Setting**
2. Click **"Mã hóa"**
3. Nhập giá trị gốc vào **Data**
4. Click **Encrypt**
5. Copy giá trị từ **Value** vào `appsettings.json`

**Ví dụ:**
- Data: `127.0.0.1`
- Value: `DTHWq1N8IUXY/QNmqfIksw==`

## 🔑 CryptorEngineHelper

### Thông số mã hóa
- **Algorithm**: AES (Advanced Encryption Standard)
- **Mode**: CBC (Cipher Block Chaining)
- **Key**: `HR$2pIjHR$2pIj12`
- **IV**: `HR$2pIjHR$2pIj12`

### API
```csharp
// Mã hóa
string encrypted = CryptorEngineHelper.Encrypt("plaintext");

// Giải mã
string decrypted = CryptorEngineHelper.Decrypt("encryptedtext");
```

## ⚠️ Lưu ý
- **KHÔNG chia sẻ** file license với người không có quyền
- **Bảo mật** key và IV trong `CryptorEngineHelper`
- Connection string mã hóa chỉ dùng cho **production**
- Trong môi trường **development**, có thể dùng connection string trực tiếp

## 🔄 Nâng cấp từ .NET Framework
Project này đã được nâng cấp từ **.NET Framework 4.7.2** lên **.NET 8**:
- ✅ Bỏ DevExpress dependencies
- ✅ Chuyển sang WinForms controls chuẩn
- ✅ SDK-style project file
- ✅ Giữ nguyên chức năng Encrypt/Decrypt

## 📞 Hỗ trợ
Liên hệ team phát triển nếu cần hỗ trợ về mã hóa hoặc tạo license.

---
**Phiên bản:** 2.0 (.NET 8)  
**Cập nhật:** 30/12/2025

