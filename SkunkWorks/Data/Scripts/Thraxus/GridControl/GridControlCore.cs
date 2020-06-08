using Sandbox.ModAPI;
using SkunkWorks.Thraxus.Common.BaseClasses;
using SkunkWorks.Thraxus.Common.Enums;
using SkunkWorks.Thraxus.GridControl.Controllers;
using VRage.Collections;
using VRage.Game.Components;
using VRage.Game.ModAPI;
using VRage.ModAPI;

namespace SkunkWorks.Thraxus.GridControl
{
	public class GridControlCore : BaseSessionComp
	{
		protected override string CompName { get; } = "GridControlCore";
		protected override CompType Type { get; } = CompType.Server;
		protected override MyUpdateOrder Schedule { get; } = MyUpdateOrder.BeforeSimulation;
		
		private readonly MyConcurrentList<ControllableGrid> _grids = new MyConcurrentList<ControllableGrid>();

		protected override void SuperEarlySetup()
		{
			base.SuperEarlySetup();
			// Code below here
			MyAPIGateway.Entities.OnEntityAdd += OnEntityAdd;
		}

		private void OnEntityAdd(IMyEntity entity)
		{
			IMyCubeGrid grid = entity as IMyCubeGrid;
			if (grid == null) return;
			RegisterNewGrid(grid);
		}

		private void RegisterNewGrid(IMyCubeGrid grid)
		{
			// Logic for picking proper grids goes here

			ControllableGrid controllableGrid = new ControllableGrid(grid);
			controllableGrid.OnWriteToLog += WriteToLog;
			controllableGrid.OnClose += OnGridClose;
			_grids.Add(controllableGrid);
		}

		private void DeRegisterGrid(ControllableGrid grid)
		{
			if (grid == null) return;
			grid.OnWriteToLog -= WriteToLog;
			grid.OnClose -= OnGridClose;
			grid.Close(null);
			_grids.Remove(grid);
		}

		private void OnGridClose(BaseClosableClass grid)
		{
			DeRegisterGrid(grid as ControllableGrid);
		}

		protected override void Unload()
		{
			MyAPIGateway.Entities.OnEntityAdd -= OnEntityAdd;
			foreach (ControllableGrid grid in _grids)
				DeRegisterGrid(grid);

			// Unload code above base
			base.Unload();
		}
	}
}
