using System;
using System.IO;
using SQLite;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Xml;
using System.Text;

namespace WeightTracker
{
	public static class Utilities
	{
		public static string ApplicationDirectory = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath + "/WeightTracker";
		public static string DatabasePath = Utilities.ApplicationDirectory + "/weight.db3";
        public static string ExportedDatabasePath = Utilities.ApplicationDirectory + "/weight.xml";

        public const int LINK_TO_DROPBOX_REQUEST = 1000;

		public static void CreateApplicationDirectory()
		{
			if (!Directory.Exists (Utilities.ApplicationDirectory))
				Directory.CreateDirectory (Utilities.ApplicationDirectory);
		}

		public static void CreateTables()
		{
			using (SQLiteConnection conn = new SQLiteConnection(Utilities.DatabasePath))
			{
				conn.CreateTable<BodyMeasurements> ();
				conn.CreateTable<User> ();
				conn.CreateTable<Settings> ();

				if (conn.Table<User>().Count() == 0)
				{
					conn.Insert (User.CreateNewUser ("User 1"));
				}

				if (conn.Table<Settings>().Count() == 0)
				{
					conn.Insert (Settings.CreateNew (conn.Table<User> ().First ().ID));
				}
			}
		}

        public static void ExportMeasurements(List<BodyMeasurements> measurements)
        {
            Serialize(measurements, Utilities.ExportedDatabasePath);
        }

        public static void Serialize<T>(T dataToSerialize, string filePath)
        {
            try
            {
                using (Stream stream = File.Open(filePath, FileMode.Create, FileAccess.ReadWrite))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(T));
                    XmlTextWriter writer = new XmlTextWriter(stream, Encoding.Default);
                    writer.Formatting = Formatting.Indented;
                    serializer.Serialize(writer, dataToSerialize);
                    writer.Close();
                }
            }
            catch
            {
                throw;
            }
        }

//        public static T Deserialize<T>(string xml)
//        {
//            T deserializedObject;
//            try
//            {
//                XmlSerializer ser = new XmlSerializer(typeof(T));
//                StringReader sr = new StringReader(xml);
//                XmlReader xr = XmlReader.Create (sr);
//
//                deserializedObject = (T)ser.Deserialize (xr);
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine(ex.Message);
//                return default(T);
//            }
//
//            return deserializedObject;
//        }

        public static T Deserialize<T>(string filePath)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                T serializedData;

                using (Stream stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
                {
                    serializedData = (T)serializer.Deserialize(stream);
                }

                return serializedData;
            }
            catch
            {
                throw;
            }
        }
	}
}

