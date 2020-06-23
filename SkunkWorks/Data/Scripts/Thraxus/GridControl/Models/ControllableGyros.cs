using Sandbox.Game.Entities;
using Sandbox.ModAPI;
using SkunkWorks.Thraxus.Common.BaseClasses;
using VRage.Collections;
using VRage.Game.Entity;
using VRageMath;

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
			gyro.OnClose += Close;
			_gyros.Add(gyro);
			_gyros.ApplyAdditions();
		}

		private void Close(MyEntity gyro)
		{
			gyro.OnClose -= Close;
			_gyros.Remove((MyGyro) gyro);
			_gyros.ApplyRemovals();
		}

		private void ApplyGyroOverride(double pitchSpeed, double yawSpeed, double rollSpeed)
		{
			Vector3D rotationVec = new Vector3D(pitchSpeed, yawSpeed, rollSpeed);
			MatrixD shipMatrix = _thisController.WorldMatrix;
			Vector3D relativeRotationVec = Vector3D.TransformNormal(rotationVec, shipMatrix);
			foreach (IMyGyro gyro in _gyros)
			{
				MatrixD gyroMatrix = gyro.WorldMatrix;
				Vector3D transformedRotationVec = Vector3D.TransformNormal(relativeRotationVec, Matrix.Transpose(gyroMatrix));
				gyro.Pitch = (float)transformedRotationVec.X;
				gyro.Yaw = (float)transformedRotationVec.Y;
				gyro.Roll = (float)transformedRotationVec.Z;
			}
		}
	}
}