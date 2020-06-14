using System;
using System.Collections.Generic;
using Sandbox.Game.Entities;
using Sandbox.ModAPI;
using SkunkWorks.Thraxus.Common.BaseClasses;
using SkunkWorks.Thraxus.Common.Utilities.Statics;
using SkunkWorks.Thraxus.GridControl.DataTypes.Enums;
using VRage.Collections;
using VRageRender.Messages;

namespace SkunkWorks.Thraxus.GridControl.Models
{
	public class ControllableThrusters : BaseLoggingClass
	{

		// TODO: Add a balanced option that distributes the force to all thrusters, not just one
		//			One just makes it easier to debug since i can still fly a ship manually, will likely use balanced for release
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

		private readonly IMyShipController _thisIController;
		private readonly MyShipController _thisController;

		public event Action<ThrustDirection> InsufficientThrustAvailable;
		
		public ControllableThrusters(IMyShipController controller)
		{
			_thisIController = controller;
			_thisController = (MyShipController) controller;
			_thisController.ControlThrusters = true;
			controller.GetNaturalGravity();
			_thrusters.Add(ThrustDirection.Forward, new ConcurrentCachingList<ControllableThruster>());
			_thrusters.Add(ThrustDirection.Back, new ConcurrentCachingList<ControllableThruster>());
			_thrusters.Add(ThrustDirection.Up, new ConcurrentCachingList<ControllableThruster>());
			_thrusters.Add(ThrustDirection.Down, new ConcurrentCachingList<ControllableThruster>());
			_thrusters.Add(ThrustDirection.Left, new ConcurrentCachingList<ControllableThruster>());
			_thrusters.Add(ThrustDirection.Right, new ConcurrentCachingList<ControllableThruster>());
		}

		public void AddNewThruster(MyThrust myThrust)
		{
			if (myThrust == null || _thisIController == null) return;
			ControllableThruster thruster = null;
			if (_thisIController.WorldMatrix.Forward * -1 == myThrust.WorldMatrix.Forward)
				thruster = new ControllableThruster(myThrust, ThrustDirection.Forward);
			if (_thisIController.WorldMatrix.Backward * -1 == myThrust.WorldMatrix.Forward)
				thruster = new ControllableThruster(myThrust, ThrustDirection.Back);
			if(_thisIController.WorldMatrix.Left * -1 == myThrust.WorldMatrix.Forward)
				thruster = new ControllableThruster(myThrust, ThrustDirection.Left);
			if(_thisIController.WorldMatrix.Right * -1 == myThrust.WorldMatrix.Forward)
				thruster = new ControllableThruster(myThrust, ThrustDirection.Right);
			if(_thisIController.WorldMatrix.Up * -1 == myThrust.WorldMatrix.Forward)
				thruster = new ControllableThruster(myThrust, ThrustDirection.Up);
			if (_thisIController.WorldMatrix.Down * -1 == myThrust.WorldMatrix.Forward)
				thruster = new ControllableThruster(myThrust, ThrustDirection.Down);
			if (thruster == null) return;
			thruster.OnClose += CloseThruster;
			_thrusters[thruster.ThrustDirection].Add(thruster);
			RecalculateMaxEffectiveThrust();
		}

		private void CloseThruster(BaseClosableClass thruster)
		{
			thruster.OnClose -= CloseThruster;
			ControllableThruster closedThruster = (ControllableThruster) thruster;
			_thrusters[closedThruster.ThrustDirection].Remove(closedThruster);
			RecalculateMaxEffectiveThrust();
		}

		public void RecalculateMaxEffectiveThrust()
		{
			if (IsClosed) return;
			ResetMaxEffectiveThrust();
			ResetUtilizedThrust();
			foreach (KeyValuePair<ThrustDirection, ConcurrentCachingList<ControllableThruster>> thrusterType in _thrusters)
			{
				foreach (ControllableThruster thruster in thrusterType.Value)
				{
					_maxEffectiveThrust[thruster.ThrustDirection] += thruster.MaxThrust();
					_currentlyUtilizedThrust[thruster.ThrustDirection] += thruster.CurrentThrust();
				}
			}
			RecalculateUtilizedThrust();
		}
		
		private void RecalculateUtilizedThrust()
		{
			ResetUtilizedThrust();
			foreach (KeyValuePair<ThrustDirection, ConcurrentCachingList<ControllableThruster>> thrust in _thrusters)
			{
				foreach (ControllableThruster thruster in thrust.Value)
				{
					_currentlyUtilizedThrust[thrust.Key] += thruster.CurrentThrust();
				}
			}
		}

		private void ResetMaxEffectiveThrust()
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
		
		public float GetMaxEffectiveThrustInDirection(ThrustDirection direction)
		{
			return _maxEffectiveThrust[direction];
		}
		
		public void SetThrust(ThrustDirection direction, float value)
		{
			SetRollingThrust(direction, value);
		}

		private void SetRollingThrust(ThrustDirection direction, float value)
		{
			if (IsClosed) return;
			float tmpValue = value;
			_currentlyUtilizedThrust[direction] = 0;
			foreach (ControllableThruster thruster in _thrusters[direction])
			{
				if (Math.Abs(value) <= 0)
				{
					thruster.SetThrust(0);
					continue;
				}

				float availableThrust = thruster.MaxThrust() - thruster.CurrentThrust();
				if (availableThrust <= 0)
					continue;

				if (availableThrust > tmpValue)
				{
					thruster.SetThrust(thruster.CurrentThrust() + tmpValue);
					tmpValue = 0;
				}
				else
				{
					thruster.SetThrust(thruster.MaxThrust());
					tmpValue -= thruster.MaxThrust();
				}
				if (tmpValue > 0) continue;
				thruster.SetThrust(0);
			}
			if (tmpValue > 0) 
			{
				InsufficientThrustAvailable?.Invoke(direction);
				_currentlyUtilizedThrust[direction] = value - tmpValue;
				return;
			}
			_currentlyUtilizedThrust[direction] = value;
		}

		private void SetBalancedThrust(ThrustDirection direction, float value)
		{
			if (IsClosed) return;
			if (_maxEffectiveThrust[direction] - _currentlyUtilizedThrust[direction] > value) InsufficientThrustAvailable?.Invoke(direction);
			foreach (ControllableThruster thruster in _thrusters[direction])
			{
				thruster.SetThrust(value / _thrusters[direction].Count);
			}
			_currentlyUtilizedThrust[direction] = value;
		}

	}
}