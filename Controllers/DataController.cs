using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FlickLog_Pet.Contract;
using FlickLog_Pet.DbAccets;
using FlickLog_Pet.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
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
        Console.WriteLine("Задействуеться контроллер  POST AddData!");
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
                DateTime = request.DateTime,
                SerNumber = request.SerNumber,
                Statuc = request.Statuc,
                UserId = userId
            };
            await _context.DataModel.AddAsync(dataModel);
            await _context.SaveChangesAsync();
            return Ok(dataModel);
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
    public async Task<IActionResult> Stat([FromQuery] SortAndSearch request)
    {
        List<DataModel1> result;
        var userId = User.FindFirst("Id")?.Value;
        var userName = User.FindFirst("Name")?.Value;
        var data = _context.DataModel
            .Where(x => x.UserId == userId &&
                (string.IsNullOrEmpty(request.search) ||
                 EF.Functions.Like(
                     EF.Functions.Collate(x.NameFilm, "NOCASE"),
                     $"%{Strok.ToTitleCase(request.search ?? "")}%")));

        if (!string.IsNullOrEmpty(request.sortStatuc))
        {
            data = data.Where(x => x.Statuc ==request.sortStatuc);
        }
        

        if (request.sortData == "desc")
        {
            var query = data.OrderByDescending(x => x.DateTime);
            result = await query.ToListAsync();
        }
        else
        {
            result = await data.ToListAsync();
        }



        return Ok(new
        {
            Creator = userName,
            Count = result.Count,
            Data = result
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
        return Ok(new
        {
            id=idCard,
            CheackIf.NameFilm,
            CheackIf.Link,
            CheackIf.SerNumber,
            CheackIf.DateTime,
            CheackIf.Statuc
        });
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
        return Ok(new { 
        message="Все норм карточка была удалена"});
    }
}

public static class Strok
{
    public static string ToTitleCase(string input)
{
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }

    if (input.Length == 1)
        return input.ToUpper();

    return char.ToUpper(input[0]) + input.Substring(1).ToLower();
}
}
