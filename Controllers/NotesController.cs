using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FlickLog_Pet.Contract;
using FlickLog_Pet.Models;
using FlickLog_Pet.DbAccets;
using System.Security.Claims;


namespace FlickLog_Pet.Controllers;

[ApiController]
[Route("[controller]")]
public class NotesController : ControllerBase
{
    [Authorize]
    [HttpGet("Status")]
    public async Task<IActionResult> Get()
    {
        var beta = User.Claims;
        List<string> str = new List<string>(20);

        foreach (var alfa in beta)
        { 
                str.Add(alfa.ToString());
        }
        Console.WriteLine("Использовался контроллер статуса");
        return Ok($"Статус-Вы авторизованые под именем {str[1]}");
    }
}
