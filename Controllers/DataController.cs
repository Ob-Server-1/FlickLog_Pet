using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FlickLog_Pet.Contract;
using FlickLog_Pet.DbAccets;
using FlickLog_Pet.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
namespace FlickLog_Pet.Controllers;



[ApiController]
[Route("api/[controller]")]
public class DataController : ControllerBase //ДАнный контроллербудет отвечать за добавление данных в карточки
{
    private readonly DbContextData _context;
    public DataController(DbContextData context)
    {
        _context = context;
    }
    [HttpPost("Add")]
    public async Task<IActionResult> AddData([FromBody] Data_Add request)
    {
        string? requestCookies =  Request.Cookies["Token"]; // Отлавливаем куки с ответом
        if (string.IsNullOrEmpty(requestCookies)) 
        {
            return Unauthorized("Вы не авторизованы");
        }
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(requestCookies);
        var userId = jwtToken.Claims.First(c => c.Type == "Id").Value;
        var userName = jwtToken.Claims.First(c=>c.Type=="Name").Value;

        DataModel1 dataModel = new DataModel1
        {
            NameFilm = request.NameFilm,
            Link = request.Link,
            DateTime = DateTime.Now.ToString(),
            SerNumber = request.SerNumber,
            Statuc = request.Statuc,
            UserId = userId
        };
        await _context.DataModel.AddAsync(dataModel);
        await _context.SaveChangesAsync();
        return Ok($"Ваши данные успешно получены");
    }
    [HttpGet("Stat")]
    public async Task<IActionResult> Stat()
    {
        List<DataModel1> last = _context.DataModel.ToList();
        return Ok(last); //Проблем высветить всех
    }
}


