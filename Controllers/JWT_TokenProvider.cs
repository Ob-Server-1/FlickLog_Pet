using Microsoft.AspNetCore.Authentication.JwtBearer;




using FlickLog_Pet.Models;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace FlickLog_Pet.Controllers;




public class JWT_TokenProvider // Класс для создания jwt токена
{
    public string GenerateToken(RegModel user)
    {
        var signingCredentials = new SigningCredentials(            //Ключ для шифрования Jwt токена
            new SymmetricSecurityKey()
            
            );

        var token = new JwtSecurityToken(
            
            );





        return string.Empty;
    }


}
