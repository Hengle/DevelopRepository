using System;

namespace Util {
	public static class DateTimeUtility {
		public static int ToYMDIntTime (DateTime date) {
			return date.Year * 10000 + date.Month * 100 + date.Day;
		}

		public static bool IsOverHours (DateTime target, int hours) {
			return (DateTime.Now - target).Hours >= hours;
		}
	}
}
