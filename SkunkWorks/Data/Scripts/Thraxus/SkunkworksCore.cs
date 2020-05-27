using SkunkWorks.Thraxus.Common.BaseClasses;
using VRage.Game.Components;

namespace SkunkWorks.Thraxus
{
	[MySessionComponentDescriptor(MyUpdateOrder.NoUpdate)]
	public class SkunkWorksCore : BaseServerSessionComp
	{
		private const string SessionName = "SkunkWorksCore";
		public SkunkWorksCore() : base(SessionName) { }


		public override void LoadData()
		{

			base.LoadData();
		}

		public override void BeforeStart()
		{

			base.BeforeStart();
		}

		
	}
}