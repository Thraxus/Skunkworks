﻿using Sandbox.ModAPI;
using SkunkWorks.Thraxus.Common.Enums;
using SkunkWorks.Thraxus.Common.Interfaces;
using SkunkWorks.Thraxus.Common.Utilities.Tools.Logging;
using SkunkWorks.Thraxus.Settings;
using VRage.Game;
using VRage.Game.Components;

namespace SkunkWorks.Thraxus.Common.BaseClasses
{
	public abstract class BaseSessionComp : MySessionComponentBase, ILog
	{
		private readonly string _logName;
		private readonly string _sessionName;

		private readonly bool _noUpdate;

		internal long TickCounter;

		private Log _generalLog;

		private bool _superEarlySetupComplete;
		private bool _earlySetupComplete;
		private bool _lateSetupComplete;

		private readonly bool _blockUpdates;

		protected BaseSessionComp(string sessionName, SessionCompType type, bool noUpdate = true)
		{
			_sessionName = sessionName;
			_logName = $"{_sessionName}-General";
			_noUpdate = noUpdate;
			switch (type)
			{
				case SessionCompType.None:
					_blockUpdates = false;
					break;
				case SessionCompType.Both:
					_blockUpdates = false;
					break;
				case SessionCompType.Client:
					_blockUpdates = ModSettings.IsServer;
					break;
				case SessionCompType.Server:
					_blockUpdates = !ModSettings.IsServer;
					break;
				default:
					break;
			}
		}

		/// <inheritdoc />
		public override void LoadData()
		{
			if (_blockUpdates) return;
			base.LoadData();
			if (!_superEarlySetupComplete) SuperEarlySetup();
		}

		public override MyObjectBuilder_SessionComponent GetObjectBuilder()
		{
			// Always return base.GetObjectBuilder(); after your code! 
			// Do all saving here, make sure to return the OB when done;
			return base.GetObjectBuilder();
		}

		public override void SaveData()
		{
			// this save happens after the game save, so it has limited uses really
			base.SaveData();
		}

		protected virtual void SuperEarlySetup()
		{
			_superEarlySetupComplete = true;
			_generalLog = new Log(_logName);
		}

		public override void BeforeStart()
		{
			if (_blockUpdates) return;
			base.BeforeStart();
		}

		public override void Init(MyObjectBuilder_SessionComponent sessionComponent)
		{
			if (_blockUpdates) return;
			base.Init(sessionComponent);
			if (!_earlySetupComplete) EarlySetup();
		}

		protected virtual void EarlySetup()
		{
			_earlySetupComplete = true;
			WriteToLog("EarlySetup", $"Waking up.", LogType.General);
		}

		public override void UpdateBeforeSimulation()
		{
			if (_blockUpdates) return;
			base.UpdateBeforeSimulation();
			if (!_lateSetupComplete) LateSetup();
			RunBeforeSimUpdate();
		}

		protected virtual void RunBeforeSimUpdate()
		{
			TickCounter++;
		}

		protected virtual void LateSetup()
		{
			_lateSetupComplete = true;
			if (_noUpdate) MyAPIGateway.Utilities.InvokeOnGameThread(() => SetUpdateOrder(MyUpdateOrder.NoUpdate));
			WriteToLog("LateSetup", $"Fully online.", LogType.General);
		}


		public override void UpdateAfterSimulation()
		{
			if (_blockUpdates) return;
			base.UpdateAfterSimulation();
		}

		protected override void UnloadData()
		{
			Unload();
			base.UnloadData();
		}

		protected virtual void Unload()
		{
			if (_blockUpdates) return;
			WriteToLog("Unload", $"Retired.", LogType.General);
			_generalLog?.Close();
		}

		public void WriteToLog(string caller, string message, LogType type, bool showOnHud = false, int duration = ModSettings.DefaultLocalMessageDisplayTime, string color = MyFontEnum.Green)
		{
			switch (type)
			{
				case LogType.Exception:
					WriteException(caller, message, showOnHud, duration, color);
					return;
				case LogType.General:
					WriteGeneral(caller, message, showOnHud, duration, color);
					return;
				default:
					return;
			}
		}

		private readonly object _writeLocker = new object();

		private void WriteException(string caller, string message, bool showOnHud, int duration, string color)
		{
			StaticLog.WriteToLog($"{_sessionName}: {caller}", $"Exception! {message}", LogType.Exception, showOnHud, duration, color);
		}

		private void WriteGeneral(string caller, string message, bool showOnHud, int duration, string color)
		{
			lock (_writeLocker)
			{
				_generalLog?.WriteToLog($"{_sessionName}: {caller}", message, showOnHud, duration, color);
			}
		}
	}
}