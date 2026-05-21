using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using StudentApi.Data;
using StudentApi.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ── Controllers ──────────────────────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// ── Swagger with JWT "Authorize" button ──────────────────────────────────────
// Adds a lock icon in Swagger UI so you can paste a JWT token and test protected endpoints.
builder.Services.AddSwaggerGen(options =>
{
    // IOpenApiSecurityScheme is the interface Swashbuckle 10 expects; OpenApiSecurityScheme implements it.
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Paste your JWT token here (without 'Bearer ' prefix)."
    });

    // Swashbuckle 10: AddSecurityRequirement takes a factory Func<OpenApiDocument, OpenApiSecurityRequirement>.
    // OpenApiSecuritySchemeReference (v2.x) replaces the old nested Reference property pattern.
    options.AddSecurityRequirement(_ => new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecuritySchemeReference("Bearer"),
            new List<string>()
        }
    });
});

// ── Database ─────────────────────────────────────────────────────────────────
// Registers AppDbContext with SQLite. The connection string is in appsettings.json.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// ── Auth Services ─────────────────────────────────────────────────────────────
// Scoped: one AuthService instance per HTTP request (correct for DB-backed services)
builder.Services.AddScoped<IAuthService, AuthService>();

// ── JWT Authentication ────────────────────────────────────────────────────────
// Reads JwtSettings from appsettings.json and configures the middleware to validate
// the "Authorization: Bearer <token>" header on every protected request.
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"]!;

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,           // Rejects expired tokens
            ValidateIssuerSigningKey = true,   // Verifies token wasn't tampered with
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
        };
    });

builder.Services.AddAuthorization();

// ── CORS (allow React frontend) ───────────────────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactApp", policy =>
        policy.WithOrigins("http://localhost:3000", "http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

var app = builder.Build();

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
