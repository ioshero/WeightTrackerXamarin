using System;
using SQLite;

namespace WeightTracker
{
	public class Settings
	{
		[PrimaryKey, AutoIncrement]
		public int ID { get; set; }

		public int CurrentUserID { get; set; }

		public Settings ()
		{
		}

		public User GetCurrentUser()
		{
			User user = null;
			using (SQLiteConnection conn = new SQLiteConnection(Utilities.DatabasePath))
			{
				user = conn.Table<User> ().Where (t => t.ID == this.CurrentUserID).FirstOrDefault();
			}

			return user;
		}

		public static Settings LoadSettings()
		{
			Settings setting = null;
			using (SQLiteConnection conn = new SQLiteConnection(Utilities.DatabasePath))
			{
				setting = conn.Table<Settings> ().FirstOrDefault();
			}

			return setting;
		}

		public static Settings CreateNew(int defaultUserID)
		{
			return new Settings () { CurrentUserID = defaultUserID };
		}

		public void Save()
		{
			using (SQLiteConnection conn = new SQLiteConnection(Utilities.DatabasePath))
			{
				conn.InsertOrReplace (this);
			}
		}
	}
}

