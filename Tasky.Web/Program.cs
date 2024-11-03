using Google;
using Google.Api;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tasky.Datos.EF;
using Tasky.Logica;
using Tasky.Logica.Calendar;
using Tasky.Logica.Core;
using Tasky.Logica.Gmail;
using Tasky.Logica.Session;




var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();

// Agrega los servicios necesarios para la autenticación con la api de gmail
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
.AddCookie()
.AddGoogle(googleOptions =>
{
    googleOptions.ClientId = builder.Configuration["Google:ClientId"];
    googleOptions.ClientSecret = builder.Configuration["Google:ClienteSecret"];
    googleOptions.Scope.Add("https://www.googleapis.com/auth/calendar");
    googleOptions.Scope.Add("https://www.googleapis.com/auth/userinfo.email");
    googleOptions.Scope.Add("https://www.googleapis.com/auth/userinfo.profile");
    googleOptions.Scope.Add("openid");
    googleOptions.Scope.Add("https://www.googleapis.com/auth/gmail.readonly");
   
    // Permisos para obtener información del usuario
    googleOptions.SaveTokens = true; // Guarda los tokens de acceso y refresh
});




builder.Services.AddDbContext<TaskyContext>(options =>
    options.UseSqlServer(builder.Configuration["ConnectionStrings:DefaultConnection"]));


builder.Services.AddIdentity<AspNetUsers, IdentityRole>()
    .AddEntityFrameworkStores<TaskyContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
{
    options.TokenLifespan = TimeSpan.FromHours(3);
});


builder.Services.AddHostedService<CoreBackgroundService>();
builder.Services.AddSingleton<IEventService<TaskEventsArgs>, EventService<TaskEventsArgs>>();
builder.Services.AddSingleton<ICoreBackgroundService, CoreBackgroundService>();
builder.Services.AddTransient<EmailService>();
builder.Services.AddScoped<IGoogleAccountService, GoogleAccountService>();
builder.Services.AddSingleton<IGmailNotificationService, GmailNotificationService>();
builder.Services.AddScoped<ITaskManager, TaskManager>();
builder.Services.AddScoped<IGoogleCalendarService, GoogleCalendarService>();
builder.Services.AddScoped<HttpClient>();
builder.Services.AddScoped<IAcountSessionManager, AcountSessionManager>();




builder.Services.AddControllersWithViews();

//Session

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(10 * 60 * 24);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Limpiar la tabla GoogleSession al iniciar la aplicación
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<TaskyContext>();
    dbContext.googleSessions.RemoveRange(dbContext.googleSessions);
    dbContext.SaveChanges();
}


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}");

app.Run();
