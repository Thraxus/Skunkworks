using System;
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
		private readonly MyConcurrentDictionary<ThrustDirection, float> _maxEffectiveThrust = new MyConcurrentDictionary<ThrustDirection, float>()
		{
			{ ThrustDirection.Forward, 0 },
			{ ThrustDirection.Back, 0 },
			{ ThrustDirection.Up, 0 },
			{ ThrustDirection.Down, 0 },
			{ ThrustDirection.Left, 0 },
			{ ThrustDirection.Right, 0 },
		};
		private readonly MyConcurrentDictionary<ThrustDirection, float> _currentlyUtilizedThrust = new MyConcurrentDictionary<ThrustDirection, float>()
		{
			{ ThrustDirection.Forward, 0 },
			{ ThrustDirection.Back, 0 },
			{ ThrustDirection.Up, 0 },
			{ ThrustDirection.Down, 0 },
			{ ThrustDirection.Left, 0 },
			{ ThrustDirection.Right, 0 },
		};
		private readonly MyConcurrentDictionary<ThrustDirection, float> _currentlyRequiredThrust = new MyConcurrentDictionary<ThrustDirection, float>()
		{
			{ ThrustDirection.Forward, 0 },
			{ ThrustDirection.Back, 0 },
			{ ThrustDirection.Up, 0 },
			{ ThrustDirection.Down, 0 },
			{ ThrustDirection.Left, 0 },
			{ ThrustDirection.Right, 0 },
		};

		private readonly IMyShipController _thisController;


		public event Action<ThrustDirection> InsufficientThrustAvailable;

		private bool _thrustCalculationDirty;
		private long _lastUpdateTick;
		private float _planetaryInfluence;
		private long _thrustRecalculationLimit = Common.Settings.TicksPerSecond * 5;
		

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

		public void RecalculateMaxEffectiveThrust(long tick)
		{
			if (IsClosed) return;
			if (tick - _lastUpdateTick < _thrustRecalculationLimit) _thrustCalculationDirty = true;
			if (!_thrustCalculationDirty) return;
			_lastUpdateTick = tick;
			ResetMaxThrust();
			ResetUtilizedThrust();
			SetPlanetaryInfluence();
			bool inAtmosphere = _planetaryInfluence > 0;
			foreach (KeyValuePair<ThrustDirection, ConcurrentCachingList<ControllableThruster>> thrusterType in _thrusters)
			{
				foreach (ControllableThruster thruster in thrusterType.Value)
				{
					_maxEffectiveThrust[thruster.ThrustDirection] += thruster.CalculateAdjustedMaxThrust(inAtmosphere, _planetaryInfluence);
					_currentlyUtilizedThrust[thruster.ThrustDirection] += thruster.CurrentThrust();
				}
			}
			ValidateRequiredThrust();
			_thrustCalculationDirty = false;
		}

		private void ValidateRequiredThrust()
		{
			foreach (KeyValuePair<ThrustDirection, float> requiredThrust in _currentlyRequiredThrust)
			{
				if (_maxEffectiveThrust[requiredThrust.Key] > requiredThrust.Value)
					InsufficientThrustAvailable?.Invoke(requiredThrust.Key);
				if (_currentlyUtilizedThrust[requiredThrust.Key] < requiredThrust.Value)
				{
					IncreaseThrust(requiredThrust.Key, requiredThrust.Value - _currentlyUtilizedThrust[requiredThrust.Key], false);
					continue;
				}
				if (!(_currentlyUtilizedThrust[requiredThrust.Key] > requiredThrust.Value)) continue;
				DecreaseThrust(requiredThrust.Key, _currentlyUtilizedThrust[requiredThrust.Key] - requiredThrust.Value, false);
			}
		}

		private void ResetMaxThrust()
		{
			_maxEffectiveThrust[ThrustDirection.Forward] = 0;
			_maxEffectiveThrust[ThrustDirection.Back] = 0;
			_maxEffectiveThrust[ThrustDirection.Up] = 0;
			_maxEffectiveThrust[ThrustDirection.Down] = 0;
			_maxEffectiveThrust[ThrustDirection.Left] = 0;
			_maxEffectiveThrust[ThrustDirection.Right] = 0;
		}

		private void ResetUtilizedThrust()
		{
			_currentlyUtilizedThrust[ThrustDirection.Forward] = 0;
			_currentlyUtilizedThrust[ThrustDirection.Back] = 0;
			_currentlyUtilizedThrust[ThrustDirection.Up] = 0;
			_currentlyUtilizedThrust[ThrustDirection.Down] = 0;
			_currentlyUtilizedThrust[ThrustDirection.Left] = 0;
			_currentlyUtilizedThrust[ThrustDirection.Right] = 0;
		}
		
		private void ResetRequiredThrust()
		{
			_currentlyRequiredThrust[ThrustDirection.Forward] = 0;
			_currentlyRequiredThrust[ThrustDirection.Back] = 0;
			_currentlyRequiredThrust[ThrustDirection.Up] = 0;
			_currentlyRequiredThrust[ThrustDirection.Down] = 0;
			_currentlyRequiredThrust[ThrustDirection.Left] = 0;
			_currentlyRequiredThrust[ThrustDirection.Right] = 0;
		}

		private float GetMaxEffectiveThrust(ThrustDirection direction)
		{
			return _maxEffectiveThrust[direction];
		}

		private void IncreaseThrust(ThrustDirection direction, float value, bool setRequired = true)
		{
			if (IsClosed) return;
			float tmpValue = value;
			if (setRequired) _currentlyRequiredThrust[direction] += value;
			if (GetMaxEffectiveThrust(direction) - _currentlyUtilizedThrust[direction] > value) InsufficientThrustAvailable?.Invoke(direction);
			foreach (ControllableThruster thruster in _thrusters[direction])
			{
				float thrustRemaining = thruster.AdjustedMaxThrust - thruster.CurrentThrust();
				if (thrustRemaining <= 0) continue;
				if (thrustRemaining > tmpValue)
				{
					thruster.SetThrust(thruster.CurrentThrust() + tmpValue);
					_currentlyUtilizedThrust[direction] += thruster.CurrentThrust();
					break;
				}
				thruster.SetThrust(thruster.CurrentThrust() + thrustRemaining);
				_currentlyUtilizedThrust[direction] += thruster.CurrentThrust();
				tmpValue -= thrustRemaining;
				if (tmpValue > 0) continue;
				break;
			}
		}

		public void DecreaseThrust(ThrustDirection direction, float value, bool setRequired = true)
		{
			if (IsClosed) return;
			float tmpValue = value;
			if (setRequired) _currentlyRequiredThrust[direction] -= value;
			if (_currentlyRequiredThrust[direction] < 0) _currentlyRequiredThrust[direction] = 0;
			foreach (ControllableThruster thruster in _thrusters[direction])
			{
				float currentThrust = thruster.CurrentThrust();
				if (currentThrust <= 0) continue;
				if (currentThrust >= tmpValue)
				{
					thruster.SetThrust(currentThrust - tmpValue);
					_currentlyUtilizedThrust[direction] -= currentThrust - tmpValue;
					break;
				}
				thruster.SetThrust(0);
				tmpValue -= currentThrust;
				if (tmpValue > 0) continue;
				break;
			}
		}
	}
}