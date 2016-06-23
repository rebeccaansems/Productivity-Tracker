using System;
using SQLite;

public class ProductiveData
{
    [PrimaryKey, AutoIncrement]
    public int DataNum { get; set; }

    public DateTime Date { get; set; }

    public int ProdutivityLevel { get; set; }

    public override string ToString()
    {
        return string.Format("[Data Point: DataNum={0}, Date={1}, ProdutivityLevel={2}]", DataNum, Date, ProdutivityLevel);
    }
}