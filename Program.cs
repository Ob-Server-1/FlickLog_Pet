using FlickLog_Pet.Controllers;
using FlickLog_Pet.DbAccets;
using Microsoft.AspNetCore.CookiePolicy;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(nameof(JwtOptions)));

builder.Services.AddScoped<DbContextData>();
builder.Services.AddScoped<DbContextReg>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Information); // или \\
                                                       //
builder.WebHost.UseUrls("http://0.0.0.0:7047");
builder.Services.AddScoped<IPasswordHeasher,PasswordHeasher>(); //Разобратся как работает DI
builder.Services.AddScoped<JWT_TokenProvider>();

builder.Services.AddControllers();

builder.Services.AddApiAuthentication(builder.Configuration);


var app = builder.Build();


app.UseCookiePolicy(new CookiePolicyOptions
{
    MinimumSameSitePolicy = SameSiteMode.Lax, // ✅ Для HTTP
    HttpOnly = HttpOnlyPolicy.Always,
    Secure = CookieSecurePolicy.SameAsRequest // ✅ Если HTTP → кука без Secure
});


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();
app.UseRouting();

//app.MapFallbackToFile("index.html"); // Перехватывает все маршруты и возвращает index.html
app.UseHttpsRedirection();

app.UseAuthentication(); 
app.UseAuthorization();
app.MapControllers();
app.Run();
