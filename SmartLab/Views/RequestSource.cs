using System;
using System.Linq;
using System.Collections.Generic;
using UIKit;
using Foundation;
using CoreText;
using CoreGraphics;

namespace SmartLab
{
	public class RequestSource : UITableViewSource
	{
		public Response Response;
		public RequestSource (Response resp)
		{
			this.Response = resp;
		}

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			return this.Response.Requests.Count + 1;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			if (indexPath.Row == this.Response.Requests.Count) {
				var cell = tableView.DequeueReusableCell ("sumCell");
				if (cell == null) {
					cell = new UITableViewCell (UITableViewCellStyle.Default, "sumCell");
				}
				string[] counts = new string[] { this.Response.Help.ToString(), 
					this.Response.InUse.ToString(), 
					this.Response.Idle.ToString(), 
					this.Response.Offline.ToString() 
				};

				string txt = String.Format ("{0}\t\t{1}\t\t{2}\t\t{3}", counts [0], counts [1], counts [2], counts [3]);
				NSMutableAttributedString str = new NSMutableAttributedString (txt);
				var attr = new UIStringAttributes () {
					ForegroundColor = UIColor.Red
				};

				int count = counts [0].Length + 2;
				str.AddAttributes (attr, new NSRange (0, counts [0].Length));

				attr.ForegroundColor = UIColor.Blue;
				str.AddAttributes(attr, new NSRange(count, counts[1].Length));
				count += counts [1].Length + 2;

				attr.ForegroundColor = UIColor.FromRGB (39, 174, 96);
				str.AddAttributes(attr, new NSRange(count, counts[2].Length));
				count += counts [2].Length + 2;

				attr.ForegroundColor = UIColor.Black;
				str.AddAttributes(attr, new NSRange(count, counts[3].Length));

				cell.TextLabel.AttributedText = str;
				cell.TextLabel.TextAlignment = UITextAlignment.Center;

				return cell;
			} else {
				var cell = tableView.DequeueReusableCell ("requestCell");
				if (cell == null) {
					cell = new UITableViewCell (UITableViewCellStyle.Value1, "requestCell");
				}

				var r = this.Response.Requests [indexPath.Row];
				cell.TextLabel.Text = r.Location;
				if (r.IsBizCalc ()) {
					cell.TextLabel.TextColor = UIColor.FromRGB (111, 38, 135);
				} else {
					cell.TextLabel.TextColor = UIColor.Red;
				}
				cell.TextLabel.Font = UIFont.BoldSystemFontOfSize (22f);

				cell.DetailTextLabel.Text = r.Created.ToString ("G");
				return cell;
			}
		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			tableView.DeselectRow (indexPath, true);
		}
	}
}

