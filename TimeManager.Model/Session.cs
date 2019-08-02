using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeManager.Model
{
	public class Session
	{
		public Guid id = new Guid();

		public string Name { get; set; }

		public string Time
		{
			get => Timer.ElapsedTime;
			set => Timer.ElapsedTime = value;
		}

		public DateTime LastDate { get; set; }
		public DateTime CreateDate { get; set; }

		public Timer Timer { get; set; }

		public Session(Timer timer, string name)
		{
			Timer = timer;
			Name = name;
			LastDate = CreateDate = DateTime.Today;
		}
	}
}
