using Microsoft.EntityFrameworkCore;
using MojAtar.Core.Domain.RepositoryContracts;
using MojAtar.Core.ServiceContracts;
using MojAtar.Core.Services;
using MojAtar.Infrastructure.MojAtar;
using MojAtar.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using Rotativa.AspNetCore;


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

builder.Services.AddScoped<IPasswordHasherService, IdentityPasswordHasherService>();

builder.Services.AddScoped<IKatastarskaOpstinaRepository, KatastarskaOpstinaRepository>();
builder.Services.AddScoped<IKatastarskaOpstinaService, KatastarskaOpstinaService>();

builder.Services.AddScoped<IParcelaRepository, ParcelaRepository>();
builder.Services.AddScoped<IParcelaService, ParcelaService>();

builder.Services.AddScoped<IKulturaRepository, KulturaRepository>();
builder.Services.AddScoped<IKulturaService, KulturaService>();

builder.Services.AddScoped<IResursRepository, ResursRepository>();
builder.Services.AddScoped<IResursService, ResursService>();

builder.Services.AddScoped<IRadnaMasinaRepository, RadnaMasinaRepository>();
builder.Services.AddScoped<IRadnaMasinaService, RadnaMasinaService>();

builder.Services.AddScoped<IPrikljucnaMasinaRepository, PrikljucnaMasinaRepository>();
builder.Services.AddScoped<IPrikljucnaMasinaService, PrikljucnaMasinaService>();

builder.Services.AddScoped<IRadnjaRepository, RadnjaRepository>();
builder.Services.AddScoped<IRadnjaService, RadnjaService>();

builder.Services.AddScoped<IRadnjaRadnaMasinaRepository, RadnjaRadnaMasinaRepository>();
builder.Services.AddScoped<IRadnjaRadnaMasinaService, RadnjaRadnaMasinaService>();

builder.Services.AddScoped<IRadnjaPrikljucnaMasinaRepository, RadnjaPrikljucnaMasinaRepository>();
builder.Services.AddScoped<IRadnjaPrikljucnaMasinaService, RadnjaPrikljucnaMasinaService>();

builder.Services.AddScoped<IRadnjaResursRepository, RadnjaResursRepository>();
builder.Services.AddScoped<IRadnjaResursService, RadnjaResursService>();

builder.Services.AddScoped<IParcelaKulturaRepository, ParcelaKulturaRepository>();
builder.Services.AddScoped<IParcelaKulturaService, ParcelaKulturaService>();

builder.Services.AddScoped<IIzvestajRepository, IzvestajRepository>();
builder.Services.AddScoped<IIzvestajService, IzvestajService>();

builder.Services.AddScoped<ICenaResursaRepository, CenaResursaRepository>();
builder.Services.AddScoped<ICenaResursaService, CenaResursaService>();

builder.Services.AddScoped<ICenaKultureRepository, CenaKultureRepository>();
builder.Services.AddScoped<ICenaKultureService, CenaKultureService>();

builder.Services.AddScoped<IProdajaRepository, ProdajaRepository>();
builder.Services.AddScoped<IProdajaService, ProdajaService>();

builder.Services.AddScoped<IPocetnaService, PocetnaService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseRotativa();
app.UseHsts();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


app.Run();
