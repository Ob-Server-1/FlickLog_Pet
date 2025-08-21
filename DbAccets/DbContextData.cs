using Microsoft.EntityFrameworkCore;
using FlickLog_Pet.Models;

namespace FlickLog_Pet.DbAccets;

public class DbContextData :DbContext //Для доавбление изменения или удаления карточек
{
     public DbSet<DataModel1> DataModel => Set<DataModel1>();

    public DbContextData()
    {
        Database.EnsureCreatedAsync(); //Создаем бд
    }
    protected override void OnConfiguring(DbContextOptionsBuilder dataOptions)
    {
        dataOptions.UseSqlite("Data Source=DataCard.db");
    }

}
