# Script test decrypt password
Write-Host "=== TEST DECRYPT PASSWORD ===" -ForegroundColor Cyan
Write-Host ""

# Encrypted password từ database
$encryptedPassword = "drgrMqLgOrVxRqQjh3Ba5g=="
Write-Host "Encrypted Password (trong DB): " -NoNewline -ForegroundColor Yellow
Write-Host $encryptedPassword

Write-Host ""
Write-Host "Để biết password gốc, chạy Common.Setting tool:" -ForegroundColor Green
Write-Host "  1. cd Common.Setting"
Write-Host "  2. dotnet run"
Write-Host "  3. Click 'Mã hóa'"
Write-Host "  4. Paste '$encryptedPassword' vào ô Data"
Write-Host "  5. Click 'Decrypt'"
Write-Host "  6. Password gốc sẽ hiện ở ô Value"
Write-Host ""

Write-Host "Hoặc test trực tiếp với các password phổ biến:" -ForegroundColor Cyan
$commonPasswords = @("admin", "123456", "password", "admin123", "12345678")

foreach ($pwd in $commonPasswords) {
    Write-Host "  Testing: $pwd" -ForegroundColor Gray
}

Write-Host ""
Write-Host "Sau khi biết password, test login API:" -ForegroundColor Green
Write-Host "  POST http://localhost:5084/api/auth/login"
Write-Host '  Body: {"userName": "admin", "password": "YOUR_PASSWORD"}'

