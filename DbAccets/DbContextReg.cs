
using Microsoft.EntityFrameworkCore;
using FlickLog_Pet.Models;

namespace FlickLog_Pet.DbAccets;




public class DbContextReg : DbContext
{
    public DbSet<RegModel> Users => Set<RegModel>(); //Рег и авторизация модель для дбКонтекста
    public DbContextReg()
    {
        Database.EnsureCreatedAsync();
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=Users.db"); //Создание бд с таким именем, позже лучше кинуть в апсетинги
    }
}
