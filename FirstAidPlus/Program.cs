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

// PayOS Initialization
var payOsClientId = builder.Configuration["PayOS:ClientId"];
var payOsApiKey = builder.Configuration["PayOS:ApiKey"];
var payOsChecksumKey = builder.Configuration["PayOS:ChecksumKey"];
PayOS.PayOSClient payOS = new PayOS.PayOSClient(payOsClientId, payOsApiKey, payOsChecksumKey);
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
