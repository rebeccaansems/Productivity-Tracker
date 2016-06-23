using System;
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
        Button b_Graph, b_Summary;

        TextView t_console;

        SQLiteConnection db;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            db = new SQLiteConnection(MainActivity.dbPath);
            db.CreateTable<ProductiveData>();

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            b_Awesome = FindViewById<Button>(Resource.Id.buttonAwesome);
            b_Good = FindViewById<Button>(Resource.Id.buttonGood);
            b_Mediocre = FindViewById<Button>(Resource.Id.buttonMediocre);
            b_Poor = FindViewById<Button>(Resource.Id.buttonPoor);
            b_Terrible = FindViewById<Button>(Resource.Id.buttonTerrible);

            b_Graph = FindViewById<Button>(Resource.Id.buttonGraph);
            b_Summary = FindViewById<Button>(Resource.Id.buttonSummary);

            t_console = FindViewById<TextView>(Resource.Id.textFeeling);

            b_Awesome.Click += AwesomeClicked;
            b_Good.Click += GoodClicked;
            b_Mediocre.Click += MediocreClicked;
            b_Poor.Click += PoorClicked;
            b_Terrible.Click += TerribleClicked;

            b_Graph.Click += GraphClicked;
            b_Summary.Click += SummaryClicked;
        }

        void AwesomeClicked(object sender, EventArgs e)
        {
            b_Awesome.Text = MainActivity.dbPath;
            ProductiveData p_Data = new ProductiveData { Date = DateTime.Now, ProdutivityLevel =  1};
            db.Insert(p_Data);
        }

        void GoodClicked(object sender, EventArgs e)
        {
            b_Good.Text = "2";
            ProductiveData p_Data = new ProductiveData { Date = DateTime.Now, ProdutivityLevel =  2};
            db.Insert(p_Data);
        }

        void MediocreClicked(object sender, EventArgs e)
        {
            b_Mediocre.Text = "3";
            ProductiveData p_Data = new ProductiveData { Date = DateTime.Now, ProdutivityLevel =  3};
            db.Insert(p_Data);
        }

        void PoorClicked(object sender, EventArgs e)
        {
            b_Poor.Text = "4";
            ProductiveData p_Data = new ProductiveData { Date = DateTime.Now, ProdutivityLevel =  4};
            db.Insert(p_Data);
        }

        void TerribleClicked(object sender, EventArgs e)
        {
            b_Terrible.Text = "5";
            ProductiveData p_Data = new ProductiveData { Date = DateTime.Now, ProdutivityLevel =  5};
            db.Insert(p_Data);
        }

        // --------------------------------------------

        void GraphClicked(object sender, EventArgs e)
        {
            b_Graph.Text = "G";

            Console.WriteLine("Reading data");
            var table = db.Table<ProductiveData>();
            string stringDBTest = "";
            foreach (var s in table)
            {
                stringDBTest += s.DataNum+": "+s.Date + " " + s.ProdutivityLevel+"\n";
            }
            t_console.Text = stringDBTest;
        }

        void SummaryClicked(object sender, EventArgs e)
        {
            db.DeleteAll<ProductiveData>();
            b_Summary.Text = "S";
        }
    }
}

