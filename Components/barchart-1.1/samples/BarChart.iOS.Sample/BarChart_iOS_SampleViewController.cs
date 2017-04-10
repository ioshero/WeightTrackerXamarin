using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using BarChart;
using System.Collections.Generic;
using MonoTouch.CoreGraphics;

namespace iOS.Sample {
	public partial class BarChart_iOS_SampleViewController : UIViewController {
		const float BarChartTopMargin = 5f;
		const float BarChartBottomMargin = 50f;
		const float BarChartHorizontalMargin = 30f;

		BarChartView barChart;

		public BarChart_iOS_SampleViewController () : base ("BarChart_iOS_SampleViewController", null)
		{
		}
		
		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
		}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			barChart = new BarChartView ();
//			barChart.MinimumValue = -50000f;
//			barChart.MaximumValue = 50000f;
//			barChart.BarColor = UIColor.Green;
			barChart.BarOffset = 2f;
			barChart.BarWidth = 40f;
			barChart.BarCaptionInnerColor = UIColor.White;
			barChart.BarCaptionInnerShadowColor = UIColor.Black;
			barChart.BarCaptionOuterColor = UIColor.Black;
			barChart.BarCaptionOuterShadowColor = UIColor.White;

			barChart.BarClick += OnBarClick;
			barChart.Frame = new RectangleF (BarChartHorizontalMargin, BarChartTopMargin, View.Bounds.Width - 2 * BarChartHorizontalMargin,
			                                 View.Bounds.Height - BarChartTopMargin - BarChartBottomMargin);

			View.AddSubview (barChart);

			barChart.ItemsSource = GenerateTestData ();

//			barChart.AddLevelIndicator (0f, "0");
//			barChart.AddLevelIndicator (100f, "100");
//			barChart.AddLevelIndicator (-100f, "-100");

//			barChart.GridHidden = true;
//			barChart.LegendHidden = true;
//			barChart.LevelsHidden = true;
		}

		void OnBarClick (object sender, BarClickEventArgs e)
		{
			Console.WriteLine ("bar clicked: name = {0}, value = {1}", e.Bar.Legend, e.Bar.Value);
		}

		partial void OnRandomButtonClicked ()
		{
			barChart.ItemsSource = GenerateTestData ();
		}

		public List<BarModel> GenerateTestData ()
		{
			var models = new List<BarModel> ();

			var rnd = new Random ((int)DateTime.UtcNow.Ticks);

			for (var i = 0; i < rnd.Next (10); i += 1) {
				models.Add (new BarModel () { Value = rnd.Next (-50000, 50000), Color = UIColor.Gray, Legend = "1H" });

				models.Add (new BarModel () { Value = rnd.Next (-50000, 50000), Color = UIColor.Brown, Legend = "6H" });

				models.Add (new BarModel () { Value = rnd.Next (-50000, 50000), Color = UIColor.Gray, Legend = "12H"});
				models.Add (new BarModel () { Value = rnd.Next (-50000, 50000), Legend = "24H"});			
				models.Add (new BarModel () { Value = rnd.Next (-50000, 50000), Legend = "32H"});
			}

			return models;
		}

		public override void ViewDidLayoutSubviews ()
		{
			base.ViewDidLayoutSubviews ();

			barChart.Frame = new RectangleF (BarChartHorizontalMargin, BarChartTopMargin, View.Bounds.Width - 2 * BarChartHorizontalMargin,
			                                 View.Bounds.Height - BarChartTopMargin - BarChartBottomMargin);
		}
	}
}