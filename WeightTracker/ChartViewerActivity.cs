using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using BarChart;
using Android.Graphics;

namespace WeightTracker
{
    public enum ChartDataType
    {
        Weight_By_Date = 0,
        Weight_By_Month,
        Size_By_Date,
        Size_By_Month,
        Total_Weight_By_Date
    }

    [Activity(Label = "Weight Tracker")]			
    public class ChartViewerActivity : Activity
    {
        private BarChartView _barChart = null;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            this.SetContentView(Resource.Layout.ChartViewer);

            // Create your application here
            this.RequestedOrientation = Android.Content.PM.ScreenOrientation.Landscape;

            var spinner = FindViewById<Spinner>(Resource.Id.BarChartDataSpinner);
            spinner.ItemSelected += BarChartDataChanged;
            var adapter = ArrayAdapter.CreateFromResource(this, Resource.Array.ChartDataArray, Android.Resource.Layout.SimpleSpinnerItem);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinner.Adapter = adapter;

            _barChart = FindViewById<BarChartView>(Resource.Id.MeasurementBarChart);
            _barChart.BarWidth = 70;
            _barChart.BarOffset = 15;

            PopulateWeightByDate();
        }

        private void BarChartDataChanged (object sender, AdapterView.ItemSelectedEventArgs e)
        {
            switch(e.Position)
            {
                case (int)ChartDataType.Weight_By_Month:
                    PopulateWeightByMonth();
                    break;
                case (int)ChartDataType.Weight_By_Date:
                    PopulateWeightByDate();
                    break;
                case (int)ChartDataType.Size_By_Month:
                    PopulateSizeChangesByMonth();
                    break;
                case (int)ChartDataType.Size_By_Date:
                    PopulateSizeChangesByDate();
                    break;
                case (int)ChartDataType.Total_Weight_By_Date:
                    PopulateTotalWeightByDate();
                    break;
            }
        }

        private void PopulateSizeChangesByDate()
        {
            var data = BodyMeasurements.GetSizeChangesByDate();
            PopulateChartData(data);
        }

        private void PopulateSizeChangesByMonth()
        {
            var data = BodyMeasurements.GetSizeChangesByMonth();
            PopulateChartData(data);
        }

        private void PopulateWeightByDate()
        {
            var data = BodyMeasurements.GetWeightByDate();
            PopulateChartData(data);
        }

        private void PopulateWeightByMonth()
        {
            var data = BodyMeasurements.GetWeightByMonth();
            PopulateChartData(data);
        }

        private void PopulateTotalWeightByDate()
        {
            var data = BodyMeasurements.GetTotalWeightByDate();
            PopulateChartData(data, Color.Green);
        }

        private void PopulateChartData(Dictionary<string, double> data, Color barColor)
        {
            var items = new List<BarModel>();
            foreach(var d in data)
            {
                if (barColor == Color.Transparent)
                    barColor = (d.Value <= 0) ? Color.Green : Color.Red;

                var bar = new BarModel {
                    Value = (float)d.Value,
                    Color = barColor,
                    Legend = d.Key,
                    ValueCaptionHidden = false,
                    ValueCaption = d.Value.ToString()
                };
                items.Add(bar);
            }

            CreateBarChart(items);
        }

        private void PopulateChartData(Dictionary<string, double> data)
        {
            PopulateChartData(data, Color.Transparent);
        }

        private void CreateBarChart(List<BarModel> items)
        {
            var chart = new BarChartView (this) {
                ItemsSource = items,
                BarCaptionFontSize = 22,
                BarWidth = 70,
                BarOffset = 35,
                LegendFontSize = 18
            };

            var layout = FindViewById<LinearLayout>(Resource.Id.BarChartLinearLayout);

            if (layout.ChildCount > 0)
                layout.RemoveAllViewsInLayout();

            layout.AddView(chart, new ViewGroup.LayoutParams (
                ViewGroup.LayoutParams.FillParent, ViewGroup.LayoutParams.FillParent));
        }
    }
}

