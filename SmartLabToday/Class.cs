﻿using System;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace SmartLabToday
{
	public class Request
	{
		[JsonProperty("location")]
		public string Location { get; set; }
		[JsonProperty("time")]
		public DateTime Created { get; set; }
		[JsonIgnore()]
		public int Count { get; set; }

		public Request()
		{

		}

		public Request(string location, DateTime created) 
		{
			this.Location = location;
			this.Created = created;
		}

		public Request(string location, int times)
		{
			this.Location = location;
			this.Count = times;
		}

		public bool IsBizCalc()
		{
			string pod = this.Location.Substring (0, this.Location.Length - 1);
			return pod == "36" || pod == "40" || pod == "44" || pod == "47" || pod == "48"; 
		}
	}

	public class Calendar {
		[JsonProperty("results")]
		public List<Event> List = new List<Event>();
	}

	public class Faq
	{
		[JsonProperty("question")]
		public string Question { get; set; }
		[JsonProperty("answer")]
		public string Answer { get; set; }
	}

	public class Event
	{
		[JsonProperty("date")]
		public DateTime Date { get; set; }
		[JsonProperty("title")]
		public string Title { get; set; }
		[JsonIgnore()]
		public string FormattedDate { get { return Date.Hour == 0 ? Date.ToString ("D") : Date.ToString ("f"); } }
	}

	public class RequestComparer : IEqualityComparer<Request>
	{

		public bool Equals(Request r1, Request r2)
		{
			return r1.Location == r2.Location && r1.Created == r2.Created;
		}

		public int GetHashCode(Request r)
		{
			string s = r.Location.Substring (r.Location.Length - 1, 1);
			int table = (Int32.Parse(r.Location.Substring (0, r.Location.Length - 1)))*10;
			if (s == "A") {
				return table + 1;
			}

			if (s == "B") {
				return table + 2;
			}

			if (s == "C") {
				return table + 3;
			}

			if (s == "D") {
				return table + 4;
			}

			if (s == "E") {
				return table + 5;
			}

			if (s == "F") {
				return table + 6;
			}

			return table;
		}
	}
}
