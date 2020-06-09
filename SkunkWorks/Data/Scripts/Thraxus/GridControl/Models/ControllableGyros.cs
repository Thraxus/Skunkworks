using Sandbox.Game.Entities;
using Sandbox.ModAPI;
using SkunkWorks.Thraxus.Common.BaseClasses;
using VRage.Collections;

namespace SkunkWorks.Thraxus.GridControl.Models
{
	public class ControllableGyros : BaseLoggingClass
	{
		protected override string Id { get; } = "ControllableGyros";

		private readonly ConcurrentCachingList<MyGyro> _gyros = new ConcurrentCachingList<MyGyro>();

		private readonly IMyShipController _thisController;

		public ControllableGyros(IMyShipController thisController)
		{
			_thisController = thisController;
		}

		public override void Close()
		{
			base.Close();
			_gyros.ClearList();
		}

		public void Add(MyGyro gyro)
		{
			_gyros.Add(gyro);
			_gyros.ApplyAdditions();
		}
	}
}