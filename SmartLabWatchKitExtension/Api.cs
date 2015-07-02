using System;
using System.Linq;
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

		async public static Task<List<Request>> GetRequests() {
			Client.Timeout = TimeSpan.FromSeconds (7);
			string url = "http://azizmb.com/request.php";
			try {
				var resp = await Client.GetAsync (url);
				Updated = DateTime.Now;
				//return JsonConvert.DeserializeObject(await resp.Content.ReadAsStringAsync(), typeof(List<Request>)) as List<Request>;
				var list = new List<Request>();
				for (int i = 10; i < 23; i++) {
					list.Add(new Request() { Created = DateTime.Now, Location = i.ToString() + "F" });
				}
				return list;
			} catch {
				Updated = new DateTime (0);
				return new List<Request> ();
			}
		}

		async public static Task<List<Event>> GetCalendar() {
			Client.Timeout = TimeSpan.FromSeconds (3);
			try {
				var resp = await Client.GetAsync ("http://azizmb.com/calendar.json");
				var ev = (JsonConvert.DeserializeObject (await resp.Content.ReadAsStringAsync (), typeof(List<Event>)) as List<Event>).OrderBy(x=>x.Date).ToList();
				return ev;
			} catch {
				return new List<Event> ();
			}
		}
	}
}

