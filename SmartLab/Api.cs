using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using ModernHttpClient;

namespace SmartLab
{
	public static class Api
	{
		public const string SCHEDULE_URL = "http://google.com";

		private const string CALENDAR_URL = "http://azizmb.com/calendar.json";	// Calendar JSON URL.
		private const string WOOFOO_URL = "http://wufoo.com";					// Wufoo Form URL.
		private const string WOOFOO_API = "";									// Wufoo API Key.

		private static HttpClient Client { get; set; }
		public static DateTime Updated { get; private set; }

		static Api ()
		{
			Client = new HttpClient (new NativeMessageHandler ()) { Timeout = TimeSpan.FromSeconds (5) };
			Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue ("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes(WOOFOO_API +":Foreign")));
			Updated = new DateTime (0);
		}

		/// <summary>
		/// Fills out Call Out form with inputed information.
		/// </summary>
		/// <returns>Did it succeed?</returns>
		/// <param name="firstName">First Name.</param>
		/// <param name="lastName">Last Name.</param>
		/// <param name="email">Email Address.</param>
		/// <param name="start">Start Date.</param>
		/// <param name="multiple">Multiple Days.</param>
		/// <param name="end">End Date.</param>
		/// <param name="centers">Centers affected.</param>
		/// <param name="reason">Reason.</param>
		async public static Task<bool> CallOut(string firstName, string lastName, string email, DateTime start, bool multiple, DateTime end, bool[] centers, string reason)
		{
			// Enter values for form fields.
			List<KeyValuePair<string, string>> Values = new List<KeyValuePair<string, string>> ();
			Values.Add(new KeyValuePair<string, string>("Field109", "Call Out"));
			Values.Add (new KeyValuePair<string, string> ("Field2", firstName));
			Values.Add(new KeyValuePair<string, string>("Field3", lastName));					
			Values.Add(new KeyValuePair<string, string>("Field107", email));
			Values.Add(new KeyValuePair<string, string>("Field5", reason));

			Values.Add(new KeyValuePair<string, string>("Field4", start.Date.ToString("yyyyMMdd")));
			Values.Add(new KeyValuePair<string, string>("Field219", multiple ? "1" : "0"));
			if (multiple) {
				Values.Add (new KeyValuePair<string, string> ("Field116",end.Date.ToString("yyyyMMdd")));
			}

			// Centers to choose from.
			string[] Centers = new string[] {
				"SMART Lab (Tutor)", 
				"SMART Lab (TA)",
				"STEM Center",
				"Business Calculus",
				"Stats Center",
				"Chemistry Center",
				"Appointments",
				"Front Desk"
			};

			var fields = new int[] { 6, 11, 9, 12, 13, 7, 8, 10 };
			for (int i = 0; i < centers.Length; i++) {
				if (centers [i]) {
					Values.Add (new KeyValuePair<string, string> ("Field" + fields [i], Centers[i]));
				}
			}

			// Send it up.
			try {
				var resp = await Client.PostAsync(WOOFOO_URL, new FormUrlEncodedContent(Values));
				var s = JsonConvert.DeserializeObject(await resp.Content.ReadAsStringAsync(), typeof(FormResponse)) as FormResponse;
				return s != null && s.Success == 1;
			} catch {
				return false;
			}
		}


		/// <summary>
		/// Fills out In Late form with inputed information.
		/// </summary>
		/// <returns>Did it succeed?</returns>
		/// <param name="first">First Name.</param>
		/// <param name="last">Last Name.</param>
		/// <param name="email">Email Address.</param>
		/// <param name="inTime">Date & Time Late.</param>
		/// <param name="centers">Centers affected.</param>
		/// <param name="reason">Reason.</param>
		async public static Task<bool> SendLate(string first, string last, string email, DateTime inTime, bool[] centers, string reason)
		{
			// Enter values in form fields.
			List<KeyValuePair<string, string>> Values = new List<KeyValuePair<string, string>> ();
			Values.Add(new KeyValuePair<string, string>("Field109", "In late"));
			Values.Add(new KeyValuePair<string, string>("Field2", first));
			Values.Add(new KeyValuePair<string, string>("Field3", last));
			Values.Add(new KeyValuePair<string, string>("Field107", email));
			Values.Add(new KeyValuePair<string, string>("Field5", reason));
			Values.Add(new KeyValuePair<string, string>("Field320", inTime.Date.ToString("yyyyMMdd")));
			Values.Add (new KeyValuePair<string, string> ("Field116", inTime.ToLocalTime().ToString ("HH:mm:ss")));

			// Centers to choose form.
			string[] Centers = new string[] {
				"SMART Lab (Tutor)", 
				"SMART Lab (TA)",
				"STEM Center",
				"Business Calculus",
				"Stats Center",
				"Chemistry Center",
				"Appointments",
				"Front Desk"
			};

			var fields = new int[] { 6, 11, 9, 12, 13, 7, 8, 10 };
			for (int i = 0; i < centers.Length; i++) {
				if (centers [i]) {
					Values.Add (new KeyValuePair<string, string> ("Field" + fields [i], Centers[i]));
				}
			}

			// Send it up.
			try {
				var resp = await Client.PostAsync(WOOFOO_URL, new FormUrlEncodedContent(Values));
				var s = JsonConvert.DeserializeObject(await resp.Content.ReadAsStringAsync(), typeof(FormResponse)) as FormResponse;
				return s != null && s.Success == 1;
			} catch {
				return false;
			}
		}
			

		/// <summary>
		/// Scrapes SmartLab website to get on-going help request & counters.
		/// Sets Updated property to current time if successful, defaults to 0 if fails.
		/// </summary>
		/// <returns>Response with list of requests & computer stats.</returns>
		async public static Task<Response> GetRequests()
		{
			Client.Timeout = TimeSpan.FromSeconds (5);
			string url = "http://usfweb.usf.edu/labdisplay/view/lib232-full.aspx";
			try {
				var resp = await Client.GetAsync(url);
				Updated = DateTime.Now;
				string content = await resp.Content.ReadAsStringAsync();
				Response r = new Response() { Requests = new List<Request>(), InUse = 0, Offline = 0, Help = 0, Idle = 0 };

				Regex regex = new Regex(@"(?!<td>)\d{1,2}\w(?=</td>)|(?!<td>)\d{2}/\d{2}\s\d{2}:\d{2}:\d{2}\s[AP]M(?=</td>)");
				MatchCollection match = regex.Matches(content);
				for (int i = 0; i < match.Count / 2; i++)
				{
					string loc = match[i*2].Value;
					string dtString = match[(i*2)+1].Value;
					dtString = dtString.Insert(5, "/" + DateTime.Today.Year);
					DateTime date = DateTime.ParseExact(dtString, "MM/dd/yyyy hh:mm:ss tt", System.Globalization.CultureInfo.InvariantCulture);
					r.Requests.Add(new Request() { Location = loc, Created = date });
				}

				regex = new Regex("compInUse");
				match = regex.Matches(content);
				r.InUse = match.Count;

				regex = new Regex("compLoggingOff");
				match = regex.Matches(content);
				r.Idle = match.Count;

				regex = new Regex("compFaulty");
				match = regex.Matches(content);
				r.Offline = match.Count;

				r.Help = r.Requests.Count;

				return r;
			} catch {
				Updated = new DateTime(0);
				return new Response ();
			}
		}

		/// <summary>
		/// Gets the calendar from URL. 
		/// </summary>
		/// <returns>List of Events.</returns>
		async public static Task<List<Event>> GetCalendar() {
			try {
				var resp = await Client.GetAsync (CALENDAR_URL);
				var ev = (JsonConvert.DeserializeObject (await resp.Content.ReadAsStringAsync (), typeof(List<Event>)) as List<Event>).OrderBy(x=>x.Date).ToList();
				return ev;
			} catch {
				return new List<Event> ();
			}
		}
	}
}

