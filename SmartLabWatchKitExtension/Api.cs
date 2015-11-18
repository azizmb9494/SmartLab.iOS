using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;
using ModernHttpClient;

namespace SmartLabWatchKitExtension
{
	public static class Api
	{
		private static HttpClient Client { get; set; }
		public static DateTime Updated { get; private set; }

		static Api ()
		{
			Client = new HttpClient (new NativeMessageHandler());
			Updated = new DateTime (0);
		}

		/// <summary>
		/// Gets list of help requests from Smart Lab website.
		/// Sets Updated DateTime on success, 0 if failed.
		/// </summary>
		/// <returns>List of help requests</returns>
		async public static Task<List<Request>> GetRequests() {
			Client.Timeout = TimeSpan.FromSeconds (5);
			string url = "http://usfweb.usf.edu/labdisplay/view/lib232-full.aspx";
			try {
				var resp = await Client.GetAsync(url);
				Updated = DateTime.Now;
				string content = await resp.Content.ReadAsStringAsync();
				var Requests = new List<Request>();
				Regex regex = new Regex(@"(?!<td>)\d{1,2}\w(?=</td>)|(?!<td>)\d{2}/\d{2}\s\d{2}:\d{2}:\d{2}\s[AP]M(?=</td>)");
				MatchCollection match = regex.Matches(content);


				for (int i = 0; i < match.Count / 2; i++)
				{
					string loc = match[i*2].Value;
					string dtString = match[(i*2)+1].Value;
					dtString = dtString.Insert(5, "/" + DateTime.Today.Year);
					DateTime date = DateTime.ParseExact(dtString, "MM/dd/yyyy hh:mm:ss tt", System.Globalization.CultureInfo.InvariantCulture);
					Requests.Add(new Request() { Location = loc, Created = date });
				}

				return Requests;
			} catch {
				Updated = new DateTime (0);
				return new List<Request> ();
			}
		}
	}
}

