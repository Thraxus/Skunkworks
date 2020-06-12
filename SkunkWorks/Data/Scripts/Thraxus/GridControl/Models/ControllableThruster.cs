using Sandbox.Definitions;
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

		public float AdjustedMaxPower(bool inAtmosphere)
		{
			return ThrusterCalculations.AdjustedMaxPower(_thisThruster, inAtmosphere);
		}

		public float MaxThrust()
		{
			return _thisDefinition.ForceMagnitude * _thisIThruster.ThrustMultiplier;
		}

		public float AdjustedMaxThrust(bool inAtmosphere)
		{
			return _thisDefinition.ForceMagnitude* _thisIThruster.ThrustMultiplier * ThrusterCalculations.CalculatedThrustScalar(_thisThruster, inAtmosphere);
		}

		public float AdjustedMaxThrust(bool inAtmosphere, float planetaryInfluence)
		{
			return _thisDefinition.ForceMagnitude * _thisIThruster.ThrustMultiplier * ThrusterCalculations.CalculatedThrustScalar(_thisThruster, inAtmosphere, planetaryInfluence);
		}
	}
}
