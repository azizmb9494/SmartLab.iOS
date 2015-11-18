using System;
using Foundation;
using UIKit;

namespace SmartLabToday
{
	public static class KeyStore
	{
		private static NSUserDefaults _Defaults = new NSUserDefaults ("group.edu.usf.smartlab", NSUserDefaultsType.SuiteName);
		public static string Get(string key)
		{
			return _Defaults.StringForKey (key);
		}

		public static void Set(string key, string value)
		{
			_Defaults.SetString (value, key);
		}

		public static bool BizCalcOnly
		{
			get {
				return _Defaults.BoolForKey("BizCalcOnly");
			}
			set {
				_Defaults.SetBool (value, "BizCalcOnly");
			}
		}

		public static bool HideBizCalc
		{
			get {
				return _Defaults.BoolForKey("HideBizCalc");
			}
			set {
				_Defaults.SetBool (value, "HideBizCalc");
			}
		}
	}
}

