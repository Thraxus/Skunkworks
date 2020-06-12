﻿using Sandbox.Definitions;
using Sandbox.Game.Entities;
using Sandbox.ModAPI;
using SkunkWorks.Thraxus.Common.BaseClasses;
using SkunkWorks.Thraxus.Common.Utilities.Statics;
using SkunkWorks.Thraxus.GridControl.DataTypes.Enums;
using VRage.ModAPI;

namespace SkunkWorks.Thraxus.GridControl.Models
{
	public class ControllableThruster : BaseClosableClass
	{
		private readonly IMyThrust _thisIThruster;
		private readonly MyThrust _thisThruster;
		private readonly MyThrustDefinition _thisDefinition;
		public readonly ThrustDirection ThrustDirection;
		public float AdjustedMaxThrust;


		public ControllableThruster(IMyThrust thisThruster, ThrustDirection thisDirection)
		{
			_thisIThruster = thisThruster;
			ThrustDirection = thisDirection;
			_thisThruster = (MyThrust) thisThruster;
			_thisDefinition = _thisThruster.BlockDefinition;
			_thisThruster.OnClose += Close;
		}

		private void Close(IMyEntity thruster)
		{
			base.Close();
			_thisThruster.OnClose -= Close;
		}


		public float MaxPower()
		{
			return _thisThruster.MaxPowerConsumption;
		}

		public float CalculateAdjustedMaxPower(bool inAtmosphere)
		{
			return ThrusterCalculations.AdjustedMaxPower(_thisThruster, inAtmosphere);
		}

		public float MaxThrust()
		{
			return _thisDefinition.ForceMagnitude * _thisIThruster.ThrustMultiplier;
		}

		public float CalculateAdjustedMaxThrust(bool inAtmosphere)
		{
			AdjustedMaxThrust = _thisDefinition.ForceMagnitude * _thisIThruster.ThrustMultiplier * ThrusterCalculations.CalculatedThrustScalar(_thisThruster, inAtmosphere);
			return AdjustedMaxThrust;
		}

		public float CalculateAdjustedMaxThrust(bool inAtmosphere, float planetaryInfluence)
		{
			AdjustedMaxThrust = _thisDefinition.ForceMagnitude * _thisIThruster.ThrustMultiplier * ThrusterCalculations.CalculatedThrustScalar(_thisThruster, inAtmosphere, planetaryInfluence);
			return AdjustedMaxThrust;
		}

		public float CurrentThrust()
		{
			return _thisIThruster.ThrustOverride;
		}

		public void SetThrust(float value)
		{
			if (value < 0) return;
			_thisIThruster.ThrustOverride = value;
		}
	}
}