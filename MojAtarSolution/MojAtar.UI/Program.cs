using Microsoft.EntityFrameworkCore;
using MojAtar.Core.Domain.RepositoryContracts;
using MojAtar.Core.ServiceContracts;
using MojAtar.Core.Services;
using MojAtar.Infrastructure.MojAtar;
using MojAtar.Infrastructure.Repositories;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddLogging();
builder.Services.AddControllersWithViews();
builder.Services.AddMudServices();

builder.Services.AddDbContext<MojAtarDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IKorisnikRepository, KorisnikRepository>();
builder.Services.AddScoped<IKorisnikService, KorisnikService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.UseRouting();
app.MapControllers();


app.Run();
