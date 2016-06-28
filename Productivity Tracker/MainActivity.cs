using System;
using System.IO;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Diagnostics;
using Android.OS;
using SQLite;

namespace Productivity_Tracker
{
    [Activity(Label = "Productivity_Tracker", MainLauncher = true)]
    public class MainActivity : Activity
    {
        private static string dbName = "db.sqlite";

#if DEBUG
        public static string dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), dbName);
#else
        public static string dbPath = Path.Combine(Android.OS.Environment.ExternalStorageDirectory.ToString(), dbName);
#endif

        Button b_Awesome, b_Good, b_Mediocre, b_Poor, b_Terrible;
        Button b_Summary, b_RawData, b_Clear;

        TextView t_console;

        Button b_BackSummary;

        TextView t_Summary;

        Button b_BackRaw;

        TextView t_RawData;

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

        protected override void OnPause()
        {
            base.OnPause();
            CreateNotification();
        }

        void LoadMain()
        {
            b_Awesome = FindViewById<Button>(Resource.Id.buttonAwesome);
            b_Good = FindViewById<Button>(Resource.Id.buttonGood);
            b_Mediocre = FindViewById<Button>(Resource.Id.buttonMediocre);
            b_Poor = FindViewById<Button>(Resource.Id.buttonPoor);
            b_Terrible = FindViewById<Button>(Resource.Id.buttonTerrible);

            b_RawData = FindViewById<Button>(Resource.Id.buttonRawData);
            b_Summary = FindViewById<Button>(Resource.Id.buttonSummary);
            b_Clear = FindViewById<Button>(Resource.Id.buttonClear);

            t_console = FindViewById<TextView>(Resource.Id.textFeeling);

            b_Awesome.Click += AwesomeClicked;
            b_Good.Click += GoodClicked;
            b_Mediocre.Click += MediocreClicked;
            b_Poor.Click += PoorClicked;
            b_Terrible.Click += TerribleClicked;

            b_Summary.Click += SummaryClicked;
            b_RawData.Click += RawDataClicked;
            b_Clear.Click += ClearClicked;

            var database = db.Table<ProductiveData>();
            foreach (var dataPoint in database)
            {
                if (dataPoint.DateHour == DateTime.Now.Hour && dataPoint.DateDay == DateTime.Now.Day && dataPoint.DateMonth == DateTime.Now.Month)
                {
                    DisbleButtons();
                }
            }
        }

        void CreateNotification()
        {
            var alarmIntent = new Intent(this, typeof(AlarmReceiver));

            var pending = PendingIntent.GetBroadcast(this, 0, alarmIntent, PendingIntentFlags.UpdateCurrent);

            var alarmManager = GetSystemService(AlarmService).JavaCast<AlarmManager>();
            alarmManager.Set(AlarmType.ElapsedRealtime, SystemClock.ElapsedRealtime() + 60 * 2 * 1000, pending);//notify in one hour
        }

        void LoadSummary()
        {
            b_BackSummary = FindViewById<Button>(Resource.Id.buttonBackSummary);
            t_Summary = FindViewById<TextView>(Resource.Id.textSummary);

            b_BackSummary.Click += BackClicked;
        }

        void LoadRawData()
        {
            b_BackRaw = FindViewById<Button>(Resource.Id.buttonBackRaw);
            t_RawData = FindViewById<TextView>(Resource.Id.textRawdata);
            var database = db.Table<ProductiveData>();
            string dbAllData = "";

            foreach (var dataPoint in database)
            {
                dbAllData += "Date: "+dataPoint.DateMonth+"/"+dataPoint.DateDay+" Time: "+dataPoint.DateHour + " Productivity Level: " + dataPoint.ProdutivityLevel+"\n";
            }

            t_RawData.Text = dbAllData;

            b_BackRaw.Click += BackClicked;
        }

        void AwesomeClicked(object sender, EventArgs e)
        {
            ProductiveData p_Data = new ProductiveData { DateHour = DateTime.Now.Hour, DateDay = DateTime.Now.Day, DateMonth = DateTime.Now.Month, ProdutivityLevel = 5 };
            db.Insert(p_Data);
            DisbleButtons();
        }

        void GoodClicked(object sender, EventArgs e)
        {
            ProductiveData p_Data = new ProductiveData { DateHour = DateTime.Now.Hour, DateDay = DateTime.Now.Day, DateMonth = DateTime.Now.Month, ProdutivityLevel = 4 };
            DisbleButtons();
        }

        void MediocreClicked(object sender, EventArgs e)
        {
            ProductiveData p_Data = new ProductiveData { DateHour = DateTime.Now.Hour, DateDay = DateTime.Now.Day, DateMonth = DateTime.Now.Month, ProdutivityLevel = 3 };
            db.Insert(p_Data);
            DisbleButtons();
        }

        void PoorClicked(object sender, EventArgs e)
        {
            ProductiveData p_Data = new ProductiveData { DateHour = DateTime.Now.Hour, DateDay = DateTime.Now.Day, DateMonth = DateTime.Now.Month, ProdutivityLevel = 2 };
            db.Insert(p_Data);
            DisbleButtons();
        }

        void TerribleClicked(object sender, EventArgs e)
        {
            ProductiveData p_Data = new ProductiveData { DateHour = DateTime.Now.Hour, DateDay = DateTime.Now.Day, DateMonth = DateTime.Now.Month, ProdutivityLevel = 1 };
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

        void SummaryClicked(object sender, EventArgs e)
        {
            SetContentView(Resource.Layout.Summary);
            LoadSummary();

            Tuple<int, int>[] productivityHour = new Tuple<int, int>[24];
            int productiveDataPoints = 0, productivityLevelTotalHour = 0;

            var database = db.Table<ProductiveData>();
            for (int i = 0; i < productivityHour.Length; i++)
            {
                productivityHour[i] = new Tuple<int, int>(0, 0);//(number of data points, productivity level)
            }

            foreach (var dataPoint in database)
            {
                productiveDataPoints = productivityHour[dataPoint.DateHour].Item1 + 1;
                productivityLevelTotalHour = productivityHour[dataPoint.DateHour].Item2 + dataPoint.ProdutivityLevel;
                productivityHour[dataPoint.DateHour] = new Tuple<int, int>(productiveDataPoints, productivityLevelTotalHour);
            }
            List<Tuple<int, float>> bestProductivityTimes = CalculateBestTimes(CalculateAverage(productivityHour));

            string summaryText = "The times your are most productive are:\n";
            for(int i=0; i<bestProductivityTimes.Count; i++)
            {
                if (bestProductivityTimes[i].Item1 > 12)
                {
                    summaryText += bestProductivityTimes[i].Item1 - 12 + "pm \n";
                } else
                {
                    summaryText += bestProductivityTimes[i].Item1 + "am \n";
                }
            }
            t_Summary.Text = summaryText;
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

        void RawDataClicked(object sender, EventArgs e)
        {
            SetContentView(Resource.Layout.Raw_Data);
            LoadRawData();
        }

        float[] CalculateAverage(Tuple<int, int>[] prodHours)
        {
            float[] prodHoursAverage = new float[24];
            for (int i = 0; i < prodHours.Length; i++)
            {
                if (prodHours[i].Item1 != 0)
                {
                    prodHoursAverage[i] = (float)prodHours[i].Item2 / (float)prodHours[i].Item1;
                }
            }
            return prodHoursAverage;
        }

        List<Tuple<int, float>> CalculateBestTimes(float[] prodHoursAverages)
        {
            float max = 0;
            List<Tuple<int, float>> bestProdTimes = new List<Tuple<int, float>>();
            for (int i = 0; i < prodHoursAverages.Length; i++)
            {
                if (max < prodHoursAverages[i])
                {
                    max = prodHoursAverages[i];
                }
            }

            for (int hour = 0; hour < prodHoursAverages.Length; hour++)
            {
                if (prodHoursAverages[hour] >= max - 1)
                {
                    bestProdTimes.Add(new Tuple<int, float>(hour, prodHoursAverages[hour]));
                }
            }
            return bestProdTimes;
        }
    }
}

