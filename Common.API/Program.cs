using AutoMapper;
using Common.Database.Data;
using Common.Library.Helper;
using Common.Model.Common;
using Common.Service;
using Common.Service.Common;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

//using Common.Library.Helper;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorOrigin",
        policy => policy
            // Cho phép các Port của Blazor truy cập (Lấy từ launchSettings của Blazor)
            .WithOrigins("https://localhost:7096", "http://localhost:5273")
            .AllowAnyMethod() // Cho phép GET, POST, PUT, DELETE...
            .AllowAnyHeader()); // Cho phép gửi Token header
});

// ✅ 1. LẤY ENCRYPTED CONNECTION STRING
string encryptedServer = builder.Configuration.GetConnectionString("DatabaseIP") ?? "";
string encryptedDatabase = builder.Configuration.GetConnectionString("DatabaseName") ?? "";
string encryptedUserName = builder.Configuration.GetConnectionString("DatabaseUser") ?? "";
string encryptedPassword = builder.Configuration.GetConnectionString("DatabasePassword") ?? "";

// ✅ 2. TẠO MODEL VÀ SET CHO UnitOfWork
var clientSql = new SQLConnectionStringModel
{
    Ip = encryptedServer,
    Database = encryptedDatabase,
    UserName = encryptedUserName,
    Password = encryptedPassword
};
UnitOfWork.SetClientConnectionString = clientSql;

// ✅ 3. TẠO DataProvider VÀ LẤY CONNECTION STRING
var tempProvider = new DataProvider(clientSql);
string efConn = tempProvider.GetConnectionString();

Console.WriteLine("========== DECRYPTED VALUES ==========");
Console.WriteLine($"Server: {CryptorEngineHelper.Decrypt(encryptedServer)}");
Console.WriteLine($"Database: {CryptorEngineHelper.Decrypt(encryptedDatabase)}");
Console.WriteLine($"Username: {CryptorEngineHelper.Decrypt(encryptedUserName)}");
Console.WriteLine($"Password: [{CryptorEngineHelper.Decrypt(encryptedPassword)}]");
Console.WriteLine($"Password Length: {CryptorEngineHelper.Decrypt(encryptedPassword).Length}");
Console.WriteLine("======================================");
Console.WriteLine($"EF Connection String: {efConn}");
Console.WriteLine("======================================");

// ✅ 4. ĐĂNG KÝ DbContext - QUAN TRỌNG!
builder.Services.AddDbContext<QLVN_DbContext>(options =>
{
    options.UseSqlServer(efConn, sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 3,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null
        );
        sqlOptions.CommandTimeout(30);
    });
}, ServiceLifetime.Scoped); // ✅ Đảm bảo Scoped lifetime

// ✅ 5. ĐĂNG KÝ BaseEntity VỚI CÙNG CONNECTION STRING
builder.Services.AddScoped<BaseEntity>(sp => new BaseEntity(efConn));

// ✅ ĐĂNG KÝ AutoMapper - Sử dụng BaseEntity
builder.Services.AddScoped<IMapper>(sp =>
{
    var baseEntity = sp.GetRequiredService<BaseEntity>();
    return MapperConfig.BuildMapper(baseEntity);
});

// Cấu hình JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"]!))
    };

  
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            // Ghi log lỗi ra Console 
            //Console.WriteLine("Authentication failed: " + context.Exception.Message);
            return Task.CompletedTask;
        }
    };
});



// 1. Đăng ký Services (không dùng Interfaces, gọi trực tiếp Service)
builder.Services.AddScoped<SysThemeService>();
builder.Services.AddScoped<UsGroupService>();
builder.Services.AddScoped<DvsdService>();
builder.Services.AddScoped<UsUserService>();
builder.Services.AddScoped<SysMenuService>();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// --- 5. CẤU HÌNH SWAGGER (ĐỂ TEST TOKEN) ---
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Hệ thống Quản lý Vùng nuôi (QLVN) API", Version = "v1" });

    // Cấu hình định nghĩa bảo mật cho Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Vui lòng nhập Token theo định dạng: Bearer {your_token}",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });

    // Áp dụng bảo mật cho toàn bộ các API
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[]{}
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 2. KÍCH HOẠT CORS
app.UseCors("AllowBlazorOrigin");

app.UseAuthentication(); // Xác thực
app.UseAuthorization();

app.MapControllers();

app.Run();
