using System;
using SkunkWorks.Thraxus.Common.Enums;
using SkunkWorks.Thraxus.Settings;
using VRage.Game;

namespace SkunkWorks.Thraxus.Common.BaseClasses
{
	public abstract class LogBaseEvent
	{
		public event Action<string, string, LogType, bool, int, string> OnWriteToLog;
		private readonly string _id;

		protected LogBaseEvent(string id)
		{
			_id = id;
		}

		protected void WriteToLog(string caller, string message, LogType type, bool showOnHud = false, int duration = ModSettings.DefaultLocalMessageDisplayTime, string color = MyFontEnum.Green)
		{
			OnWriteToLog?.Invoke($"{_id}: {caller}", message, type, showOnHud, duration, color);
		}
	}
}