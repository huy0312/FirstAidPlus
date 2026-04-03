using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.HttpOverrides;



AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();

// Add DbContext
// Add DbContext
builder.Services.AddDbContext<FirstAidPlus.Data.AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Repositories and Services
builder.Services.AddScoped<FirstAidPlus.Repositories.IUserRepository, FirstAidPlus.Repositories.UserRepository>();
builder.Services.AddScoped<FirstAidPlus.Repositories.ICourseRepository, FirstAidPlus.Repositories.CourseRepository>();
builder.Services.AddScoped<FirstAidPlus.Services.IAccountService, FirstAidPlus.Services.AccountService>();
builder.Services.AddTransient<FirstAidPlus.Services.IEmailService, FirstAidPlus.Services.EmailService>();
builder.Services.AddScoped<FirstAidPlus.Services.IVnPayService, FirstAidPlus.Services.VnPayService>();
builder.Services.AddHttpClient();
builder.Services.AddTransient<FirstAidPlus.Services.IMoMoService, FirstAidPlus.Services.MoMoService>();
builder.Services.AddSingleton<FirstAidPlus.Services.IAIService, FirstAidPlus.Services.GeminiAIService>();
builder.Services.AddHostedService<FirstAidPlus.Services.SubscriptionExpirationWorker>();

// PayOS Initialization
var payOsClientId = builder.Configuration["PayOS:ClientId"];
var payOsApiKey = builder.Configuration["PayOS:ApiKey"];
var payOsChecksumKey = builder.Configuration["PayOS:ChecksumKey"];

if (string.IsNullOrEmpty(payOsClientId) || payOsClientId.Contains("YOUR_"))
{
    Console.WriteLine("WARNING: PayOS ClientId is missing or has placeholder value!");
}

PayOS.PayOSClient payOS = new PayOS.PayOSClient(payOsClientId ?? "MISSING", payOsApiKey ?? "MISSING", payOsChecksumKey ?? "MISSING");
builder.Services.AddSingleton(payOS);
builder.Services.AddScoped<FirstAidPlus.Services.IPayOSService, FirstAidPlus.Services.PayOSService>();

// Add Authentication
var authBuilder = builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
});

var googleClientId = builder.Configuration["Authentication:Google:ClientId"];
var googleClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];

if (!string.IsNullOrEmpty(googleClientId) && !string.IsNullOrEmpty(googleClientSecret))
{
    authBuilder.AddGoogle(googleOptions =>
    {
        googleOptions.ClientId = googleClientId;
        googleOptions.ClientSecret = googleClientSecret;
        
        // Fix for "Correlation failed" over HTTP during local development/port forwarding
        googleOptions.CorrelationCookie.SameSite = SameSiteMode.Unspecified;
    });
}

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});



var app = builder.Build();

// Automatic Migration
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<FirstAidPlus.Data.AppDbContext>();
        
        // Fix: Manually ensure the Category column exists to stop the crash immediately
        // This is necessary because the EF Migration history is out of sync with the existing DB tables
        try {
            context.Database.ExecuteSqlRaw("ALTER TABLE \"courses\" ADD COLUMN IF NOT EXISTS \"category\" TEXT;");
        } catch { /* Ignore if it fails, the next step might catch it or it might already exist */ }

        context.Database.Migrate();
        Console.WriteLine("Database migration completed successfully.");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        if (ex.Message.Contains("already exists") || (ex.InnerException != null && ex.InnerException.Message.Contains("already exists")))
        {
            logger.LogWarning("Database relation already exists, skipping migration steps.");
            Console.WriteLine("Warning: Database relation already exists. Skipping migration.");
        }
        else
        {
            logger.LogError(ex, "An error occurred while migrating the database.");
            Console.WriteLine($"Error during migration: {ex.Message}");
        }
    }
}



// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
    app.UseHttpsRedirection();
}
else
{
    app.UseDeveloperExceptionPage();
}
app.UseForwardedHeaders();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapHub<FirstAidPlus.Hubs.NotificationHub>("/notificationHub");
app.MapHub<FirstAidPlus.Hubs.ChatHub>("/chatHub");

app.Run();
