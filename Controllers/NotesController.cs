using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FlickLog_Pet.Contract;
using FlickLog_Pet.Models;
using FlickLog_Pet.DbAccets;


namespace FlickLog_Pet.Controllers;



[ApiController]
[Route("[Controller]/API")]
public class NotesController : ControllerBase
{

    [Authorize]
    [HttpGet("GetData")]
    public async Task<IActionResult> Get()
    {

        return Ok("Вы авторизованы");
    }
}
