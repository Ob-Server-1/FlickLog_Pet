namespace FlickLog_Pet.Models;

public class DataModel1 // Модель данных дял контролера с создание карт
{
    public int Id { get; set; }
    public string NameFilm { get; set; } = string.Empty;
    public string Link { get; set; } = string.Empty;
    public int SerNumber { get; set; } = 1;
    public string DateTime { get; set; } = string.Empty;
    public string Statuc { get; set; } = string.Empty;
    public string UserId { get; set; }
}
