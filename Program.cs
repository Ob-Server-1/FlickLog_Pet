using FlickLog_Pet.Controllers;
using FlickLog_Pet.DbAccets;
using Microsoft.AspNetCore.CookiePolicy;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(nameof(JwtOptions)));

builder.Services.AddScoped<DbContextData>();
builder.Services.AddScoped<DbContextReg>();

builder.Services.AddScoped<IPasswordHeasher,PasswordHeasher>(); //Разобратся как работает DI
builder.Services.AddScoped<JWT_TokenProvider>();

builder.Services.AddControllers();

builder.Services.AddApiAuthentication(builder.Configuration);

var app = builder.Build();

app.UseCors();



app.MapControllers();

app.UseCookiePolicy(new CookiePolicyOptions
{
    MinimumSameSitePolicy = SameSiteMode.None,
    HttpOnly = HttpOnlyPolicy.Always,
    Secure = CookieSecurePolicy.Always
}); //Поликтки куки для безопасности

app.UseStaticFiles();
app.UseRouting();

app.MapFallbackToFile("index.html"); // Перехватывает все маршруты и возвращает index.html
app.UseHttpsRedirection();
app.UseAuthentication(); 
app.UseAuthorization();
app.Run();
