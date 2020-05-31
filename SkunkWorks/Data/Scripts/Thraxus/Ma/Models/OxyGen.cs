using System.Collections.Generic;
using System.Text;
using Sandbox.Definitions;
using Sandbox.Game.Entities;
using Sandbox.Game.Entities.Cube;
using Sandbox.Game.EntityComponents;
using Sandbox.Game.Localization;
using Sandbox.ModAPI;
using SkunkWorks.Thraxus.Common.BaseClasses;
using SkunkWorks.Thraxus.Common.Enums;
using VRage;
using VRage.Game;
using VRage.Game.Entity;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.ModAPI;
using VRage.Utils;
using VRageRender;

namespace SkunkWorks.Thraxus.Ma.Models
{
	public class OxyGen : BaseClosableLoggingClass
	{
		protected sealed override string Id { get; } = "OxyGen";

		public override void Close()
		{
			base.Close();
			_thisGenerator.OnUpgradeValuesChanged -= OnUpgradeValuesChanged;
			_thisGenerator.AppendingCustomInfo -= AppendingCustomInfo;
			_thisGenerator.OnClose -= OnClose;
			WriteToLog("Close:", $"I'm out! {_thisGenerator.EntityId}", LogType.General);
		}
		
		private readonly IMyGasGenerator _thisGenerator;
		private readonly IMyTerminalBlock _thisTerminalBlock;
		private readonly MyCubeBlock _thisCubeBlock;
		private readonly MyEntity _thisEntity;
		private const string Power = "PowerEfficiency";
		private const string Yield = "Effectiveness";
		private const string Speed = "Productivity";
		private const float BasePowerConsumptionMultiplier = 1f;
		private const float BaseProductionCapacityMultiplier = 1f;

		public OxyGen(IMyGasGenerator thisGenerator)
		{
			Id = $"{Id} ({thisGenerator.EntityId})";
			_thisGenerator = thisGenerator;
			_thisTerminalBlock = thisGenerator;
			_thisCubeBlock = (MyCubeBlock) thisGenerator;
			_thisEntity = (MyEntity) thisGenerator;
			_thisGenerator.OnUpgradeValuesChanged += OnUpgradeValuesChanged;
			_thisGenerator.AppendingCustomInfo += AppendingCustomInfo;
			_thisEntity.AddedToScene += OnAddedToScene;
			_thisGenerator.OnClose += OnClose;
			_thisGenerator.AddUpgradeValue("PowerEfficiency", 1f);
			_thisGenerator.AddUpgradeValue("Effectiveness", 1f);
			_thisGenerator.AddUpgradeValue("Productivity", 1f);
		}

		private void OnAddedToScene(MyEntity entity)
		{
			if (entity != _thisEntity) return;
			_thisTerminalBlock.RefreshCustomInfo();
			_thisEntity.AddedToScene -= OnAddedToScene;
		}

		private void AppendingCustomInfo(IMyTerminalBlock block, StringBuilder value)
		{
			if (block != _thisTerminalBlock) return;
			UpdateInfo(value);
			UpdateTerminal();
		}

		private void UpdateTerminal()
		{
			MyOwnershipShareModeEnum shareMode;
			long ownerId;
			if (_thisCubeBlock.IDModule != null)
			{
				ownerId = _thisCubeBlock.IDModule.Owner;
				shareMode = _thisCubeBlock.IDModule.ShareMode;
			}
			else
			{
				var sorter = _thisCubeBlock as IMyTerminalBlock;
				if (sorter != null)
				{
					sorter.ShowOnHUD = !sorter.ShowOnHUD;
					sorter.ShowOnHUD = !sorter.ShowOnHUD;
				}
				return;
			}
			_thisCubeBlock.ChangeOwner(ownerId, shareMode == MyOwnershipShareModeEnum.None ? MyOwnershipShareModeEnum.Faction : MyOwnershipShareModeEnum.None);
			_thisCubeBlock.ChangeOwner(ownerId, shareMode);
		}

		private void OnClose(IMyEntity obj)
		{
			Close();
		}

		public void Report()
		{
			WriteToLog("Report:", $"I'm alive: {_thisGenerator.EntityId}", LogType.General);
		}

		//private readonly StringBuilder _detailedInfo = new StringBuilder();
		private void UpdateInfo(StringBuilder detailedInfo)
		{
			detailedInfo.AppendStringBuilder(MyTexts.Get(MySpaceTexts.BlockPropertiesText_RequiredInput));
			MyValueFormatter.AppendWorkInBestUnit(_thisGenerator.ResourceSink.RequiredInputByType(MyResourceDistributorComponent.ElectricityId), detailedInfo);
			detailedInfo.AppendFormat("\n\n");
			//detailedInfo.Append(MyTexts.Get(MySpaceTexts.BlockPropertiesText_Productivity));
			//detailedInfo.Append(((float)((_thisGenerator.UpgradeValues["Productivity"]) * 100.0)).ToString(" 0"));
			detailedInfo.Append("Power Efficiency: ");
			detailedInfo.Append((_thisGenerator.UpgradeValues["PowerEfficiency"] * 100f).ToString(" 0"));
			detailedInfo.Append("%\n");
			detailedInfo.Append("Production Capacity: ");
			detailedInfo.Append((_thisGenerator.ProductionCapacityMultiplier * 100.0).ToString(" 0"));
			detailedInfo.Append("%\n");
			//detailedInfo.Append(MyTexts.Get(MySpaceTexts.BlockPropertiesText_Effectiveness));
			//detailedInfo.Append((_thisGenerator.UpgradeValues["Effectiveness"] * 100f).ToString(" 0"));
			//detailedInfo.Append("%\n");
			//detailedInfo.Append(MyTexts.Get(MySpaceTexts.BlockPropertiesText_Efficiency));
			//detailedInfo.Append((_thisGenerator.UpgradeValues["PowerEfficiency"] * 100f).ToString(" 0"));
			//detailedInfo.Append("%\n\n");
			WriteToLog("CustomInfo:", $"{detailedInfo}", LogType.General);
			//return _detailedInfo;
		}

		private readonly object _syncLock = new object();

		private void OnUpgradeValuesChanged()
		{
			lock (_syncLock)
			{
				//foreach (KeyValuePair<long, MyCubeBlock.AttachedUpgradeModule> attachedUpgrade in _thisCubeBlock.CurrentAttachedUpgradeModules)
				//{
				//	WriteToLog("Found:", $"{attachedUpgrade.Value.SlotCount} | {attachedUpgrade.Value.Compatible}", LogType.General);

				//	foreach (KeyValuePair<string, float> upgradeValue in attachedUpgrade.Value.Block.UpgradeValues)
				//	{
				//		_thisGenerator.AddUpgradeValue(upgradeValue.Key, upgradeValue.Value);
				//		WriteToLog("Adding:", $"{upgradeValue.Key} | {upgradeValue.Value}", LogType.General);
				//	}
				//}

				float power = 1;
				_thisGenerator.UpgradeValues.TryGetValue(Power, out power);
				// Reference: <Modifier>1.2228445</Modifier>
				// return (float) ((double) base.GetOperationalPowerConsumption() * (1.0 + (double) this.UpgradeValues["Productivity"]) * (1.0 / (double) this.UpgradeValues["PowerEfficiency"]));
				float yield = 1;
				_thisGenerator.UpgradeValues.TryGetValue(Yield, out yield);
				// Reference: <Modifier>1.0905077</Modifier>
				// float num = (float) result.Amount * this.m_refineryDef.MaterialEfficiency * this.UpgradeValues["Effectiveness"];
				// this.OutputInventory.AddItems((MyFixedPoint) ((float) blueprintAmount * num), (MyObjectBuilder_Base) newObject);
				float speed = 1;
				_thisGenerator.UpgradeValues.TryGetValue(Speed, out speed);
				// Reference: <Modifier>0.5</Modifier>

				_thisGenerator.PowerConsumptionMultiplier = (BasePowerConsumptionMultiplier / power) * speed * yield; // Power Efficiency
				_thisGenerator.ProductionCapacityMultiplier = (BaseProductionCapacityMultiplier * (yield > 1 ? yield * 1.25f : yield)) * (speed); // Yield
				
				if (_thisGenerator.UpgradeValues.Count <= 0) return;
				foreach (KeyValuePair<string, float> upgrade in _thisGenerator.UpgradeValues)
				{
					WriteToLog("Upgrades:", $"{upgrade}", LogType.General);
				}
				WriteToLog("PowerConsumptionMultiplier:", $"{_thisGenerator.PowerConsumptionMultiplier}", LogType.General);
				WriteToLog("ProductionCapacityMultiplier:", $"{_thisGenerator.ProductionCapacityMultiplier}", LogType.General);
				_thisTerminalBlock.RefreshCustomInfo();
			}
		}
	}
}


//protected override float GetOperationalPowerConsumption()
//{
//	return base.GetOperationalPowerConsumption() * (1f + base.UpgradeValues["Productivity"]) * (1f / base.UpgradeValues["PowerEfficiency"]);
//}
