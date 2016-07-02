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
    [Activity(Label = "Productivity Tracker", MainLauncher = true)]
    public class MainActivity : Activity
    {
        private static string dbName = "db.sqlite";
        SQLiteConnection db;

#if DEBUG
        public static string dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), dbName);
#else
        public static string dbPath = Path.Combine(Android.OS.Environment.ExternalStorageDirectory.ToString(), dbName);
#endif
        //General
        Button b_Summary, b_Main, b_Options;
        //Summary
        TextView t_SummaryMost, t_SummaryLeast;
        //Main
        Button b_Awesome, b_Good, b_Mediocre, b_Poor, b_Terrible;
        //Options
        Button b_Clear, b_TimeMin, b_TimeMax;
        TextView t_TimeMin, t_TimeMax;

        int hourMin = 8, hourMax = 12+10;
        int minuteMin, minuteMax;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            db = new SQLiteConnection(MainActivity.dbPath);
            db.CreateTable<ProductiveData>();

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            LoadMain();
            LoadFooter();
        }

        protected override void OnPause()
        {
            base.OnPause();
            CreateNotification();
        }

        void LoadFooter()
        {
            b_Summary = FindViewById<Button>(Resource.Id.buttonSummary);
            b_Main = FindViewById<Button>(Resource.Id.buttonMain);
            b_Options = FindViewById<Button>(Resource.Id.buttonOptions);

            b_Summary.Click += SummaryClicked;
            b_Main.Click += MainClicked;
            b_Options.Click += OptionsClicked;
        }

        void LoadSummary()
        {
            t_SummaryMost = FindViewById<TextView>(Resource.Id.textSummaryMost);
            t_SummaryLeast = FindViewById<TextView>(Resource.Id.textSummaryLeast);
        }

        void LoadMain()
        {
            b_Awesome = FindViewById<Button>(Resource.Id.buttonAwesome);
            b_Good = FindViewById<Button>(Resource.Id.buttonGood);
            b_Mediocre = FindViewById<Button>(Resource.Id.buttonMediocre);
            b_Poor = FindViewById<Button>(Resource.Id.buttonPoor);
            b_Terrible = FindViewById<Button>(Resource.Id.buttonTerrible);

            b_Awesome.Click += AwesomeClicked;
            b_Good.Click += GoodClicked;
            b_Mediocre.Click += MediocreClicked;
            b_Poor.Click += PoorClicked;
            b_Terrible.Click += TerribleClicked;

            var database = db.Table<ProductiveData>();
            foreach (var dataPoint in database)
            {
                if (dataPoint.DateHour == DateTime.Now.Hour && dataPoint.DateDay == DateTime.Now.Day && dataPoint.DateMonth == DateTime.Now.Month)
                {
                    DisbleButtons();
                }
            }
        }

        void LoadOptions()
        {
            //Min/max times
            t_TimeMax = FindViewById<TextView>(Resource.Id.t_TimeMax);
            t_TimeMin = FindViewById<TextView>(Resource.Id.t_TimeMin);
            b_TimeMax = FindViewById<Button>(Resource.Id.b_TimeMax);
            b_TimeMin = FindViewById<Button>(Resource.Id.b_TimeMin);

            // Add a click listener to the button
            b_TimeMin.Click += (o, e) => ShowDialog(0);
            b_TimeMax.Click += (o, e) => ShowDialog(1);

            //Set min/max times
            hourMin = 8;
            hourMax = 12 + 10;
            minuteMin = 0;
            minuteMax = 0;

            //clear button
            b_Clear = FindViewById<Button>(Resource.Id.b_Clear);

            b_Clear.Click += ClearClicked;

            UpdateTimes();
        }

        void CreateNotification()
        {
            var alarmIntent = new Intent(this, typeof(AlarmReceiver));

            var pending = PendingIntent.GetBroadcast(this, 0, alarmIntent, PendingIntentFlags.UpdateCurrent);

            var alarmManager = GetSystemService(AlarmService).JavaCast<AlarmManager>();
            long timeDifference = SystemClock.ElapsedRealtime() + 60 * 60 * 60 * 1000;//One hour
            if (DateTime.Now.Hour + 1 > hourMax)
            {
                timeDifference = SystemClock.ElapsedRealtime() + (long)new DateTime(2010, 1, 2, hourMin, minuteMin, 0).Subtract(new DateTime(2010, 1, 1, hourMax, minuteMax, 0)).TotalMilliseconds;
            }
            alarmManager.Set(AlarmType.ElapsedRealtime, timeDifference, pending);
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

        void MainClicked(object sender, EventArgs e)
        {
            SetContentView(Resource.Layout.Main);
            LoadMain();
            LoadFooter();
        }

        void OptionsClicked(object sender, EventArgs e)
        {
            SetContentView(Resource.Layout.Options);
            LoadOptions();
            LoadFooter();
        }

        void SummaryClicked(object sender, EventArgs e)
        {
            SetContentView(Resource.Layout.Summary);
            LoadSummary();
            LoadFooter();

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

            string summaryMostText = "";
            for (int i = 0; i < bestProductivityTimes.Count; i++)
            {
                if (bestProductivityTimes[i].Item1 > 12)
                {
                    summaryMostText += bestProductivityTimes[i].Item1 - 12 + "pm \n";
                }
                else
                {
                    summaryMostText += bestProductivityTimes[i].Item1 + "am \n";
                }
            }

            t_SummaryMost.Text = summaryMostText;
        }

        void ClearClicked(object sender, EventArgs e)
        {
            db.DeleteAll<ProductiveData>();
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

        private void TimePickerCallbackMinimum(object sender, TimePickerDialog.TimeSetEventArgs e)
        {
            hourMin = e.HourOfDay;
            minuteMin = e.Minute;
            UpdateTimes();
        }

        private void TimePickerCallbackMaximum(object sender, TimePickerDialog.TimeSetEventArgs e)
        {
            hourMax = e.HourOfDay;
            minuteMax = e.Minute;
            UpdateTimes();
        }

        //Conert time from 24 hour clock to 12 hour clock
        string ConvertTime(string openingStatement, int hour, int min)
        {
            bool isPM = false;

            if (hour > 12)
            {
                isPM = true;
                hour -= 12;
            }
            else if (hour == 0)
            {
                hour = 12;
            }
            else if (hour == 12)
            {
                isPM = true;
            }
            if (isPM)
            {
                return string.Format(openingStatement + "{0}:{1}PM", hour, min.ToString().PadLeft(2, '0'));
            }
            return string.Format(openingStatement + "{0}:{1}AM", hour, min.ToString().PadLeft(2, '0'));
        }

        void UpdateTimes()
        {
            DateTime dateTimeMin = new DateTime(2016, 1, 1, hourMin, minuteMin, 0);
            DateTime dateTimeMax = new DateTime(2016, 1, 1, hourMax, minuteMax, 0);

            if (dateTimeMax < dateTimeMin)
            {
                int tempHour = hourMin;
                int tempMinute = minuteMin;

                hourMin = hourMax;
                minuteMin = minuteMax;
                hourMax = tempHour;
                minuteMax = tempMinute;
            }

            dateTimeMin = new DateTime(2016, 1, 1, hourMin, minuteMin, 0);
            dateTimeMax = new DateTime(2016, 1, 1, hourMax, minuteMax, 0);

            string timeMin = ConvertTime("Day start time: ", hourMin, minuteMin);
            string timeMax = ConvertTime("Day end time: ", hourMax, minuteMax);
            
            t_TimeMin.Text = timeMin;
            t_TimeMax.Text = timeMax;
        }

        const int minID = 0, maxID = 1;
        protected override Dialog OnCreateDialog(int id)
        {
            if (id == minID)
            {
                return new TimePickerDialog(this, TimePickerCallbackMinimum, hourMin, minuteMin, false);
            }
            else if (id == maxID)
            {
                return new TimePickerDialog(this, TimePickerCallbackMaximum, hourMax, minuteMax, false);
            }

            return null;
        }
    }
}

