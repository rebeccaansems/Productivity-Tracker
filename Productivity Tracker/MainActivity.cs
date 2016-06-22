using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace Productivity_Tracker
{
    [Activity(Label = "Productivity_Tracker", MainLauncher = true)]
    public class MainActivity : Activity
    {

        Button b_Awesome, b_Good, b_Mediocre, b_Poor, b_Terrible;
        Button b_Graph, b_Summary;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

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
            b_Awesome.Text = "1";
        }

        void GoodClicked(object sender, EventArgs e)
        {
            b_Good.Text = "2";
        }

        void MediocreClicked(object sender, EventArgs e)
        {
            b_Mediocre.Text = "3";
        }

        void PoorClicked(object sender, EventArgs e)
        {
            b_Poor.Text = "4";
        }

        void TerribleClicked(object sender, EventArgs e)
        {
            b_Terrible.Text = "5";
        }

        // --------------------------------------------

        void GraphClicked(object sender, EventArgs e)
        {
            b_Graph.Text = "G";
        }

        void SummaryClicked(object sender, EventArgs e)
        {
            b_Summary.Text = "S";
        }
    }
}

