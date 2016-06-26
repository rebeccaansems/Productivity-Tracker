using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using SQLite;

namespace Productivity_Tracker
{
    [Activity(Label = "Productivity_Tracker", MainLauncher = true)]
    public class ButtonController : Activity
    {

        Button b_Awesome, b_Good, b_Mediocre, b_Poor, b_Terrible;
        Button b_Graph, b_Summary, b_Clear;

        TextView t_console;

        Button b_BackSummary;

        TextView t_Summary;

        Button b_BackGraph;

        SQLiteConnection db;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            db = new SQLiteConnection(MainActivity.dbPath);
            db.CreateTable<ProductiveData>();

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            LoadMain();
        }

        void LoadMain()
        {
            b_Awesome = FindViewById<Button>(Resource.Id.buttonAwesome);
            b_Good = FindViewById<Button>(Resource.Id.buttonGood);
            b_Mediocre = FindViewById<Button>(Resource.Id.buttonMediocre);
            b_Poor = FindViewById<Button>(Resource.Id.buttonPoor);
            b_Terrible = FindViewById<Button>(Resource.Id.buttonTerrible);

            b_Graph = FindViewById<Button>(Resource.Id.buttonGraph);
            b_Summary = FindViewById<Button>(Resource.Id.buttonSummary);
            b_Clear = FindViewById<Button>(Resource.Id.buttonClear);

            t_console = FindViewById<TextView>(Resource.Id.textFeeling);

            b_Awesome.Click += AwesomeClicked;
            b_Good.Click += GoodClicked;
            b_Mediocre.Click += MediocreClicked;
            b_Poor.Click += PoorClicked;
            b_Terrible.Click += TerribleClicked;

            b_Graph.Click += GraphClicked;
            b_Summary.Click += SummaryClicked;
            b_Clear.Click += ClearClicked;

            var database = db.Table<ProductiveData>();
            foreach (var dataPoint in database)
            {
                if (dataPoint.DateHour == DateTime.Now.Hour && dataPoint.DateDay == DateTime.Now.Date)
                {
                    DisbleButtons();
                    Console.WriteLine("[PRD] BUTTON NO");
                }
                Console.WriteLine("[PRD] " + dataPoint.DateHour + " COMPARED " + DateTime.Now.Hour);
            }
        }

        void LoadSummary()
        {
            b_BackSummary = FindViewById<Button>(Resource.Id.buttonBackSummary);
            t_Summary = FindViewById<TextView>(Resource.Id.textSummary);

            b_BackSummary.Click += BackClicked;
        }

        void LoadGraph()
        {
            b_BackGraph = FindViewById<Button>(Resource.Id.buttonBackGraph);

            b_BackGraph.Click += BackClicked;
        }

        void AwesomeClicked(object sender, EventArgs e)
        {
            ProductiveData p_Data = new ProductiveData { DateHour = DateTime.Now.Hour, DateDay = DateTime.Now.Date, ProdutivityLevel = 5 };
            db.Insert(p_Data);
            DisbleButtons();
        }

        void GoodClicked(object sender, EventArgs e)
        {
            ProductiveData p_Data = new ProductiveData { DateHour = DateTime.Now.Hour, DateDay = DateTime.Now.Date, ProdutivityLevel = 4 };
            db.Insert(p_Data);
            DisbleButtons();
        }

        void MediocreClicked(object sender, EventArgs e)
        {
            ProductiveData p_Data = new ProductiveData { DateHour = DateTime.Now.Hour, DateDay = DateTime.Now.Date, ProdutivityLevel = 3 };
            db.Insert(p_Data);
            DisbleButtons();
        }

        void PoorClicked(object sender, EventArgs e)
        {
            ProductiveData p_Data = new ProductiveData { DateHour = DateTime.Now.Hour, DateDay = DateTime.Now.Date, ProdutivityLevel = 2 };
            db.Insert(p_Data);
            DisbleButtons();
        }

        void TerribleClicked(object sender, EventArgs e)
        {
            ProductiveData p_Data = new ProductiveData { DateHour = DateTime.Now.Hour, DateDay = DateTime.Now.Date, ProdutivityLevel = 1 };
            db.Insert(p_Data);
            DisbleButtons();
        }

        // --------------------------------------------

        void DisbleButtons()
        {
            b_Awesome.Enabled = false;
            b_Good.Enabled = false;
            b_Mediocre.Enabled = false;
            b_Poor.Enabled = false;
            b_Terrible.Enabled = false;
        }

        // --------------------------------------------

        void GraphClicked(object sender, EventArgs e)
        {
            SetContentView(Resource.Layout.Graph);
            LoadGraph();
        }

        void SummaryClicked(object sender, EventArgs e)
        {
            SetContentView(Resource.Layout.Summary);
            LoadSummary();

            Tuple<int, int>[] productivityHour = new Tuple<int, int>[24];
            int productiveDataPoints = 0, productivityLevelTotalHour = 0;
            FakeData();

            var database = db.Table<ProductiveData>();
            for (int i = 0; i < 24; i++)
            {
                productivityHour[i] = new Tuple<int, int>(0, 0);
            }

            foreach (var dataPoint in database)
            {
                productiveDataPoints = productivityHour[dataPoint.DateHour].Item1 + 1;
                productivityLevelTotalHour = productivityHour[dataPoint.DateHour].Item2 + dataPoint.ProdutivityLevel;
                productivityHour[dataPoint.DateHour] = new Tuple<int, int>(productiveDataPoints, productivityLevelTotalHour);
            }

            string stringDBTest = "";
            for (int i = 0; i < 24; i++)
            {
                float average = 0;
                if (productivityHour[i].Item1 != 0)
                {
                    average = (float)productivityHour[i].Item2 / (float)productivityHour[i].Item1;
                }
                stringDBTest += "(" + i + ", " + average.ToString("0.00") + ")\n";
            }
            t_Summary.Text = stringDBTest;
        }

        void ClearClicked(object sender, EventArgs e)
        {
            db.DeleteAll<ProductiveData>();
        }

        void BackClicked(object sender, EventArgs e)
        {
            SetContentView(Resource.Layout.Main);
            LoadMain();
        }

        void FakeData()
        {
            ProductiveData p_Data = new ProductiveData { DateHour = 12, DateDay = DateTime.Now.Date, ProdutivityLevel = 5 };
            db.Insert(p_Data);
            p_Data = new ProductiveData { DateHour = 12, DateDay = DateTime.Now.Date, ProdutivityLevel = 4 };
            db.Insert(p_Data);
            p_Data = new ProductiveData { DateHour = 12, DateDay = DateTime.Now.Date, ProdutivityLevel = 3 };
            db.Insert(p_Data);
            p_Data = new ProductiveData { DateHour = 12, DateDay = DateTime.Now.Date, ProdutivityLevel = 2 };
            db.Insert(p_Data);
            p_Data = new ProductiveData { DateHour = 12, DateDay = DateTime.Now.Date, ProdutivityLevel = 1 };
            db.Insert(p_Data);
            p_Data = new ProductiveData { DateHour = 8, DateDay = DateTime.Now.Date, ProdutivityLevel = 5 };
            db.Insert(p_Data);
            p_Data = new ProductiveData { DateHour = 2, DateDay = DateTime.Now.Date, ProdutivityLevel = 4 };
            db.Insert(p_Data);
            p_Data = new ProductiveData { DateHour = 1, DateDay = DateTime.Now.Date, ProdutivityLevel = 3 };
            db.Insert(p_Data);
            p_Data = new ProductiveData { DateHour = 9, DateDay = DateTime.Now.Date, ProdutivityLevel = 2 };
            db.Insert(p_Data);
            p_Data = new ProductiveData { DateHour = 9, DateDay = DateTime.Now.Date, ProdutivityLevel = 1 };
            db.Insert(p_Data);
        }
    }
}