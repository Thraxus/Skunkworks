using Sandbox.Game.Entities;
using SkunkWorks.Thraxus.Common.BaseClasses;
using VRage.Game.ModAPI;
using VRage.ModAPI;

namespace SkunkWorks.Thraxus.GridControl.Controllers
{
	public class ControllableGrid : BaseLoggingClass
	{
		protected override string Id { get; } = "ControllableGrid";

		private readonly IMyEntity _thisEntity;
		private readonly IMyCubeGrid _thisIGrid;
		private readonly MyCubeGrid _thisGrid;

		private bool _isClosed;

		public ControllableGrid(IMyCubeGrid grid)
		{
			_thisIGrid = grid;
			_thisEntity = grid;
			_thisGrid = (MyCubeGrid) grid;

			_thisGrid.OnFatBlockAdded += OnFatBlockAdded;
			_thisGrid.OnFatBlockRemoved += OnFatBlockRemoved;
			_thisGrid.OnClose += Close;
		}
		
		public void Close(IMyEntity entity)
		{
			if (_isClosed) return;
			_isClosed = true;
			_thisGrid.OnClose -= Close;
			_thisGrid.OnFatBlockAdded -= OnFatBlockAdded;
			_thisGrid.OnFatBlockRemoved -= OnFatBlockRemoved;
			Close();
		}

		private void OnFatBlockRemoved(MyCubeBlock block)
		{
			
		}

		private void OnFatBlockAdded(MyCubeBlock block)
		{
			
		}

		private void RegisterNewBlock(IMyEntity entity)
		{

		}
	}
}
