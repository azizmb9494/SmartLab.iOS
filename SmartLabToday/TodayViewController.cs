using System;
using System.Linq;
using System.Drawing;
using CoreGraphics;
using NotificationCenter;
using Foundation;
using Social;
using UIKit;

namespace SmartLabToday
{
	public partial class TodayViewController : UIViewController, INCWidgetProviding
	{
		private UILabel TimeLbl;
		public TodayViewController (IntPtr handle) : base (handle)
		{
			this.View.BackgroundColor = UIColor.Clear;
			this.TimeLbl = new UILabel (new CGRect (40, 0, this.View.Frame.Width-80, 22)) {
				Text = "Never Updated",
				TextAlignment = UITextAlignment.Left,
				TextColor = UIColor.White
			};

			this.Add (this.TimeLbl);
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();

			// Release any cached data, images, etc that aren't in use.
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			foreach (var v in this.View.Subviews) {
				if (v.GetType() == typeof(BlimpView)) {
					v.RemoveFromSuperview();
				}
			}

			NSTimer.CreateRepeatingScheduledTimer (6, delegate {
				InvokeOnMainThread(delegate {
					this.TimeLbl.Text = "Updating...";	
				});

				InvokeInBackground (async delegate {
					var requests = await Api.GetRequests();
					if (KeyStore.BizCalcOnly) {
						requests = requests.Where(x=>x.IsBizCalc()).ToList();
					}

					if (KeyStore.HideBizCalc) {
						requests = requests.Where(
							x=> x.IsBizCalc() == false
						).ToList();
					}

					InvokeOnMainThread(delegate {
						foreach (var v in this.View.Subviews) {
							if (v.GetType() == typeof(BlimpView)) {
								v.RemoveFromSuperview();
							}
						}

						if (Api.Updated != new DateTime(0)) {
							for (int i = 0; i < Math.Min(requests.Count, 10); ++i)
							{
								var b = new BlimpView(requests[i].Location);
								if (requests[i].IsBizCalc()) {
									b.BackgroundColor = b.BackgroundColor;
								}
								b.Frame = new CoreGraphics.CGRect(3 + (45*(i % 5)), 25 + (45*(i/5)), 40, 40);
								this.View.Add(b);
							}
							this.TimeLbl.Text = Api.Updated.ToString("G");
						} else {
							this.TimeLbl.Text = "Update Failed";
						}
					});
				});
			});

			InvokeInBackground (async delegate {
				InvokeOnMainThread(delegate {
					this.TimeLbl.Text = "Updating...";	
				});
					
				var requests = await Api.GetRequests();
				InvokeOnMainThread(delegate {
					foreach (var v in this.View.Subviews) {
						if (v.GetType() == typeof(BlimpView)) {
							v.RemoveFromSuperview();
						}
					}

					if (Api.Updated != new DateTime(0)) {
						for (int i = 0; i < Math.Min(requests.Count, 10); ++i)
						{
							var b = new BlimpView(requests[i].Location);
							b.Frame = new CoreGraphics.CGRect(3 + (45*(i % 5)), 25 + (45*(i/5)), 40, 40);
							this.View.Add(b);
						}

						this.TimeLbl.Text = Api.Updated.ToString("G");
					} else {
						this.TimeLbl.Text = "Update Failed";
					}
				});
			});
		}
			
		[Export ("widgetPerformUpdateWithCompletionHandler:")]
		public void WidgetPerformUpdate (Action<NCUpdateResult> completionHandler)
		{
			InvokeOnMainThread (delegate {
				this.TimeLbl.Text = "Updating...";
			});

			InvokeInBackground (async delegate {
				var requests = await Api.GetRequests();
				if (KeyStore.BizCalcOnly) {
					requests = requests.Where(
						x=>x.Location.Contains("36") || 
						x.Location.Contains("40") || 
						x.Location.Contains("44") ||
						x.Location.Contains("47") ||
						x.Location.Contains("48")
					).ToList();
				}

				InvokeOnMainThread(delegate {
					foreach (var v in this.View.Subviews) {
						if (v.GetType() == typeof(BlimpView)) {
							v.RemoveFromSuperview();
						}
					}

					if (Api.Updated != new DateTime(0)) {
						for (int i = 0; i < Math.Min(requests.Count, 10); ++i)
						{
							var b = new BlimpView(requests[i].Location);
							b.Frame = new CoreGraphics.CGRect(3 + (45*(i % 5)), 25 + (45*(i/5)), 40, 40);
							this.View.Add(b);
						}
						completionHandler(NCUpdateResult.NewData);
						this.TimeLbl.Text = Api.Updated.ToString("G");
					} else {
						this.TimeLbl.Text = "Update Failed";
					}
				});
			});
		}
	}
}

