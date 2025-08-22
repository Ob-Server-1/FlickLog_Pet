using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FlickLog_Pet.Contract;
using FlickLog_Pet.DbAccets;
using FlickLog_Pet.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
namespace FlickLog_Pet.Controllers;


[Authorize]
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
        string? requestCookies = Request.Cookies["Token"]; // Отлавливаем куки с ответом
        if (string.IsNullOrEmpty(requestCookies))
        {
            return Unauthorized("Вы не авторизованы");
        }
        try
        {
            var handler = new JwtSecurityTokenHandler();

            var userId = User.FindFirst("Id")?.Value;
            var userName = User.FindFirst("Name")?.Value;

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
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Некорректный токен {ex.Message}");
            return BadRequest("Некорректный токен");
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("Sequence contains no matching element"))
        {
            return Unauthorized("Токен не содержит необходимых данных");
        }
        catch (Exception exp)
        {
            return StatusCode(500, "Призошла ошибка на сервере, оперативно доставил в штаб");
        }
    }
   
    [HttpGet("Stat")]
    public async Task<IActionResult> Stat()
    {
     
        var userId = User.FindFirst("Id")?.Value;
        var userName = User.FindFirst("Name")?.Value;
        var data = await _context.DataModel
            .Where(x => x.UserId == userId)
            .ToListAsync();
   
        if (data == null)
        {
            return NotFound($"Вы не использовали карты, ПОШЕЛ ВОН!");
        }
        return Ok(data);
    }
}


