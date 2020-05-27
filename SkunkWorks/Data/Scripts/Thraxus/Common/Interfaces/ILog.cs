using SkunkWorks.Thraxus.Common.Enums;
using SkunkWorks.Thraxus.Settings;
using VRage.Game;

namespace SkunkWorks.Thraxus.Common.Interfaces
{
	internal interface ILog
	{
		void WriteToLog(string caller, string message, LogType type, bool showOnHud = false, int duration = ModSettings.DefaultLocalMessageDisplayTime, string color = MyFontEnum.Green);
	}
}
