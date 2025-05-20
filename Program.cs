using FlickLog_Pet.Controllers;
using FlickLog_Pet.DbAccets;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddScoped<DbContextReg>();
builder.Services.AddScoped<IPasswordHeasher,PasswordHeasher>();
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

app.Run();
