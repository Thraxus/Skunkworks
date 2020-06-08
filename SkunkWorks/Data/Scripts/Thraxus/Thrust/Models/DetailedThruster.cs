using System;
using System.Collections.Generic;
using System.Text;
using Sandbox.Definitions;
using Sandbox.Game.Entities;
using Sandbox.Game.EntityComponents;
using Sandbox.Game.GameSystems;
using Sandbox.Game.World;
using Sandbox.ModAPI;
using SkunkWorks.Thraxus.Common.BaseClasses;
using VRage.Game;
using VRage.Game.Entity;
using VRage.Game.ModAPI;
using VRage.ModAPI;
using VRage.Utils;
using VRageMath;

namespace SkunkWorks.Thraxus.Thrust.Models
{
	public class DetailedThruster : BaseLoggingClass
	{
		protected override string Id { get; }

		private readonly MyEntity _thisEntity;
		private readonly IMyEntity _thisIEntity;
		private readonly MyThrust _thisThrust;
		private readonly IMyThrust _thisIThrust;
		private readonly IMyTerminalBlock _thisTerminal;
		private readonly MyCubeBlock _thisBlock;
		private readonly IMyCubeBlock _thisIBlock;
		private readonly MyThrustDefinition _thisThrustDefinition;

		private MyResourceSinkComponent MySink => ((MyResourceSinkComponent)_thisIThrust.ResourceSink);

		private bool _isClosed;
		private bool _isSetup;

		public DetailedThruster(IMyEntity thruster)
		{
			Id = thruster.EntityId.ToString();
			_thisEntity = (MyEntity)thruster;
			_thisIEntity = thruster;
			_thisThrust = (MyThrust) thruster;
			_thisIThrust = (IMyThrust) thruster;
			_thisTerminal = (IMyTerminalBlock) thruster;
			_thisBlock = (MyCubeBlock) thruster;
			_thisIBlock = (IMyCubeBlock) thruster;
			_thisThrustDefinition = _thisThrust.BlockDefinition;
			
			
			_thisEntity.AddedToScene += OnAddedToScene;
		}

		public override void Close()
		{
			if (_isClosed || !_isSetup) return;
			_isClosed = true;
			_thisTerminal.AppendingCustomInfo -= AppendingCustomInfo;
		}

		private void OnAddedToScene(MyEntity entity)
		{
			if (entity != _thisEntity || _isSetup) return;
			_thisTerminal.AppendingCustomInfo += AppendingCustomInfo;
			_thisEntity.AddedToScene -= OnAddedToScene;
			_thisTerminal.RefreshCustomInfo();
			_isSetup = true;
		}

		private void MySinkOnCurrentInputChanged(MyDefinitionId resourcetypeid, float oldinput, MyResourceSinkComponent sink)
		{
			_thisTerminal.RefreshCustomInfo();
		}

		private void OnRequiredInputChanged(MyDefinitionId unused1, MyResourceSinkComponent unused2, float unused3, float unused4)
		{
			_thisTerminal.RefreshCustomInfo();
		}

		public void Update()
		{
			if (!_isSetup) return;
			if (_thisTerminal.CustomData.Contains("neutral")) _thisThrust.ThrustOverride = CalculateRequiredLift();
			_thisTerminal.RefreshCustomInfo();
		}

		private void AppendingCustomInfo(IMyTerminalBlock block, StringBuilder value)
		{
			if (block != _thisTerminal || _isClosed) return;
			UpdateInfo(value);
			UpdateTerminal();
		}

		private void UpdateTerminal()
		{
			MyOwnershipShareModeEnum shareMode;
			long ownerId;
			if (_thisBlock.IDModule != null)
			{
				ownerId = _thisBlock.IDModule.Owner;
				shareMode = _thisBlock.IDModule.ShareMode;
			}
			else
			{
				IMyTerminalBlock sorter = _thisBlock as IMyTerminalBlock;
				if (sorter == null) return;
				sorter.ShowOnHUD = !sorter.ShowOnHUD;
				sorter.ShowOnHUD = !sorter.ShowOnHUD;
				return;
			}
			_thisBlock.ChangeOwner(ownerId, shareMode == MyOwnershipShareModeEnum.None ? MyOwnershipShareModeEnum.Faction : MyOwnershipShareModeEnum.None);
			_thisBlock.ChangeOwner(ownerId, shareMode);
		}

		private int i = 0;
		private void UpdateInfo(StringBuilder detailedInfo)
		{
			detailedInfo.Append("\n");
			detailedInfo.Append($"Adjusted Max Power: ");
			MyValueFormatter.AppendWorkInBestUnit(AdjustedMaxPower(), detailedInfo);
			detailedInfo.Append("\n");
			detailedInfo.Append($"Current Power: ");
			MyValueFormatter.AppendWorkInBestUnit(CalculatedCurrentPower(), detailedInfo);
			detailedInfo.Append("\n");
			detailedInfo.Append($"Calculated Max Thrust: ");
			MyValueFormatter.AppendForceInBestUnit(CalculatedMaxThrust(), detailedInfo);
			detailedInfo.Append("\n");
			detailedInfo.Append($"Current Thrust: ");
			MyValueFormatter.AppendForceInBestUnit(_thisIThrust.CurrentThrust, detailedInfo);
			detailedInfo.Append("\n");
			detailedInfo.Append($"Required Lift: ");
			MyValueFormatter.AppendForceInBestUnit(CalculateRequiredLift(), detailedInfo);
			detailedInfo.Append("\n");
			detailedInfo.Append($"CalculatedThrustScalar: {CalculatedThrustScalar()} \n");
			detailedInfo.Append($"Test: {i++} \n");
		}

		private float PlanetaryInfluence()
		{
			BoundingBoxD box = _thisEntity.PositionComp.WorldAABB;
			MyPlanet closestPlanet = MyGamePruningStructure.GetClosestPlanet(ref box);
			if (closestPlanet == null) return 0;
			MyGravityProviderComponent myGravityProviderComponent = closestPlanet.Components.Get<MyGravityProviderComponent>();
			return closestPlanet.GetAirDensity(box.Center) * myGravityProviderComponent.GetGravityMultiplier(box.Center);
		}

		private float CalculatedMaxThrust(bool inAtmosphere = true)
		{
			return _thisThrust.BlockDefinition.ForceMagnitude * _thisIThrust.ThrustMultiplier * CalculatedThrustScalar(inAtmosphere);
		}

		private float AdjustedMaxPower()
		{	// Atmospheric thrusters get scaled based on where in the atmospheric layer they are
			return _thisThrust.MaxPowerConsumption * CalculatedThrustScalar();
		}
		
		private float CalculatedCurrentPower()
		{
			return (_thisIThrust.CurrentThrust / _thisIThrust.MaxThrust) * _thisThrust.MaxPowerConsumption;
		}
		
		private float CalculatedThrustScalar(bool inAtmosphere = true)
		{
			float result = 1f;
			if (_thisThrustDefinition.NeedsAtmosphereForInfluence && !inAtmosphere)
			{
				result = _thisThrustDefinition.EffectivenessAtMinInfluence;
			}
			else if (Math.Abs(_thisThrustDefinition.MaxPlanetaryInfluence - _thisThrustDefinition.MinPlanetaryInfluence) > 0)
			{
				float value = (PlanetaryInfluence() - _thisThrustDefinition.MinPlanetaryInfluence) * _thisThrustDefinition.InvDiffMinMaxPlanetaryInfluence;
				result = MathHelper.Lerp(_thisThrustDefinition.EffectivenessAtMinInfluence, _thisThrustDefinition.EffectivenessAtMaxInfluence, MathHelper.Clamp(value, 0f, 1f));
			}
			return result;
		}

		private float CalculateRequiredLift()
		{
			BoundingBoxD box = _thisEntity.PositionComp.WorldAABB;
			MyPlanet closestPlanet = MyGamePruningStructure.GetClosestPlanet(ref box);
			if (closestPlanet == null) return 0;
			MyGravityProviderComponent myGravityProviderComponent = closestPlanet.Components.Get<MyGravityProviderComponent>();
			MatrixD worldMatrixRef = _thisEntity.PositionComp.WorldMatrixRef;
			//MySphericalNaturalGravityComponent naturalGravity = closestPlanet.Components.Get<MySphericalNaturalGravityComponent>();
			//myGravityProviderComponent.GetWorldGravity(box.Center);
			return _thisBlock.CubeGrid.Mass * myGravityProviderComponent.GetGravityMultiplier(worldMatrixRef.Translation) * GravityConstant;
		}

		private const float GravityConstant = 9.81f; 
	}
}

