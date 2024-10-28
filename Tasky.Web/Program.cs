using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tasky.Datos.EF;
using Tasky.Logica;
using Tasky.Logica.Core;
using Tasky.Logica.Gmail;




var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();

// Agrega los servicios necesarios para la autenticación con la api de gmail
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    // Establece la expiración de la cookie como no persistente
    options.Cookie.IsEssential = true; // Hace la cookie esencial para evitar bloqueos de GDPR
    options.Cookie.HttpOnly = true;    // Asegura la cookie solo para HTTP, para evitar ataques de XSS
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Tiempo de expiración corto
    options.SlidingExpiration = false; // No renueva la expiración automáticamente
    options.Cookie.Expiration = null;  // Deja la cookie sin persistir

    // Expira cuando el navegador se cierra
    options.Cookie.MaxAge = null;
})
.AddGoogle(googleOptions =>
{
    googleOptions.ClientId = builder.Configuration["Google:ClientId"];
    googleOptions.ClientSecret = builder.Configuration["Google:ClienteSecret"];
    googleOptions.Scope.Add(builder.Configuration["Google:email.readonly"]); // Permisos para leer correos
    googleOptions.Scope.Add(builder.Configuration["Google:userinfo.email"]); // Permisos para obtener información del usuario
    googleOptions.Scope.Add(builder.Configuration["Google:userinfo.profile"]); // Permisos para obtener información del usuario
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


//ideal transient para servicios de mail(por eso lo uso)
builder.Services.AddTransient<EmailService>();
builder.Services.AddScoped<IGoogleAccountService, GoogleAccountService>();
builder.Services.AddScoped<IGmailNotificationService, GmailNotificationService>();
builder.Services.AddScoped<ITaskManager, TaskManager>();
builder.Services.AddScoped<HttpClient>();


builder.Services.AddControllersWithViews();

//Session

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(10 * 60 * 24);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

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
