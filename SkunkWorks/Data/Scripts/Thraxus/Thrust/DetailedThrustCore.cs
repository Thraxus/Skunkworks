using Sandbox.Game.Entities;
using Sandbox.ModAPI;
using SkunkWorks.Thraxus.Common.BaseClasses;
using SkunkWorks.Thraxus.Common.Enums;
using SkunkWorks.Thraxus.Thrust.Models;
using VRage.Collections;
using VRage.Game.Components;
using VRage.Game.Entity;

namespace SkunkWorks.Thraxus.Thrust
{
	[MySessionComponentDescriptor(MyUpdateOrder.BeforeSimulation, priority: int.MinValue + 1)]
	public class DetailedThrustCore : BaseSessionComp
	{
		protected override string CompName { get; } = "DetailedThrustCore";
		protected override CompType Type { get; } = CompType.Server;
		protected override MyUpdateOrder Schedule { get; } = MyUpdateOrder.BeforeSimulation;

		private readonly MyConcurrentList<DetailedThruster> _thrusters = new MyConcurrentList<DetailedThruster>();

		protected override void SuperEarlySetup()
		{
			base.SuperEarlySetup();
			MyEntities.OnEntityCreate += OnEntityCreate;
			MyAPIGateway.Utilities.MessageEntered += ChatMessageHandler;
		}

		private void ChatMessageHandler(string message, ref bool sendToOthers)
		{
			if (!message.StartsWith("go")) return;
			StartWorking();
		}
		
		private void StartWorking()
		{
			foreach (DetailedThruster generator in _thrusters)
			{
				//generator.StartWorking();
			}
		}

		public override void UpdateBeforeSimulation()
		{
			base.UpdateBeforeSimulation();
			WriteToLog("UpdateBeforeSimulation", $"Updating Thrusters", LogType.General);
			foreach (DetailedThruster thruster in _thrusters)
			{
				thruster.Update();
			}
		}

		private void OnEntityCreate(MyEntity ent)
		{

			//// Demo of adding an inventory with constraints to a block.  
			//IMyBeacon beacon = ent as IMyBeacon;
			//if (beacon != null)
			//{
			//	if (!beacon.HasInventory)
			//	{
			//		MyInventoryConstraint constraint = new MyInventoryConstraint("testConstraint")
			//		{
			//			m_useDefaultIcon = false,
			//			Icon = $@"Textures\GUI\Icons\filter_uranium.dds",
			//			IsWhitelist = true
			//		};
			//		constraint.Add(new MyDefinitionId(typeof(MyObjectBuilder_Component), "InteriorPlate"));
			//		MyInventoryBase inventory = new MyInventory(5000, Vector3.One, MyInventoryFlags.CanReceive | MyInventoryFlags.CanSend)
			//		{
			//			Constraint = constraint
			//		};
			//		beacon.Components.Add(inventory);
			//	}
			//}


			IMyThrust thrust = ent as IMyThrust;
			if (thrust == null) return;
			DetailedThruster detailedThrust = new DetailedThruster(thrust);
			detailedThrust.OnWriteToLog += WriteToLog;
			_thrusters.Add(detailedThrust);
		}

		protected override void Unload()
		{
			MyEntities.OnEntityCreate -= OnEntityCreate;
			MyAPIGateway.Utilities.MessageEntered -= ChatMessageHandler;
			foreach (DetailedThruster thruster in _thrusters)
			{
				thruster.Close();
				thruster.OnWriteToLog -= WriteToLog;
			}
			base.Unload();
		}
	}
}