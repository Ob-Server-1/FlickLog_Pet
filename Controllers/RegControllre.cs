using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FlickLog_Pet.Contract;
using FlickLog_Pet.Models;
using FlickLog_Pet.DbAccets;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
namespace FlickLog_Pet.Controllers;


[ApiController]
[Route("[Controller]")]
public class RegController : ControllerBase
{
    private readonly IPasswordHeasher passwordHeasher; //ХешерПароля
    private readonly DbContextReg _context; //Контекст БД
    private readonly JWT_TokenProvider jwtProvader; // для работы с токеном

    public RegController(DbContextReg context, 
        IPasswordHeasher passwordHeasher, 
        JWT_TokenProvider jwtProvader
       ) 
    {
        _context = context;
        this.passwordHeasher = passwordHeasher; // Объект для хеширования пароля
        this.jwtProvader = jwtProvader;
      
    }

    [HttpPost]
    public async Task<IActionResult> AddUser([FromBody] DbPost_Reg? request) //FromBody нужен для получения ответа из тела запроса
    {
        // 1. Валидация входных данных
        if (string.IsNullOrEmpty(request.login) || string.IsNullOrEmpty(request.password))
        {
            return BadRequest("Логин и пароль обязательны для заполнения");
        }
        if (await _context.Users.AnyAsync(u => u.Login == request.login))
        {
            return Conflict("Пользователь с таким логином уже существует");
        }

        RegModel user = new RegModel()
        {
            Name = request.username ?? "_Blank"  ,
            Login = request.login,
            Password = passwordHeasher.Generate(request.password) //Захерировали пароль
        }; //Пока оставляю без проверки
        await _context.Users.AddAsync(user); //Добавляем пользователя
        await _context.SaveChangesAsync();
        return Ok($"Регистрация успешна, рад встречи {user.Name}");
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] DbGET_Reg request)
    {
        if (string.IsNullOrEmpty(request.login) && string.IsNullOrEmpty(request.password))
            return BadRequest("Введены не полные данные");

        var user = await _context.Users.FirstOrDefaultAsync(u=> u.Login ==request.login); //В первую очередь зачекаем есть ли логин
        if (user == null || user.Name ==string.Empty)
        {
            Console.WriteLine("Юзер не нйаден");
            return BadRequest("Пользователь не найден");
        }
        if (!passwordHeasher.Verify(request.password, user.Password))
            return BadRequest("Пользователь не найден ПОКАА");
        else
        {
           var token = jwtProvader.GenerateToken(user);

        // Основной способ установки куки
         HttpContext.Response.Cookies.Append("Token", token, new CookieOptions
        {
            HttpOnly = true,
            Secure = HttpContext.Request.IsHttps,
            SameSite = SameSiteMode.Lax,
            Expires = DateTime.UtcNow.AddDays(10000)
        });
            return Ok($"Вы успешно вошли, добро пожаловать {user.Name}");
        }
    }



    [HttpGet("cheak")]
    public async Task<IActionResult> Cheak()
    {
        if (!User.Identity.IsAuthenticated)
            return StatusCode(401);

        return Ok();
    }
}
