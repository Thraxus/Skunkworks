using SkunkWorks.Thraxus.Common.BaseClasses;
using SkunkWorks.Thraxus.Common.Enums;
using SkunkWorks.Thraxus.Development;
using VRage.Game.Components;

namespace SkunkWorks.Thraxus
{
	[MySessionComponentDescriptor(MyUpdateOrder.BeforeSimulation, priority: int.MinValue + 1)]
	public class SkunkWorksCore : BaseSessionComp
	{
		protected override string CompName { get; } = "SkunkWorksCore";
		
		protected override CompType Type { get; } = CompType.Server;

		protected override MyUpdateOrder Schedule { get; } = MyUpdateOrder.NoUpdate;

		protected override void SuperEarlySetup()
		{
			base.SuperEarlySetup();
			ReplaceInventoryFilterIcons.Run();
		}
	}
}