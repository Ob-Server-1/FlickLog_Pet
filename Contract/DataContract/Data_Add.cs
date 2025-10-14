using System.ComponentModel.DataAnnotations;

namespace FlickLog_Pet.Contract;



public record  class Data_Add(string NameFilm, string Link, int SerNumber, string DateTime, string Statuc );
public record class Data_Change(
	[Required]
	string NameFilm, 
    string Link, 
    int SerNumber, 
    string DateTime, 
    string Statuc); //Для Put запроса на изменение данных