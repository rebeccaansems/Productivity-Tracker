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
        Button b_TimeMin, b_TimeMax, b_Clear, b_RemoveLastPoint;
        TextView t_TimeMin, t_TimeMax;

        int hourMin = 8, hourMax = 12 + 10;
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

        // --------------------------------------------
        // ------------- Load different views

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
            hourMax = 12 + 9;
            minuteMin = 0;
            minuteMax = 0;

            //clear button
            b_Clear = FindViewById<Button>(Resource.Id.b_Clear);
            b_RemoveLastPoint = FindViewById<Button>(Resource.Id.b_RemoveLastDataPoint);

            b_Clear.Click += ClearClicked;
            b_RemoveLastPoint.Click += RemoveLastPointClicked;

            //if database is empty then don't allow clearance
            if (db.Table<ProductiveData>().Count() == 0)
            {
                b_Clear.Enabled = false;
                b_RemoveLastPoint.Enabled = false;
            }

            UpdateTimes();
        }

        // --------------------------------------------
        // ------------- Notifications

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

        // --------------------------------------------
        // ------------- Productivity buttons

        void AwesomeClicked(object sender, EventArgs e)
        {
            ProductiveData p_Data = new ProductiveData { DateHour = DateTime.Now.Hour, DateDay = DateTime.Now.Day, DateMonth = DateTime.Now.Month, ProdutivityLevel = 5 };
            db.Insert(p_Data);
            DisbleButtons();
        }

        void GoodClicked(object sender, EventArgs e)
        {
            ProductiveData p_Data = new ProductiveData { DateHour = DateTime.Now.Hour, DateDay = DateTime.Now.Day, DateMonth = DateTime.Now.Month, ProdutivityLevel = 4 };
            db.Insert(p_Data);
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

        void DisbleButtons()
        {
            b_Awesome.Enabled = false;
            b_Good.Enabled = false;
            b_Mediocre.Enabled = false;
            b_Poor.Enabled = false;
            b_Terrible.Enabled = false;
        }

        // --------------------------------------------
        // ------------- View buttons (in footer) clicked

        void MainClicked(object sender, EventArgs e)
        {
            SetContentView(Resource.Layout.Main);
            LoadMain();
            LoadFooter();
        }

        void MainClicked()
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
            //fill 24 instance(hour) array with zero'd tuples 
            for (int i = 0; i < productivityHour.Length; i++)
            {
                productivityHour[i] = new Tuple<int, int>(0, 0);//(number of data points, productivity level)
            }

            int numberOfDataPoints = 0;
            foreach (var dataPoint in database)
            {
                productiveDataPoints = productivityHour[dataPoint.DateHour].Item1 + 1;
                productivityLevelTotalHour = productivityHour[dataPoint.DateHour].Item2 + dataPoint.ProdutivityLevel;
                productivityHour[dataPoint.DateHour] = new Tuple<int, int>(productiveDataPoints, productivityLevelTotalHour);
                numberOfDataPoints++;
            }

            if (numberOfDataPoints > 20)
            {
                Tuple<List<Tuple<int, float>>, List<Tuple<int, float>>> bestWorstProd =
                    CalculateBestWorstTimes(CalculateAverage(productivityHour));

                List<Tuple<int, float>> bestProductivityTimes = bestWorstProd.Item1;
                List<Tuple<int, float>> worstProductivityTimes = bestWorstProd.Item2;

                string summaryMostText = "";
                for (int i = 0; i < bestProductivityTimes.Count; i++)
                {
                    if (bestProductivityTimes[i].Item1 > 12)
                    {
                        summaryMostText += bestProductivityTimes[i].Item1 - 12 + "pm \n";
                    }
                    else if (bestProductivityTimes[i].Item1 == 0)
                    {
                        summaryMostText += 12 + "am \n";
                    }
                    else if (bestProductivityTimes[i].Item1 == 12)
                    {
                        summaryMostText += 12 + "pm \n";
                    }
                    else
                    {
                        summaryMostText += bestProductivityTimes[i].Item1 + "am \n";
                    }
                }

                string summaryWorstText = "";
                for (int i = 0; i < worstProductivityTimes.Count; i++)
                {
                    if (worstProductivityTimes[i].Item1 > 12)
                    {
                        summaryWorstText += worstProductivityTimes[i].Item1 - 12 + "pm \n";
                    }
                    else if (worstProductivityTimes[i].Item1 == 0)
                    {
                        summaryWorstText += 12 + "am \n";
                    }
                    else if (worstProductivityTimes[i].Item1 == 12)
                    {
                        summaryWorstText += 12 + "pm \n";
                    }
                    else
                    {
                        summaryWorstText += worstProductivityTimes[i].Item1 + "am \n";
                    }
                }
                t_SummaryMost.Text = summaryMostText;
                t_SummaryLeast.Text = summaryWorstText;
            }
            else
            {
                t_SummaryMost.Text = "To ensure accuracy, you have to have a minumum of 20 data points.";
                t_SummaryLeast.Text = "Currently you have " + numberOfDataPoints + " data point/s.";
            }
        }

        // --------------------------------------------
        // ------------- Other

        //delete the database
        void ClearClicked(object sender, EventArgs e)
        {
            db.DeleteAll<ProductiveData>();
            b_Clear.Enabled = false;
            MainClicked();
        }

        //delete last data point in database
        void RemoveLastPointClicked(object sender, EventArgs e)
        {
            var database = db.Table<ProductiveData>();
            var lastData = new ProductiveData();

            //find last data point
            foreach (var dataPoint in database)
            {
                lastData = dataPoint;
            }

            db.Delete<ProductiveData>(lastData.DataNum);
            MainClicked();
        }

        //calculate the average of an array of tuples
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

        //calculates the best and worst times for productivity based on averages
        Tuple<List<Tuple<int, float>>, List<Tuple<int, float>>> CalculateBestWorstTimes(float[] prodHoursAverages)
        {
            float max = 0;
            float min = 100;

            List<Tuple<int, float>> bestProdTimes = new List<Tuple<int, float>>();
            List<Tuple<int, float>> worstProdTimes = new List<Tuple<int, float>>();

            for (int i = 0; i < prodHoursAverages.Length; i++)
            {
                if (max < prodHoursAverages[i])
                {
                    max = prodHoursAverages[i];
                }

                if (min > prodHoursAverages[i] && min != 0)
                {
                    min = prodHoursAverages[i];
                }
            }

            for (int hour = 0; hour < prodHoursAverages.Length; hour++)
            {
                if (prodHoursAverages[hour] >= max - 0.5f && prodHoursAverages[hour] != 0)
                {
                    bestProdTimes.Add(new Tuple<int, float>(hour, prodHoursAverages[hour]));
                }
                else if (prodHoursAverages[hour] <= min + 0.5f && prodHoursAverages[hour] != 0)
                {
                    worstProdTimes.Add(new Tuple<int, float>(hour, prodHoursAverages[hour]));
                }
            }
            Tuple<List<Tuple<int, float>>, List<Tuple<int, float>>> bestWorstProdTimes =
                new Tuple<List<Tuple<int, float>>, List<Tuple<int, float>>>(bestProdTimes, worstProdTimes);
            return bestWorstProdTimes;
        }

        //callback for earliest notification time (Options Menu)
        private void TimePickerCallbackMinimum(object sender, TimePickerDialog.TimeSetEventArgs e)
        {
            hourMin = e.HourOfDay;
            minuteMin = e.Minute;
            UpdateTimes();
        }

        //callback for latest notification time (Options Menu)
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

        //update display times (Options Menu)
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

            string timeMin = ConvertTime("Notification start time: ", hourMin, minuteMin);
            string timeMax = ConvertTime("Notification end time: ", hourMax, minuteMax);

            t_TimeMin.Text = timeMin;
            t_TimeMax.Text = timeMax;
        }

        //opens time selection screen (Options Menu)
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

