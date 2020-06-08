using System;
using SkunkWorks.Thraxus.Common.Enums;
using VRage.Game;

namespace SkunkWorks.Thraxus.Common.BaseClasses
{
	public abstract class BaseLoggingClass : BaseClosableClass
	{
		public event Action<string, string, LogType, bool, int, string> OnWriteToLog;

		protected abstract string Id { get; }
		
		protected void WriteToLog(string caller, string message, LogType type, bool showOnHud = false, int duration = Settings.DefaultLocalMessageDisplayTime, string color = MyFontEnum.Green)
		{
			OnWriteToLog?.Invoke($"{Id}: {caller}", message, type, showOnHud, duration, color);
		}
	}
}