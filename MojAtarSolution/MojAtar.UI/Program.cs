using Microsoft.EntityFrameworkCore;
using MojAtar.Core.Domain.RepositoryContracts;
using MojAtar.Core.ServiceContracts;
using MojAtar.Core.Services;
using MojAtar.Infrastructure.MojAtar;
using MojAtar.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login"; // Stranica za prijavu
        options.LogoutPath = "/logout"; // Stranica za odjavu
        options.AccessDeniedPath = "/accessdenied"; // Stranica ako nema prava pristupa
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Samo ako koristiš HTTPS
        options.Cookie.SameSite = SameSiteMode.Lax;
    });


builder.Services.AddAuthorization();

//builder.Services.AddLogging();
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<MojAtarDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IKorisnikRepository, KorisnikRepository>();
builder.Services.AddScoped<IKorisnikService, KorisnikService>();

builder.Services.AddScoped<IKatastarskaOpstinaRepository, KatastarskaOpstinaRepository>();
builder.Services.AddScoped<IKatastarskaOpstinaService, KatastarskaOpstinaService>();

builder.Services.AddScoped<IParcelaRepository, ParcelaRepository>();
builder.Services.AddScoped<IParcelaService, ParcelaService>();

builder.Services.AddScoped<IKulturaRepository, KulturaRepository>();
builder.Services.AddScoped<IKulturaService, KulturaService>();


var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseHsts();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


app.Run();
