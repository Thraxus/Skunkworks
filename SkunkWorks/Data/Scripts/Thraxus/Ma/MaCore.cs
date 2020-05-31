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

		private readonly MyConcurrentList<BaseClosableLoggingClass> _generators = new MyConcurrentList<BaseClosableLoggingClass>();

		protected override void SuperEarlySetup()
		{
			base.SuperEarlySetup();
			MyEntities.OnEntityCreate += OnEntityCreate;
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
			foreach (BaseClosableLoggingClass generator in _generators)
			{
				generator.Close();
				generator.OnWriteToLog -= WriteToLog;
			}
			base.Unload();
		}
	}
}