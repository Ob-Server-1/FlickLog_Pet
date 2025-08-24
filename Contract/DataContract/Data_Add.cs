namespace FlickLog_Pet.Contract;
 



public record class Data_Add(
    string NameFilm,
    string Link,
    int SerNumber, 
    string DateTime, 
    string Statuc, 
    string UserId); //Для POST запроса на добавление данных

public record class Data_Change(int Id, 
    string NameFilm, 
    string Link, 
    int SerNumber, 
    string DateTime, 
    string Statuc,
    string UserId); //Для Put запроса на изменение данных