using BCrypt.Net;

namespace FlickLog_Pet.Controllers;

public interface IPasswordHeasher
{
    string Generate(string password);
    bool Verify(string password, string hashPassword);
}
public class PasswordHeasher : IPasswordHeasher
{   
     public string Generate(string password) =>
        BCrypt.Net.BCrypt.EnhancedHashPassword(password); //Хешируем пароль

    public bool Verify(string password, string hashPassword) =>
        BCrypt.Net.BCrypt.EnhancedVerify(password,hashPassword); //Верифицируем
}
