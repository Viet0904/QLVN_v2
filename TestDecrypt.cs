using Common.Library.Helper;

// Test decrypt password từ database
string encryptedPassword = "drgrMqLgOrVxRqQjh3Ba5g==";
string decryptedPassword = CryptorEngineHelper.Decrypt(encryptedPassword);

Console.WriteLine($"Encrypted: {encryptedPassword}");
Console.WriteLine($"Decrypted: {decryptedPassword}");
Console.WriteLine($"");
Console.WriteLine($"Để login, sử dụng:");
Console.WriteLine($"  Username: admin");
Console.WriteLine($"  Password: {decryptedPassword}");

