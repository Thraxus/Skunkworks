﻿using System;
using Sandbox.ModAPI;

namespace SkunkWorks.Thraxus.Settings
{
	public static class ModSettings
	{	// These settings should be used by the mod directly, and not changeable by a user.  They are considered "reference only"

		public const string MyIniFileName = "MyCustomIniName.ini";
		public const string SaveFileName = "MyCustomSave.file";
		public const string SandboxVariableName = "MyCustomSandboxVariableName";

		public const string ChatCommandPrefix = "chatCommand";
		public const string ExceptionLogName = "Exception";
		public const string StaticGeneralLogName = "StaticLog-General";
		public const ushort NetworkId = 16759;

		#region Reference Values

		public static bool IsServer => MyAPIGateway.Multiplayer.IsServer;

		public const int DefaultLocalMessageDisplayTime = 5000;
		public const int DefaultServerMessageDisplayTime = 10000;
		public const int TicksPerSecond = 60;
		public const int TicksPerMinute = TicksPerSecond * 60;

		public static Random Random { get; } = new Random();

		#endregion
	}
}
