using Foundation;
using UIKit;
using SafariServices;

namespace SmartLab
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the
	// User Interface of the application, as well as listening (and optionally responding) to application events from iOS.
	[Register ("AppDelegate")]
	public class AppDelegate : UIApplicationDelegate
	{
		// class-level declarations
		public UIApplicationShortcutItem LaunchedShortcutItem { get; set; }
		public RequestsViewCtrl _Controller;

		public override UIWindow Window {
			get;
			set;
		}

		public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
		{
			// Override point for customization after application launch.
			// If not required for your application you can safely delete this method

			// Theming
			UINavigationBar.Appearance.BarTintColor = UIColor.FromRGB (111, 38, 135);
			UINavigationBar.Appearance.TintColor = UIColor.FromRGB (129, 172, 39);
			UINavigationBar.Appearance.SetTitleTextAttributes (new UITextAttributes () { TextColor = UIColor.FromRGB (129, 172, 39) });


			// Force Touch Support.
			var shouldPerformAdditionalDelegateHandling = true;

			// Get possible shortcut item
			if (launchOptions != null) {
				LaunchedShortcutItem = launchOptions [UIApplication.LaunchOptionsShortcutItemKey] as UIApplicationShortcutItem;
				shouldPerformAdditionalDelegateHandling = (LaunchedShortcutItem == null);
			}
			this._Controller = Window.RootViewController.ChildViewControllers [0] as RequestsViewCtrl;
			return shouldPerformAdditionalDelegateHandling;
		}

		public override void OnResignActivation (UIApplication application)
		{
			// Invoked when the application is about to move from active to inactive state.
			// This can occur for certain types of temporary interruptions (such as an incoming phone call or SMS message) 
			// or when the user quits the application and it begins the transition to the background state.
			// Games should use this method to pause the game.
		}

		public override void DidEnterBackground (UIApplication application)
		{
			// Use this method to release shared resources, save user data, invalidate timers and store the application state.
			// If your application supports background exection this method is called instead of WillTerminate when the user quits.
		}

		public override void WillEnterForeground (UIApplication application)
		{
			// Called as part of the transiton from background to active state.
			// Here you can undo many of the changes made on entering the background.
		}

		public override void OnActivated (UIApplication application)
		{
			// Handle any shortcut item being selected
			HandleShortcutItem(LaunchedShortcutItem);

			// Clear shortcut after it's been handled
			LaunchedShortcutItem = null;
		}

		public override void PerformActionForShortcutItem (UIApplication application, UIApplicationShortcutItem shortcutItem, UIOperationHandler completionHandler)
		{
			// Perform action
			completionHandler(HandleShortcutItem(shortcutItem));
		}

		public override void WillTerminate (UIApplication application)
		{
			// Called when the application is about to terminate. Save data, if needed. See also DidEnterBackground.
		}

		// Handle force touch shortcuts being clicked.
		public bool HandleShortcutItem(UIApplicationShortcutItem shortcutItem) {
			var handled = false;

			// Anything to process?
			if (shortcutItem == null)
				return false;

			// Take action based on the shortcut type
			switch (shortcutItem.Type) {
			case "edu.usf.smartlab.000":
				if (UIDevice.CurrentDevice.CheckSystemVersion(9,0)) {
					var sfViewCtrl = new SFSafariViewController (new NSUrl(Api.SCHEDULE_URL));
					this._Controller.PresentViewControllerAsync (sfViewCtrl, true);
				} else {
					UIApplication.SharedApplication.OpenUrl(new NSUrl(Api.SCHEDULE_URL));
				}
				handled = true;
				break;
			case "edu.usf.smartlab.001":
				this._Controller.NavigationController.PushViewController(new CalloutViewCtrl(), true);
				handled = true;
				break;
			}

			// Return results
			return handled;
		}
	}
}