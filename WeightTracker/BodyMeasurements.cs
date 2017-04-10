using System;
using System.Linq;
using SQLite;
using System.Collections.Generic;

namespace WeightTracker
{
	public class BodyMeasurements : Java.Lang.Object
	{
		[PrimaryKey, AutoIncrement]
		public int ID { get; set; }

		public int UserID { get; set; }

		public DateTime Date { get; set; }
		public double Weight { get; set; }
		public double LeftArm { get; set; }
		public double RightArm { get; set; }
		public double Chest { get; set; }
		public double Waist { get; set; }
		public double Abdominal { get; set; }
		public double Hips { get; set; }
		public double LeftThigh { get; set; }
		public double RightThigh { get; set; }
		public double LeftCalf { get; set; }
		public double RightCalf { get; set; }
        public double TotalSizeChange { get; set; }
        public double TotalWeightChange { get; set; }

		public BodyMeasurements ()
		{
		}

		public void Save()
		{
			using (SQLiteConnection conn = new SQLiteConnection(Utilities.DatabasePath))
			{
				conn.Insert (this);
			}
		}

        public void Delete()
        {
            using (SQLiteConnection conn = new SQLiteConnection(Utilities.DatabasePath))
            {
                conn.Delete(this);
            }
        }

		public static List<BodyMeasurements> GetAllMeasurements()
		{
			List<BodyMeasurements> list = new List<BodyMeasurements> ();
			var settings = Settings.LoadSettings ();
			using (SQLiteConnection conn = new SQLiteConnection(Utilities.DatabasePath))
			{
				list = conn.Table<BodyMeasurements> ().Where(t => t.UserID == settings.CurrentUserID).OrderByDescending (t => t.Date).ToList();
			}

			return list;
		}

        public static BodyMeasurements GetLastMeasurement(DateTime date)
		{
			BodyMeasurements bod = null;
			var settings = Settings.LoadSettings ();

			using (SQLiteConnection conn = new SQLiteConnection(Utilities.DatabasePath))
			{
				bod = conn.Table<BodyMeasurements> ().Where(t => t.UserID == settings.CurrentUserID && t.Date < date).OrderByDescending (t => t.Date).FirstOrDefault ();
			}

			return bod;
		}

        public static BodyMeasurements GetFirstMeasurement()
        {
			BodyMeasurements bod = null;
			var settings = Settings.LoadSettings ();

            using (SQLiteConnection conn = new SQLiteConnection(Utilities.DatabasePath))
            {
				bod = conn.Table<BodyMeasurements> ().Where(t => t.UserID == settings.CurrentUserID).OrderBy (t => t.ID).FirstOrDefault ();
            }

            return bod;
        }

        public static Dictionary<string, double> GetWeightByDate()
        {
            var dict = new Dictionary<string, double>();
			var data = new List<BodyMeasurements>();
			var settings = Settings.LoadSettings ();

            using (SQLiteConnection conn = new SQLiteConnection(Utilities.DatabasePath))
            {
				data = conn.Table<BodyMeasurements>().Where(t => t.UserID == settings.CurrentUserID).OrderBy(t => t.Date).ToList();
            }

            foreach(var d in data)
            {
                dict[d.Date.ToString("MM-dd-yyyy")] = d.TotalWeightChange;
            }

            return dict;
        }

        public static Dictionary<string, double> GetWeightByMonth()
        {
            var dict = new Dictionary<string, double>();
			var data = new List<BodyMeasurements>();
			var settings = Settings.LoadSettings ();

            using (SQLiteConnection conn = new SQLiteConnection(Utilities.DatabasePath))
            {
				data = conn.Table<BodyMeasurements>().Where(t => t.UserID == settings.CurrentUserID).OrderBy(t => t.Date).ToList();
            }

            var monthAndWeight = data.GroupBy(k => k.Date.Month).Select(lg => new { Month = lg.Key, Total = lg.Sum(t => t.TotalWeightChange)});

            foreach(var mw in monthAndWeight)
            {
                var monthName = new DateTime(2012, mw.Month, 1).ToString("MMMM");
                dict[monthName] = mw.Total;
            }

            return dict;
        }

        public static Dictionary<string, double> GetSizeChangesByDate()
        {
            var dict = new Dictionary<string, double>();
			var data = new List<BodyMeasurements>();
			var settings = Settings.LoadSettings ();

            using (SQLiteConnection conn = new SQLiteConnection(Utilities.DatabasePath))
            {
				data = conn.Table<BodyMeasurements>().Where(t => t.UserID == settings.CurrentUserID).OrderBy(t => t.Date).ToList();
            }

            foreach(var d in data)
            {
                dict[d.Date.ToString("MM-dd-yyyy")] = d.TotalSizeChange;
            }

            return dict;
        }

        public static Dictionary<string, double> GetSizeChangesByMonth()
        {
            var dict = new Dictionary<string, double>();
			var data = new List<BodyMeasurements>();
			var settings = Settings.LoadSettings ();

            using (SQLiteConnection conn = new SQLiteConnection(Utilities.DatabasePath))
            {
				data = conn.Table<BodyMeasurements>().Where(t => t.UserID == settings.CurrentUserID).OrderBy(t => t.Date).ToList();
            }

            var monthAndWeight = data.GroupBy(k => k.Date.Month).Select(lg => new { Month = lg.Key, Total = lg.Sum(t => t.TotalSizeChange)});

            foreach(var mw in monthAndWeight)
            {
                var monthName = new DateTime(2012, mw.Month, 1).ToString("MMMM");
                dict[monthName] = mw.Total;
            }

            return dict;
        }

        public static Dictionary<string, double> GetTotalWeightByDate()
        {
            var dict = new Dictionary<string, double>();
			var data = new List<BodyMeasurements>();
			var settings = Settings.LoadSettings ();

            using (SQLiteConnection conn = new SQLiteConnection(Utilities.DatabasePath))
            {
				data = conn.Table<BodyMeasurements>().Where(t => t.UserID == settings.CurrentUserID).OrderBy(t => t.Date).ToList();
            }

            foreach(var d in data)
            {
                dict[d.Date.ToString("MM-dd-yyyy")] = d.Weight;
            }

            return dict;
        }
	}
}

