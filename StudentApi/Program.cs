using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StudentApi.Data;
using StudentApi.Models;
using StudentApi.Services;
using System.Text;

// Load environment variables from .env file if it exists
var currentDirEnv = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), ".env");
var baseDirEnv = System.IO.Path.Combine(System.AppContext.BaseDirectory, ".env");
var envPath = System.IO.File.Exists(currentDirEnv) ? currentDirEnv 
    : (System.IO.File.Exists(baseDirEnv) ? baseDirEnv : null);

if (envPath != null)
{
    foreach (var line in System.IO.File.ReadAllLines(envPath))
    {
        if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#")) continue;
        var parts = line.Split('=', 2);
        if (parts.Length == 2)
        {
            var key = parts[0].Trim();
            var val = parts[1].Trim();
            if ((val.StartsWith("\"") && val.EndsWith("\"")) || (val.StartsWith("'") && val.EndsWith("'")))
            {
                val = val[1..^1];
            }
            System.Environment.SetEnvironmentVariable(key, val);
        }
    }
}

var builder = WebApplication.CreateBuilder(args);


// ── Controllers ──────────────────────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// ── Swagger with JWT "Authorize" button ──────────────────────────────────────
// Adds a lock icon in Swagger UI so you can paste a JWT token and test protected endpoints.
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Paste your JWT token here (without 'Bearer ' prefix)."
    });

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
            Array.Empty<string>()
        }
    });
});

// ── Database ─────────────────────────────────────────────────────────────────
// Registers AppDbContext with MySQL. The connection string can be in environment variables (.env) or appsettings.json.
var connectionString = builder.Configuration["DB_CONNECTION_STRING"] 
                       ?? builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// ── Auth Services ─────────────────────────────────────────────────────────────
// Scoped: one AuthService instance per HTTP request (correct for DB-backed services)
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IStudentService, StudentService>();

// ── JWT Authentication ────────────────────────────────────────────────────────
// Reads JwtSettings from .env or appsettings.json and configures the middleware to validate
// the "Authorization: Bearer <token>" header on every protected request.
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = builder.Configuration["JWT_SECRET"] ?? jwtSettings["SecretKey"]!;
var issuer = builder.Configuration["JWT_ISSUER"] ?? jwtSettings["Issuer"];
var audience = builder.Configuration["JWT_AUDIENCE"] ?? jwtSettings["Audience"];

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,           // Rejects expired tokens
            ValidateIssuerSigningKey = true,   // Verifies token wasn't tampered with
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
        };
    });

builder.Services.AddAuthorization();

// ── CORS (allow React frontend) ───────────────────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactApp", policy =>
        policy.WithOrigins("http://localhost:3000", "http://localhost:5173", "http://localhost:5174", "http://localhost:5175")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

var app = builder.Build();

// Automatically create database and tables on startup if they don't exist
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.EnsureCreated();

    // Ensure the Students table exists if the database file was created before we added the table to schema
    dbContext.Database.ExecuteSqlRaw(
        "CREATE TABLE IF NOT EXISTS Students (" +
        "id INT PRIMARY KEY AUTO_INCREMENT, " +
        "name VARCHAR(255), " +
        "age INT NOT NULL, " +
        "course VARCHAR(255));"
    );

    // Seed default student data if the database table is empty
    if (!dbContext.Students.Any())
    {
        dbContext.Students.AddRange(
            new Student { name = "Sowmya", age = 22, course = "Btech" },
            new Student { name = "Deepika", age = 21, course = "Bcom" }
        );
        dbContext.SaveChanges();
    }

    // Seed default Admin user if none exists
    if (!dbContext.Users.Any(u => u.Role == "Admin"))
    {
        dbContext.Users.Add(new User
        {
            Username = "Admin",
            Email = "admin@intuceo.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
            Role = "Admin"
        });
        dbContext.SaveChanges();
    }

    // Automatically elevate sdkurupudi@intuceo.com to Admin role if they exist
    dbContext.Database.ExecuteSqlRaw(
        "UPDATE Users SET Role = 'Admin' WHERE Email = 'sdkurupudi@intuceo.com';"
    );
}

// ── HTTP Pipeline ─────────────────────────────────────────────────────────────
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("ReactApp");

// ORDER MATTERS: Authentication must come before Authorization
app.UseAuthentication();   // Reads the JWT from the header and sets HttpContext.User
app.UseAuthorization();    // Checks [Authorize] attributes against HttpContext.User

app.MapControllers();
app.Run();
