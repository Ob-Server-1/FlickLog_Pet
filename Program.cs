using FlickLog_Pet.Controllers;
using FlickLog_Pet.DbAccets;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(nameof(JwtOptions)));

builder.Services.AddScoped<DbContextReg>();
builder.Services.AddScoped<IPasswordHeasher,PasswordHeasher>(); //Разобратся как работает DI
builder.Services.AddScoped<JWT_TokenProvider>();
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

if (app.Environment.IsDevelopment()) //Swager Doka
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection(); 

app.MapControllers();
app.UseAuthentication(); 
app.UseAuthorization();
app.Run();
