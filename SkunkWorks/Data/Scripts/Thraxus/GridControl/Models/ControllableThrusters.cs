using System.Collections.Generic;
using Sandbox.Game.Entities;
using Sandbox.ModAPI;
using SkunkWorks.Thraxus.Common.BaseClasses;
using SkunkWorks.Thraxus.Common.Utilities.Statics;
using SkunkWorks.Thraxus.GridControl.DataTypes.Enums;
using VRage.Collections;

namespace SkunkWorks.Thraxus.GridControl.Models
{
	public class ControllableThrusters : BaseLoggingClass
	{
		protected override string Id { get; } = "ControllableThrusters";

		private readonly MyConcurrentDictionary<ThrustDirection, ConcurrentCachingList<ControllableThruster>> _thrusters = new MyConcurrentDictionary<ThrustDirection, ConcurrentCachingList<ControllableThruster>>();

		private readonly MyConcurrentDictionary<ThrustDirection, float> _maxThrust = new MyConcurrentDictionary<ThrustDirection, float>()
		{
			{ ThrustDirection.Forward, 0 },
			{ ThrustDirection.Back, 0 },
			{ ThrustDirection.Up, 0 },
			{ ThrustDirection.Down, 0 },
			{ ThrustDirection.Left, 0 },
			{ ThrustDirection.Right, 0 },
		};

		private readonly IMyShipController _thisController;

		public ControllableThrusters(IMyShipController controller)
		{
			_thisController = controller;
			_thrusters.Add(ThrustDirection.Forward, new ConcurrentCachingList<ControllableThruster>());
			_thrusters.Add(ThrustDirection.Back, new ConcurrentCachingList<ControllableThruster>());
			_thrusters.Add(ThrustDirection.Up, new ConcurrentCachingList<ControllableThruster>());
			_thrusters.Add(ThrustDirection.Down, new ConcurrentCachingList<ControllableThruster>());
			_thrusters.Add(ThrustDirection.Left, new ConcurrentCachingList<ControllableThruster>());
			_thrusters.Add(ThrustDirection.Right, new ConcurrentCachingList<ControllableThruster>());
		}

		public void AddNewThruster(MyThrust myThrust)
		{
			if (myThrust == null || _thisController == null) return;
			ControllableThruster thruster = null;
			if (_thisController.WorldMatrix.Forward * -1 == myThrust.WorldMatrix.Forward)
				thruster = new ControllableThruster(myThrust, ThrustDirection.Forward);
			if (_thisController.WorldMatrix.Backward * -1 == myThrust.WorldMatrix.Forward)
				thruster = new ControllableThruster(myThrust, ThrustDirection.Back);
			if(_thisController.WorldMatrix.Left * -1 == myThrust.WorldMatrix.Forward)
				thruster = new ControllableThruster(myThrust, ThrustDirection.Left);
			if(_thisController.WorldMatrix.Right * -1 == myThrust.WorldMatrix.Forward)
				thruster = new ControllableThruster(myThrust, ThrustDirection.Right);
			if(_thisController.WorldMatrix.Up * -1 == myThrust.WorldMatrix.Forward)
				thruster = new ControllableThruster(myThrust, ThrustDirection.Up);
			if (_thisController.WorldMatrix.Down * -1 == myThrust.WorldMatrix.Forward)
				thruster = new ControllableThruster(myThrust, ThrustDirection.Down);
			if (thruster == null) return;
			thruster.OnClose += CloseThruster;
			_thrusters[thruster.ThrustDirection].Add(thruster);
			_thrustCalculationDirty = true;
		}

		private void CloseThruster(BaseClosableClass thruster)
		{
			thruster.OnClose -= CloseThruster;
			ControllableThruster closedThruster = (ControllableThruster) thruster;
			_thrusters[closedThruster.ThrustDirection].Remove(closedThruster);
			_thrustCalculationDirty = true;
		}

		private void SetPlanetaryInfluence()
		{
			_planetaryInfluence = ThrusterCalculations.PlanetaryInfluence(_thisController);
		}

		private bool _thrustCalculationDirty;
		private long _lastUpdateTick;
		private float _planetaryInfluence;
		private long _thrustRecalculationLimit = Common.Settings.TicksPerSecond * 5;
		
		private void RecalculateMaxEffectiveThrust(long tick)
		{
			if (IsClosed) return;
			if (tick - _lastUpdateTick < _thrustRecalculationLimit) _thrustCalculationDirty = true;
			if (!_thrustCalculationDirty) return;
			_lastUpdateTick = tick;
			ResetMaxThrust();
			SetPlanetaryInfluence();
			bool inAtmosphere = _planetaryInfluence > 0;
			foreach (KeyValuePair<ThrustDirection, ConcurrentCachingList<ControllableThruster>> thrusterType in _thrusters)
			{
				foreach (ControllableThruster thruster in thrusterType.Value)
				{
					_maxThrust[thruster.ThrustDirection] += thruster.AdjustedMaxThrust(inAtmosphere, _planetaryInfluence);
				}
			}
			_thrustCalculationDirty = false;
		}

		private void ResetMaxThrust()
		{
			_maxThrust[ThrustDirection.Forward] = 0;
			_maxThrust[ThrustDirection.Back] = 0;
			_maxThrust[ThrustDirection.Up] = 0;
			_maxThrust[ThrustDirection.Down] = 0;
			_maxThrust[ThrustDirection.Left] = 0;
			_maxThrust[ThrustDirection.Right] = 0;
		}
	}
}