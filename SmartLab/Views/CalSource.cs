using System;
using System.Linq;
using System.Collections.Generic;
using UIKit;
using Foundation;

namespace SmartLab
{
	public class CalSource : UITableViewSource
	{
		public List<Event> Events = new List<Event>();
		public CalSource (List<Event> events)
		{
			this.Events = events;
		}

		public CalSource (List<Event> events, string qry)
		{
			qry = qry.ToLowerInvariant ();
			this.Events = events.Where (x => x.Title.ToLowerInvariant ().Contains (qry)).ToList ();
		}

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			return this.Events.Count;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell ("eventCell");
			if (cell == null) {
				cell = new UITableViewCell (UITableViewCellStyle.Subtitle, "eventCell");
			}

			var e = this.Events [indexPath.Row];
			cell.TextLabel.Text = e.Title;
			cell.DetailTextLabel.Text = e.Date.Hour == 0 ? e.Date.ToString ("D") : e.Date.ToString ("f");
			return cell;
		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			tableView.DeselectRow (indexPath, true);
		}
	}
}

