using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeManager.Model
{
	public static class AppModel
	{
		public static List<Session> Sessions { get; } = new List<Session>();
		public static List<Session> RemovedSessions { get; } = new List<Session>();
		public static Session CurrentSession { get; set; }
		public static Timer CurrentTimer => CurrentSession.Timer;

		public static Session AddSession(Timer timer, string name)
		{
			var session = new Session(timer, name);
			Sessions.Add(session);
			return session;
		}
		public static void SetCurrentSession(Session session)
		{
			if (!Sessions.Contains(session))
				Sessions.Add(session);
			CurrentSession = session;
		}
		public static void RemoveSession(Session session)
		{
			RemovedSessions.Add(session);
			Sessions.Remove(session);
		}

	}
}
