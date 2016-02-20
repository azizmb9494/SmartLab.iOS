using System;
using System.Linq;
using System.Collections.Generic;
using UIKit;
using Foundation;
using BigTed;

namespace SmartLab
{
	public class CalSource : UITableViewSource
	{
		public List<Event> Events = new List<Event>();
		private CalViewCtrl _CalViewCtrl;

		public CalSource (CalViewCtrl ctrl)
		{
			this.Events = ctrl.Events;
			this._CalViewCtrl = ctrl;
		}

		public CalSource (CalViewCtrl ctrl, string qry)
		{
			this._CalViewCtrl = ctrl;
			qry = qry.ToLowerInvariant ();
			this.Events = ctrl.Events.Where (x => x.Title.ToLowerInvariant ().Contains (qry)).ToList ();
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
			var Event = this.Events [indexPath.Row];

			// Prompt user for, and schedule reminder.
			UIAlertView alertView = new UIAlertView ("Schedule Reminder", "Would you like to schedule a reminder for this event?", null, "Cancel");
			nint yes = alertView.AddButton ("Yes");
			alertView.Clicked += (sender, e) => {
				if (e.ButtonIndex == yes) {
					UIActionSheet aSheet = new UIActionSheet ("When", null, "Nevermind", "Delete Reminders");
					nint morning = aSheet.AddButton ("Morning Of");
					nint evening = aSheet.AddButton ("Evening Before");
					aSheet.Clicked += (sr, ev) => {
						if (ev.ButtonIndex == morning) {
							UILocalNotification n = new UILocalNotification ();
							n.AlertTitle = "ASC Reminder";
							n.AlertBody = Event.Title;
							n.FireDate = (NSDate)Event.Date.ToLocalTime().Date.AddHours (8);
							UIApplication.SharedApplication.ScheduleLocalNotification (n);
							BTProgressHUD.ShowSuccessWithStatus("Added!");
						} else if (ev.ButtonIndex == evening) {
							UILocalNotification n = new UILocalNotification ();
							n.AlertTitle = "ASC Reminder";
							n.AlertBody = Event.Title;
							n.FireDate = (NSDate)Event.Date.ToLocalTime().Date.AddHours (-5);
							UIApplication.SharedApplication.ScheduleLocalNotification (n);
							BTProgressHUD.ShowSuccessWithStatus("Added!");
						}
					};

					aSheet.ShowInView(this._CalViewCtrl.View);
				}
			};
			alertView.Show ();
		}
	}
}

