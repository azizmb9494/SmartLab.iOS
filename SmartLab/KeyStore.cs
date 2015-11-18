using System;
using Foundation;
using UIKit;

namespace SmartLab
{
	public static class KeyStore
	{
		private static NSUserDefaults _Defaults = new NSUserDefaults ("group.edu.usf.smartlab", NSUserDefaultsType.SuiteName);

		/// <summary>
		/// Get Value for specified key in defaults.
		/// </summary>
		/// <param name="key">Key.</param>
		public static string Get(string key)
		{
			return _Defaults.StringForKey (key);
		}

		/// <summary>
		/// Set value for particular key in defaults.
		/// </summary>
		/// <param name="key">Key.</param>
		/// <param name="value">Value.</param>
		public static void Set(string key, string value)
		{
			_Defaults.SetString (value, key);
		}

		/// <summary>
		/// Boolean on wether to show Business Calc requests only.
		/// </summary>
		/// <value><c>true</c> if Biz Calc only; otherwise, <c>false</c>.</value>
		public static bool BizCalcOnly
		{
			get {
				return _Defaults.BoolForKey("BizCalcOnly");
			}
			set {
				_Defaults.SetBool (value, "BizCalcOnly");
			}
		}

		/// <summary>
		/// Boolean on wether to hide Business Calc requests.
		/// </summary>
		/// <value><c>true</c> if hiding Biz Calc; otherwise, <c>false</c>.</value>
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

