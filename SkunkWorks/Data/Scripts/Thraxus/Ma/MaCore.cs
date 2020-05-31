using Sandbox.Game.Entities;
using Sandbox.ModAPI;
using SkunkWorks.Thraxus.Common.BaseClasses;
using SkunkWorks.Thraxus.Common.Enums;
using SkunkWorks.Thraxus.Ma.Models;
using VRage.Collections;
using VRage.Game.Components;
using VRage.Game.Entity;

namespace SkunkWorks.Thraxus.Ma
{
	[MySessionComponentDescriptor(MyUpdateOrder.BeforeSimulation, priority: int.MinValue + 1)]
	public class MaCore : BaseSessionComp
	{
		protected override string CompName { get; } = "MaCore";
		protected override CompType Type { get; } = CompType.Server;
		protected override MyUpdateOrder Schedule { get; } = MyUpdateOrder.NoUpdate;

		private readonly MyConcurrentList<OxyGen> _generators = new MyConcurrentList<OxyGen>();

		protected override void SuperEarlySetup()
		{
			base.SuperEarlySetup();
			MyEntities.OnEntityCreate += OnEntityCreate;
			MyAPIGateway.Utilities.MessageEntered += ChatMessageHandler;
		}

		private void ChatMessageHandler(string message, ref bool sendToOthers)
		{
			if (!message.StartsWith("go")) return;
			StartWorking();
		}

		private void StartWorking()
		{
			foreach (OxyGen generator in _generators)
			{
				generator.StartWorking();
			}
		}

		private void OnEntityCreate(MyEntity ent)
		{
			IMyGasGenerator generator = ent as IMyGasGenerator;
			if (generator == null) return;
			if (generator.BlockDefinition.SubtypeId != "MA_O2") return;
			OxyGen oxy = new OxyGen(generator);
			oxy.OnWriteToLog += WriteToLog;
			oxy.Report();
			_generators.Add(oxy);
		}

		protected override void Unload()
		{
			MyEntities.OnEntityCreate -= OnEntityCreate;
			MyAPIGateway.Utilities.MessageEntered -= ChatMessageHandler;
			foreach (OxyGen generator in _generators)
			{
				generator.Close();
				generator.OnWriteToLog -= WriteToLog;
			}
			base.Unload();
		}
	}
}