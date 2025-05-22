using Microsoft.AspNetCore.Authentication.JwtBearer;



using System.Text;
using FlickLog_Pet.Models;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using System.Security.Claims;
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
        Claim[] claims = [new("userId", user.Id.ToString())]; //Клеймы, то что мы кладем в
                                                              //токен для использования атвориации в будущем

        var signingCredentials = new SigningCredentials(//Ключ для шифрования/расшифрования Jwt токена //или иначе схема аутентификации       
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey)),
            SecurityAlgorithms.HmacSha256
            );

        var token = new JwtSecurityToken(
            signingCredentials: signingCredentials,
            expires: DateTime.UtcNow.AddHours(jwtOptions.ExpiresHours),
            claims: claims
            );

        var tokenValue = new JwtSecurityTokenHandler().WriteToken(token);

        return tokenValue;
    }
}


public class JwtOptions //класс для связи с апсетингами, использууется как DI
{
    public string SecretKey { get; set; }
    public int ExpiresHours { get; set; }
}