using System;
using UIKit;
using Foundation;
using CoreGraphics;

namespace SmartLab
{
	/// <summary>
	/// Rectangular red icon with pod number in the middle.
	/// </summary>
	public class BlimpView : UIView
	{
		private UILabel Label;

		public BlimpView (string lbl) : base(new CGRect(0,0, 40, 40))
		{
			string pod = lbl.Substring (0, lbl.Length - 1);
			if (pod == "36" || pod == "40" || pod == "44" || pod == "47" || pod == "48") {
				this.BackgroundColor = UIColor.FromRGB (111, 38, 135);
			} else {
				this.BackgroundColor = UIColor.Red;
			}
			this.Label = new UILabel (new CGRect (0, 11, 40, 18)) {
				TextColor = UIColor.White,
				TextAlignment = UITextAlignment.Center,
				Text = lbl
			};

			this.Add (this.Label);
		}
	}
}

