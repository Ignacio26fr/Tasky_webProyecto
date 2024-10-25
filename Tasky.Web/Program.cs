using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tasky.Datos.EF;
using Tasky.Logica;
using Tasky.Logica.Core;
using Tasky.Logica.Gmail;
using Tasky.Logica.Redis;



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
    googleOptions.Scope.Add("https://www.googleapis.com/auth/gmail.readonly"); // Permisos para leer correos
    googleOptions.Scope.Add("https://www.googleapis.com/auth/userinfo.email"); // Permisos para obtener información del usuario
    googleOptions.Scope.Add("https://www.googleapis.com/auth/userinfo.profile"); // Permisos para obtener información del usuario
    
    googleOptions.SaveTokens = true; // Guarda los tokens de acceso y refresh
});

//Cambiarlo al appsettings.json
var connectionString = "Server=localhost\\SQLEXPRESS;Database=SmartTask;Trusted_Connection=True;";

builder.Services.AddDbContext<TaskyContext>(options =>
    options.UseSqlServer(connectionString));


builder.Services.AddIdentity<AspNetUsers, IdentityRole>()
    .AddEntityFrameworkStores<TaskyContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
{
    options.TokenLifespan = TimeSpan.FromHours(3);
});


//ideal transient para servicios de mail(por eso lo uso)
builder.Services.AddTransient<EmailService>();
builder.Services.AddScoped<IGmailAccountService, GmailAccountService>();
builder.Services.AddScoped<HttpClient>();
builder.Services.AddSingleton<ITaskManager, TaskManager>();
builder.Services.AddSingleton<IGmailTaskyService, GmailTaskyService>();
builder.Services.AddSingleton<IGmailNotificationService, GmailNotificationService>();
builder.Services.AddSingleton<IRedisSessionService, RedisSessionService>();


builder.Services.AddControllersWithViews();

//Session


builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost:6379";
    options.InstanceName = "tasky_cache";
});

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(10 * 60 * 24);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});





//var manager = new PubSubManager("tasky-439320");
//manager.DeleteAllSubscriptions();

var app = builder.Build();


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
