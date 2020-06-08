using SkunkWorks.Thraxus.Common.Enums;
using VRage.Game;

namespace SkunkWorks.Thraxus.Common.Interfaces
{
	internal interface ILog
	{
		void WriteToLog(string caller, string message, LogType type, bool showOnHud = false, int duration = Settings.DefaultLocalMessageDisplayTime, string color = MyFontEnum.Green);
	}
}
