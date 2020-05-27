using SkunkWorks.Thraxus.Common.Utilities.FileHandlers;
using VRage.Game.ModAPI.Ingame.Utilities;

namespace SkunkWorks.Thraxus.Settings.MyCustomIni
{
	public static class ExportDefaultUserSettings
	{
		private static readonly MyIni MyIni = new MyIni();
		private static bool _configExported;

		public static void Run()
		{
			if (_configExported) return;
			_configExported = true;

			BuildTheIni();
			Export();
		}

		private static void BuildTheIni()
		{
			MyIni.Set(ConfigConstants.SectionName, IniSupport.MyIniSettingBoolExampleName, UserSettings.MyIniSettingBoolExample);
			MyIni.SetComment(ConfigConstants.SectionName, IniSupport.MyIniSettingBoolExampleDescription, IniSupport.MyIniSettingBoolExampleDescription);

			MyIni.Set(ConfigConstants.SectionName, IniSupport.MyIniSettingIntExampleName, UserSettings.MyIniSettingIntExample);
			MyIni.SetComment(ConfigConstants.SectionName, IniSupport.MyIniSettingIntExampleName, IniSupport.MyIniSettingIntExampleDescription);
		}

		private static void Export()
		{
			Save.WriteToFile(ModSettings.MyIniFileName, MyIni, typeof(IniSupport));
		}
	}
}