using SkunkWorks.Thraxus.Common.Enums;
using SkunkWorks.Thraxus.Common.Utilities.FileHandlers;
using SkunkWorks.Thraxus.Common.Utilities.Tools.Logging;
using VRage.Game.ModAPI.Ingame.Utilities;

namespace SkunkWorks.Thraxus.Settings.MyCustomIni
{
	public static class ImportCustomUserSettings
	{
		private static readonly MyIni MyIni = new MyIni();
		private static string _customUserIni;
		private static bool _customConfigSet;

		public static void Run()
		{
			if (_customConfigSet) return;
			_customConfigSet = true;

			_customUserIni = Load.ReadFileFromWorldStorage(ModSettings.MyIniFileName, typeof(IniSupport));
			if (string.IsNullOrEmpty(_customUserIni))
			{
				StaticLog.WriteToLog("GetCustomUserIni", "No custom settings found. Exporting vanilla settings.", LogType.General);
				ExportDefaultUserSettings.Run();
				return;
			}
			if (!MyIni.TryParse(_customUserIni))
			{
				StaticLog.WriteToLog("GetCustomUserIni", "Parse failed for custom user settings. Exporting vanilla settings.", LogType.General);
				ExportDefaultUserSettings.Run();
				return;
			}
			if (!MyIni.ContainsSection(ConfigConstants.SectionName))
			{
				StaticLog.WriteToLog("GetCustomUserIni", "User config did not contain the proper section. Exporting vanilla settings.", LogType.General);
				ExportDefaultUserSettings.Run();
				return;
			}
			ParseConfig();
		}

		private static void ParseConfig()
		{
			UserSettings.MyIniSettingBoolExample = MyIni.Get(ConfigConstants.SectionName, IniSupport.MyIniSettingBoolExampleName).ToBoolean(UserSettings.MyIniSettingBoolExample);
			UserSettings.MyIniSettingIntExample = MyIni.Get(ConfigConstants.SectionName, IniSupport.MyIniSettingIntExampleName).ToBoolean(UserSettings.MyIniSettingIntExample);

		}
	}
}
