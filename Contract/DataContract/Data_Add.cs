namespace FlickLog_Pet.Contract;
 



public record class Data_Add(
    string NameFilm,
    string Link,
    int SerNumber, 
    string DateTime, 
    string Statuc
   ); //Для POST запроса на добавление данных

public record class Data_Change( 
    string NameFilm, 
    string Link, 
    int SerNumber, 
    string DateTime, 
    string Statuc); //Для Put запроса на изменение данных