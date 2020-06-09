using Sandbox.Definitions;
using Sandbox.Game.Entities;
using Sandbox.ModAPI;
using SkunkWorks.Thraxus.Common.BaseClasses;
using SkunkWorks.Thraxus.Common.Utilities.Statics;
using VRage.ModAPI;

namespace SkunkWorks.Thraxus.GridControl.Models
{
	public class ControllableThruster : BaseClosableClass
	{
		private readonly IMyThrust _thisIThruster;
		private readonly MyThrust _thisThruster;
		private readonly MyThrustDefinition _thisDefinition;

		public ControllableThruster(IMyThrust thisThruster)
		{
			_thisIThruster = thisThruster;
			_thisThruster = (MyThrust) thisThruster;
			_thisDefinition = _thisThruster.BlockDefinition;
			_thisThruster.OnClose += OnClose;
		}

		private new void OnClose(IMyEntity thruster)
		{
			Close();
			_thisThruster.OnClose -= OnClose;
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
	}
}
