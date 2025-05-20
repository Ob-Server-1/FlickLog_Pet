namespace FlickLog_Pet.Models;


public class RegModel //Для регистрации пользователей 
{
    public Guid Id { get; set; } // Id чпользователя
    public string Name { get; set; }    
    public string Login { get; set; } 
    public string Password { get; set; }
}
