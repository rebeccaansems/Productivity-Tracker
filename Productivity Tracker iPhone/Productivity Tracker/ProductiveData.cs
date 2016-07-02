using System;
using SQLite;

namespace Productivity_Tracker
{
    public class ProductiveData
    {
        [PrimaryKey, AutoIncrement]
        public int DataNum { get; set; }

        public int DateHour { get; set; }
        public int DateDay { get; set; }
        public int DateMonth { get; set; }
        public int ProdutivityLevel { get; set; }
    }
}