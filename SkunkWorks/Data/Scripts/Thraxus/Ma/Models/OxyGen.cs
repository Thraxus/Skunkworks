using System.Text;
using Sandbox.Game.Entities;
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
		private const float BaseOxyMaxOutput = 1500f;
		private const float BaseHydroMaxOutput = 3000f;

		private readonly MyDefinitionId _oxyDef = new MyDefinitionId(typeof(MyObjectBuilder_GasProperties), "Oxygen");
		private readonly MyDefinitionId _hydroDef = new MyDefinitionId(typeof(MyObjectBuilder_GasProperties), "Hydrogen");
		private MyResourceSourceComponent Source => _thisCubeBlock.Components.Get<MyResourceSourceComponent>();

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
			_thisGenerator.AddUpgradeValue(Power, 1f);
			_thisGenerator.AddUpgradeValue(Yield, 1f);
			_thisGenerator.AddUpgradeValue(Speed, 1f);
		}

		public void StartWorking()
		{
			_thisGenerator.UseConveyorSystem = true;
			WriteToLog("DefinedOutput", $"{Source.DefinedOutput}", LogType.General);
			WriteToLog("CurrentOutput", $"{Source.CurrentOutput}", LogType.General);
			WriteToLog("Max Oxy Output", $"{Source.MaxOutputByType(_oxyDef)}", LogType.General);
			WriteToLog("Max Hydro Output", $"{Source.MaxOutputByType(_hydroDef)}", LogType.General);
			WriteToLog("Group", $"{Source.Group} | {Source.ResourceTypes.Count}", LogType.General);
			UpdateTerminal();
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
				IMyTerminalBlock sorter = _thisCubeBlock as IMyTerminalBlock;
				if (sorter == null) return;
				sorter.ShowOnHUD = !sorter.ShowOnHUD;
				sorter.ShowOnHUD = !sorter.ShowOnHUD;
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

		private void UpdateInfo(StringBuilder detailedInfo)
		{
			detailedInfo.AppendStringBuilder(MyTexts.Get(MySpaceTexts.BlockPropertiesText_RequiredInput));
			MyValueFormatter.AppendWorkInBestUnit(_thisGenerator.ResourceSink.RequiredInputByType(MyResourceDistributorComponent.ElectricityId), detailedInfo);
			detailedInfo.AppendFormat("\n\n");
			detailedInfo.Append("Power Efficiency: ");
			detailedInfo.Append(((1f/_thisGenerator.PowerConsumptionMultiplier) * 100f).ToString(" 0"));
			detailedInfo.Append("%\n");
			detailedInfo.Append("Resource Efficiency: ");
			detailedInfo.Append(((1f/_thisGenerator.ProductionCapacityMultiplier) * 100.0).ToString(" 0"));
			detailedInfo.Append("%\n");
			detailedInfo.Append("Speed Multiplier: ");
			detailedInfo.Append((_thisGenerator.UpgradeValues[Speed] * 100.0).ToString(" 0"));
			detailedInfo.Append("%\n");
		}

		private readonly object _syncLock = new object();
		
		private void OnUpgradeValuesChanged()
		{
			lock (_syncLock)
			{
				float power = 1;
				_thisGenerator.UpgradeValues.TryGetValue(Power, out power);
				float yield = 1;
				_thisGenerator.UpgradeValues.TryGetValue(Yield, out yield);
				float speed = 1;
				_thisGenerator.UpgradeValues.TryGetValue(Speed, out speed);

				_thisGenerator.PowerConsumptionMultiplier = (BasePowerConsumptionMultiplier / power) * speed * yield; // Power Efficiency
				_thisGenerator.ProductionCapacityMultiplier = (BaseProductionCapacityMultiplier / (yield >= 1 ? yield : 1) * (speed > 1 ? (speed * 0.15f) + 1 : speed)); // Yield

				Source.SetMaxOutputByType(_oxyDef, BaseOxyMaxOutput * speed);
				Source.SetMaxOutputByType(_hydroDef, BaseHydroMaxOutput * speed);

				_thisTerminalBlock.RefreshCustomInfo();
			}
		}
	}
}