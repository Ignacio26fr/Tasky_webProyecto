using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tasky.Datos.EF;
using Tasky.Logica;



var builder = WebApplication.CreateBuilder(args);

//Cambiarlo al appsettings.json
var connectionString = "Server=INF-037\\SQLEXPRESS;Database=SmartTask;Trusted_Connection=True;";

builder.Services.AddDbContext<TaskyContext>(options =>
    options.UseSqlServer(connectionString));


builder.Services.AddIdentity<AspNetUsers, IdentityRole>()
    .AddEntityFrameworkStores<TaskyContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
{
    options.TokenLifespan = TimeSpan.FromHours(3);
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
    .AddCookie()
    .AddGoogle(options =>
    {
        
      options.Scope.Add(builder.Configuration["email.readonly"]); // Permisos para leer correos
     options.Scope.Add(builder.Configuration["userinfo.email"]); // Permisos para obtener información del usuario
        options.Scope.Add(builder.Configuration["userinfo.profile"]); // Permisos para obtener información del usuario

        options.ClientId = builder.Configuration["Google:ClientId"];
        options.ClientSecret = builder.Configuration["Google:ClienteSecret"];
        
        options.Scope.Add("email");
        options.SaveTokens = true;
    });


//ideal transient para servicios de mail(por eso lo uso)
builder.Services.AddTransient<EmailService>();

builder.Services.AddControllersWithViews();

var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); 
app.UseAuthorization();  


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}");

app.Run();
