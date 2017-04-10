using System;

namespace WeightTracker
{
	public static class Extensions
	{
		public static double AsDouble(this string text)
		{
			double value = 0.0;
			double.TryParse (text, out value);
			return value;
		}
	}
}

