using Sandbox.Common.ObjectBuilders;
using VRage.Game;
using VRage.Game.Components;
using VRage.ObjectBuilders;
using VRage.Utils;

namespace SkunkWorks.Thraxus
{
	[MyEntityComponentDescriptor(typeof(MyObjectBuilder_Reactor), false, "ThraxusCustomReactor")]
	public class SkunkWorksGameLogic : MyGameLogicComponent
	{
		public override void Init(MyObjectBuilder_EntityBase objectBuilder)
		{

			//foreach (MyObjectBuilder_ComponentContainer.ComponentData componentData in objectBuilder.ComponentContainer.Components)
			//{
			//	if (componentData.TypeId != "MyInventoryBase") continue;
			//	MyInventoryConstraint x = new MyInventoryConstraint();
			//	((MyObjectBuilder_Inventory)componentData.Component)?.
			//	((MyObjectBuilder_Inventory)componentData.Component)?.Items.Add(
			//		new MyObjectBuilder_InventoryItem
			//		{
			//			Amount = amount,
			//			PhysicalContent = item
			//		});
			//}
			base.Init(objectBuilder);
		}

		public override void Init(MyComponentDefinitionBase definition)
		{
			MyLog.Default.WriteLine($"SkunkWorks: {definition?.Id}");
			base.Init(definition);
		}
	}
}
