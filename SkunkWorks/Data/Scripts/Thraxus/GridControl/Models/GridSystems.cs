using System.Collections.Generic;
using Sandbox.Game.Entities;
using Sandbox.ModAPI;
using SkunkWorks.Thraxus.Common.BaseClasses;
using SkunkWorks.Thraxus.Common.Interfaces;
using SkunkWorks.Thraxus.GridControl.Interfaces;
using VRage.Game.ModAPI;
using VRage.ModAPI;

namespace SkunkWorks.Thraxus.GridControl.Models
{
	internal class GridSystems : BaseLoggingClass, IUpdate
	{
		protected override string Id { get; } = "GridSystems";

		internal readonly ControllableGyros ControllableGyros;
		internal readonly ControllableThrusters ControllableThrusters;
		internal readonly ControllableLandingGear ControllableLandingGear;
		internal readonly DebugScreens DebugScreens = new DebugScreens();

		private readonly List<IClose> _closeUs = new List<IClose>();

		public GridSystems(IMyShipController thisController)
		{
			ControllableGyros = new ControllableGyros(thisController);
			ControllableThrusters = new ControllableThrusters(thisController);
			ControllableLandingGear = new ControllableLandingGear(thisController);
			_closeUs.Add(ControllableGyros);
			_closeUs.Add(ControllableThrusters);
			_closeUs.Add(ControllableLandingGear);
			_closeUs.Add(DebugScreens);
		}

		public void Update(long tick)
		{
			
		}

		public override void Close()
		{
			base.Close();
			foreach (IClose closeThis in _closeUs)
			{
				closeThis?.Close();
			}
		}
	}
}
