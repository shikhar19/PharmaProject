
using PharmacyApp.Services;

var builder = WebApplication.CreateBuilder(args);

// ─── Services ────────────────────────────────────────────────────────────────

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Keep PascalCase for C# but serialize to camelCase for JS
        options.JsonSerializerOptions.PropertyNamingPolicy =
            System.Text.Json.JsonNamingPolicy.CamelCase;
    });

builder.Services.AddEndpointsApiExplorer();

// Register application services
builder.Services.AddSingleton<IMedicineService, MedicineService>();
builder.Services.AddSingleton<ISaleService, SaleService>();

// CORS - allow React dev server
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:3000",
                "http://localhost:5173",  // Vite default port
                "http://localhost:5174",
                "https://localhost:5173",  // Vite HTTPS
                "https://localhost:5174"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// ─── Pipeline ─────────────────────────────────────────────────────────────────

var app = builder.Build();

app.UseCors("AllowReactApp");
app.UseAuthorization();
app.MapControllers();

app.Run();
