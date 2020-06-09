using Sandbox.Game.Entities;
using Sandbox.ModAPI;
using SkunkWorks.Thraxus.Common.BaseClasses;
using VRage.Collections;
using VRageMath;

namespace SkunkWorks.Thraxus.GridControl.Models
{
	public class ControllableThrusters : BaseLoggingClass
	{
		protected override string Id { get; } = "ControllableThrusters";

		private readonly ConcurrentCachingList<MyThrust> _forwardThrusters = new ConcurrentCachingList<MyThrust>();
		private readonly ConcurrentCachingList<MyThrust> _reverseThrusters = new ConcurrentCachingList<MyThrust>();
		private readonly ConcurrentCachingList<MyThrust> _leftThrusters = new ConcurrentCachingList<MyThrust>();
		private readonly ConcurrentCachingList<MyThrust> _rightThrusters = new ConcurrentCachingList<MyThrust>();
		private readonly ConcurrentCachingList<MyThrust> _upThrusters = new ConcurrentCachingList<MyThrust>();
		private readonly ConcurrentCachingList<MyThrust> _downThrusters = new ConcurrentCachingList<MyThrust>();

		private readonly IMyShipController _thisController;

		public ControllableThrusters(IMyShipController controller)
		{
			_thisController = controller;
		}

		public void AddNewThruster(MyThrust myThrust)
		{
			if (myThrust == null || _thisController == null) return;
			if (_thisController.WorldMatrix.Forward * -1 == myThrust.WorldMatrix.Forward)
				_forwardThrusters.Add(myThrust);
			if (_thisController.WorldMatrix.Backward * -1 == myThrust.WorldMatrix.Forward)
				_reverseThrusters.Add(myThrust);
			if (_thisController.WorldMatrix.Left * -1 == myThrust.WorldMatrix.Forward)
				_leftThrusters.Add(myThrust);
			if (_thisController.WorldMatrix.Right * -1 == myThrust.WorldMatrix.Forward)
				_rightThrusters.Add(myThrust);
			if (_thisController.WorldMatrix.Up * -1 == myThrust.WorldMatrix.Forward)
				_upThrusters.Add(myThrust);
			if (_thisController.WorldMatrix.Down * -1 == myThrust.WorldMatrix.Forward)
				_downThrusters.Add(myThrust);
		}

		public override void Close()
		{
			_forwardThrusters.ClearList();
			_reverseThrusters.ClearList();
			_leftThrusters.ClearList();
			_rightThrusters.ClearList();
			_upThrusters.ClearList();
			_downThrusters.ClearList();
			base.Close();
		}
	}
}