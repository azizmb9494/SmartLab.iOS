using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SmartLab
{
	public static class Cache
	{
		private static string _Path;

		static Cache() {
			_Path = Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments), "cal.json");
		}

		public static List<Event> GetEvents()
		{
			if (File.Exists (_Path)) {
				return JsonConvert.DeserializeObject (File.ReadAllText (_Path), typeof(List<Event>)) as List<Event>;
			} else if (File.Exists ("calendar.json")) {
				return JsonConvert.DeserializeObject (File.ReadAllText ("calendar.json"), typeof(List<Event>)) as List<Event>;
			} else {
				return new List<Event> ();
			}
		}

		public static void SetEvents(List<Event> ev)
		{
			File.WriteAllText (_Path, JsonConvert.SerializeObject (ev));
		}
	}
}

