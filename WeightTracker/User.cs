using System;
using System.Linq;
using SQLite;
using System.Collections.Generic;

namespace WeightTracker
{
	public class User
	{
		[PrimaryKey, AutoIncrement]
		public int ID { get; set; }

		public string UserName { get; set; }

		public User ()
		{
		}

		public static User CreateNewUser(string userName)
		{
			return new User () { UserName = userName };
		}

		public static List<User> GetUsers()
		{
			List<User> users = new List<User> ();
			using (SQLiteConnection conn = new SQLiteConnection(Utilities.DatabasePath))
			{
				users = conn.Table<User> ().ToList ();
			}
			return users;
		}

		public void Save()
		{
			using (SQLiteConnection conn = new SQLiteConnection(Utilities.DatabasePath))
			{
				var users = conn.Table<User> ().Where (t => t.ID == this.ID).ToList();
				if (users.Count == 0)
					conn.Insert (this);
				else
					conn.Update (this);
			}
		}
	}
}

