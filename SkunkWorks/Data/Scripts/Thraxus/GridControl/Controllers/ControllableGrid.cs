﻿using Sandbox.Game.Entities;
using Sandbox.Game.Entities.Cube;
using Sandbox.Game.GameSystems;
using Sandbox.ModAPI;
using SkunkWorks.Thraxus.Common.BaseClasses;
using SkunkWorks.Thraxus.GridControl.Models;
using VRage.Collections;
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
		private readonly IMyShipController _thisController;

		private bool _isClosed;

		private readonly ControllableGyros _controllableGyros;
		private readonly ControllableThrusters _controllableThrusters;

		public ControllableGrid(IMyCubeGrid grid, IMyShipController controller)
		{
			_thisIGrid = grid;
			_thisEntity = grid;
			_thisGrid = (MyCubeGrid) grid;
			_thisController = controller;

			_controllableGyros = new ControllableGyros(controller);
			_controllableThrusters = new ControllableThrusters(controller);

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
		
		private void SetupGrid()
		{
			foreach (MyCubeBlock block in _thisGrid.GetFatBlocks())
			{
				MyThrust myThrust = block as MyThrust;
				if (myThrust != null)
				{
					_controllableThrusters.AddNewThruster(myThrust);
					continue;
				}

				MyGyro myGyro = block as MyGyro;
				if (myGyro == null) continue;
				_controllableGyros.Add(myGyro);
			}
		}

		private void OnFatBlockRemoved(MyCubeBlock block)
		{
			if (block == _thisController) Close(block);
		}

		private void OnFatBlockAdded(MyCubeBlock block)
		{
			
		}

		private void RegisterNewBlock(IMyEntity entity)
		{

		}
	}
}