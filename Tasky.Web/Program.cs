using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tasky.Datos.EF;
using Tasky.Logica;


var builder = WebApplication.CreateBuilder(args);

//Cambiarlo al appsettings.json
var connectionString = "Server=INF-037\\SQLEXPRESS;Database=SmartTask;Trusted_Connection=True;";

builder.Services.AddDbContext<TaskyContext>(options =>
    options.UseSqlServer(connectionString));


builder.Services.AddIdentity<Usuario, IdentityRole<int>>()
    .AddEntityFrameworkStores<TaskyContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<IUsuarioServicio, UsuarioServicio>();
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
