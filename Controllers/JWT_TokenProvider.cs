using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using FlickLog_Pet.Models;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Runtime.CompilerServices;
namespace FlickLog_Pet.Controllers;



public class JWT_TokenProvider // Класс для создания jwt токена
{
    private readonly JwtOptions jwtOptions; //По сути конфигурационный файл
    public JWT_TokenProvider(IOptions<JwtOptions> jwtOptions)
    {
        this.jwtOptions = jwtOptions.Value;
    }
    public string GenerateToken(RegModel user)
    {
        Claim[] claims =  
            [new("Id", user.Id.ToString()), 
            new("Name", user.Name.ToString())]; //Клеймы, то что мы кладем в
                                                              //токен для использования атвориации в будущем

        var signingCredentials = new SigningCredentials(//Ключ для шифрования/расшифрования Jwt токена //или иначе схема аутентификации       
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey)),
            SecurityAlgorithms.HmacSha256
            );

        var token = new JwtSecurityToken(
            signingCredentials: signingCredentials, //Схема шифровки 
            expires: DateTime.UtcNow.AddHours(jwtOptions.ExpiresHours), //Часы хранения токена
            claims: claims // Клаймы которые мы кладем внутрь
            );

        var tokenValue = new JwtSecurityTokenHandler().WriteToken(token); //Превращаем токен в строку

        return tokenValue;
    }
}


public class JwtOptions //класс для связи с апсетингами, использууется как DI
{
    public string SecretKey { get; set; }
    public int ExpiresHours { get; set; }
}



//Метод расширения для Auth

public static class Api_DOP
{
    public static void AddApiAuthentication(
        this IServiceCollection services,
        IConfiguration config)
    {
        var jwtOptions = config.GetSection(nameof(JwtOptions)).Get<JwtOptions>();
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
        {
            options.TokenValidationParameters = new()
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions!.SecretKey))
            };

            options.Events = new JwtBearerEvents()
            {
             OnMessageReceived =context =>
             {
                 context.Token = context.Request.Cookies["JwtToken"];
                 return Task.CompletedTask;
             }
            };


        });
        services.AddAuthorization();
    }
}


