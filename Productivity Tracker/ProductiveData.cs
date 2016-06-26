using System;
using SQLite;

public class ProductiveData
{
    [PrimaryKey, AutoIncrement]
    public int DataNum { get; set; }

    public int DateHour { get; set; }
    public DateTime DateDay { get; set; }
    public int ProdutivityLevel { get; set; }
}