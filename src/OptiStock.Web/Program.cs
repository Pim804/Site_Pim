using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OptiStock.Application.Stock;
using OptiStock.Infrastructure.Auth;
using OptiStock.Infrastructure.Persistence;
using OptiStock.Infrastructure.Stock;
using OptiStock.Web.Services;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("OptiStock")
    ?? throw new InvalidOperationException("Configure ConnectionStrings__OptiStock antes de iniciar o OptiStock.");

builder.Services.AddDbContext<OptiStockDbContext>(options => options.UseNpgsql(connectionString));
builder.Services.AddIdentityCore<AppUser>(options =>
    {
        options.User.RequireUniqueEmail = true;
        options.Password.RequiredLength = 8;
        options.Password.RequireDigit = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
    })
    .AddRoles<IdentityRole<Guid>>()
    .AddEntityFrameworkStores<OptiStockDbContext>()
    .AddSignInManager();

var signingKey = builder.Configuration["Jwt:SigningKey"];
if (string.IsNullOrWhiteSpace(signingKey) && builder.Environment.IsDevelopment())
{
    signingKey = Convert.ToBase64String(System.Security.Cryptography.RandomNumberGenerator.GetBytes(48));
    Console.Error.WriteLine("Aviso: foi criada uma chave JWT temporária para desenvolvimento local.");
}
if (string.IsNullOrWhiteSpace(signingKey) || Encoding.UTF8.GetByteCount(signingKey) < 32)
    throw new InvalidOperationException("Jwt__SigningKey deve ter pelo menos 32 bytes.");

var signingKeyBytes = Encoding.UTF8.GetBytes(signingKey);
const string CombinedAuthScheme = "OptiStock";
builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = CombinedAuthScheme;
        options.DefaultChallengeScheme = CombinedAuthScheme;
    })
    .AddPolicyScheme(CombinedAuthScheme, "Cookie para o site e JWT para a API", options =>
    {
        options.ForwardDefaultSelector = context => context.Request.Path.StartsWithSegments("/api", StringComparison.OrdinalIgnoreCase)
            ? JwtBearerDefaults.AuthenticationScheme
            : CookieAuthenticationDefaults.AuthenticationScheme;
    })
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.Cookie.Name = "OptiStock.Web.Auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = builder.Environment.IsDevelopment() ? CookieSecurePolicy.SameAsRequest : CookieSecurePolicy.Always;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.SlidingExpiration = false;
    })
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "OptiStock",
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"] ?? "OptiStockClients",
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(signingKeyBytes),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30)
        };
        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = async context =>
            {
                var userId = context.Principal?.FindFirstValue(ClaimTypes.NameIdentifier);
                var stamp = context.Principal?.FindFirst("security_stamp")?.Value;
                var manager = context.HttpContext.RequestServices.GetRequiredService<UserManager<AppUser>>();
                var user = userId is null ? null : await manager.FindByIdAsync(userId);
                if (user is null || !user.IsActive || stamp is null || stamp != await manager.GetSecurityStampAsync(user))
                    context.Fail("Sessão inválida ou revogada.");
            }
        };
    });
builder.Services.AddAuthorization();
builder.Services.AddProblemDetails();
builder.Services.AddScoped<IStockMovementService, StockMovementService>();
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)));
builder.Services.AddOpenApi();
builder.Services.AddCors(options => options.AddPolicy("OptiStockClients", policy =>
    policy.WithOrigins(builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [])
        .AllowAnyHeader().AllowAnyMethod()));
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient<OptiStockApiClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["OptiStockApi:BaseUrl"] ?? "http://localhost:5044/");
    client.Timeout = TimeSpan.FromSeconds(15);
});

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseExceptionHandler();
    app.MapOpenApi();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
if (app.Configuration.GetValue<bool>("Database:ApplyMigrations"))
{
    await using var scope = app.Services.CreateAsyncScope();
    var db = scope.ServiceProvider.GetRequiredService<OptiStockDbContext>();
    app.Logger.LogInformation("Aplicando migrations pendentes do banco.");
    await db.Database.MigrateAsync();
}
await IdentitySeeder.SeedAsync(app.Services, app.Configuration);
if (!app.Environment.IsDevelopment() && !app.Configuration.GetValue<bool>("ReverseProxy:HttpsTerminated"))
    app.UseHttpsRedirection();

app.UseRouting();
app.UseCors("OptiStockClients");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapStaticAssets();
app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}").WithStaticAssets();
app.Run();

public partial class Program { }