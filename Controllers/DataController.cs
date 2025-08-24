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

    [HttpGet("GetCard")]
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

        return Ok(new
        {
            Creator = userName,
            Count = data.Count,
            Data = data
        });
    }
    [HttpPut("ChangeCard/{idCard}")]
    public async Task<IActionResult> ChangeCard(int? idCard, [FromBody] Data_Change request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState); //

        var userId = User.FindFirst("Id")?.Value;

        var CheackIf =  await _context.DataModel
            .FirstOrDefaultAsync(x=>x.Id==idCard && x.UserId== userId);

        if (CheackIf == null) 
        {
            return BadRequest("Ошибка, карточка не была изменена");
        }
        CheackIf.NameFilm = request.NameFilm;
        CheackIf.Link = request.Link;
        CheackIf.SerNumber = request.SerNumber;
        CheackIf.DateTime = request.DateTime;
        CheackIf.Statuc = request.Statuc;

        await _context.SaveChangesAsync();
        return NoContent();
    }
    [HttpDelete("DeleteCard/{idCard}")]
    public async Task<IActionResult> DeleteCard(int idCard)
    {
        var userId = User.FindFirst("Id")?.Value;
        var CheackIf = await _context.DataModel
            .FirstOrDefaultAsync(x => x.Id == idCard && x.UserId == userId);
        if (CheackIf == null)
            return BadRequest("Операция не была выполнена");

        _context.DataModel.Remove(CheackIf);
        await _context.SaveChangesAsync();
        return Ok();
    }
}


