using FlickLog_Pet.Controllers;
using FlickLog_Pet.DbAccets;
using Microsoft.AspNetCore.CookiePolicy;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(nameof(JwtOptions)));

builder.Services.AddScoped<DbContextData>();
builder.Services.AddScoped<DbContextReg>();

builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Information); // ��� Debug

builder.Services.AddScoped<IPasswordHeasher,PasswordHeasher>(); //���������� ��� �������� DI
builder.Services.AddScoped<JWT_TokenProvider>();

builder.Services.AddControllers();

builder.Services.AddApiAuthentication(builder.Configuration);
builder.Services.AddSwaggerGen();
builder.Services.AddEndpointsApiExplorer();



var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseCookiePolicy(new CookiePolicyOptions
{
    MinimumSameSitePolicy = SameSiteMode.None,
    HttpOnly = HttpOnlyPolicy.Always,
    Secure = CookieSecurePolicy.Always
}); //�������� ���� ��� ������������

//app.UseStaticFiles();
app.UseRouting();

//app.MapFallbackToFile("index.html"); // ������������� ��� �������� � ���������� index.html
app.UseHttpsRedirection();
app.MapControllers();
app.UseAuthentication(); 
app.UseAuthorization();
app.Run();
