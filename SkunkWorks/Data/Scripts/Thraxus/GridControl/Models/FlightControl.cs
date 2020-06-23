using System.Collections.Generic;
using Sandbox.Game.Entities;
using Sandbox.ModAPI;
using SkunkWorks.Thraxus.Common.BaseClasses;
using SkunkWorks.Thraxus.GridControl.DataTypes;
using SkunkWorks.Thraxus.GridControl.Interfaces;
using VRageMath;

namespace SkunkWorks.Thraxus.GridControl.Models
{
	internal class FlightControl : BaseLoggingClass, IUpdate
	{
		protected override string Id { get; } = "FlightControl";

		private readonly GridSystems _gridSystems;

		private readonly MyCubeGrid _thisGrid;
		private readonly IMyShipController _thisController;
		private readonly DebugDraw _draw = new DebugDraw();

		private readonly DrawLine _thisDestination = new DrawLine();

		public FlightControl(MyCubeGrid thisGrid, IMyShipController thisController, GridSystems gridSystems)
		{
			_thisGrid = thisGrid;
			_thisController = thisController;
			_gridSystems = gridSystems;

			_thisDestination.Color = new Vector4(255, 0, 0, 1);
			_thisDestination.To = _waypoints[0];
			_thisDestination.Thickness = 0.50f;
			AddDrawLine();
			//List<IMyGps> gpsList = MyAPIGateway.Session.GPS.GetGpsList(MyAPIGateway.Session.LocalHumanPlayer.IdentityId);
		}


		public void Update(long tick)
		{
			_thisDestination.From = _thisController.GetPosition() + (_thisController.WorldMatrix.Up * 1);
			_thisDestination.Set();
			_draw.DrawLine(_thisDestination);
			_draw.Update(tick);
		}

		private void AddDrawLine()
		{
			for (int i = 0; i < _waypoints.Count - 1; i++)
			{
				DrawLine line = new DrawLine()
				{
					From = _waypoints[i],
					To = _waypoints[i + 1]
				};
				line.Set();
				_draw.AddLine(line);
			}
			DrawLine line2 = new DrawLine()
			{
				From = _waypoints[_waypoints.Count - 1],
				To = _waypoints[0]
			};
			line2.Set();
			_draw.AddLine(line2);
		}

		private readonly List<Vector3D> _waypoints = new List<Vector3D>
		{
			new Vector3D(-43229.021168609252, -9269.5177582804135, 43489.220552603845),
			new Vector3D(-42215.248862785862, -10106.238471936351, 44052.730770789916),
			new Vector3D(-41429.448022182289, -9966.6928187007179, 45368.193750827246),
			new Vector3D(-41763.776891319809, -8156.2750849056138, 45627.912503207961),
			new Vector3D(-42359.304601329386, -6601.9854178186824, 43311.280304341512),
			new Vector3D(-44799.281991765121, -7653.6194506676993, 42228.293961479911),
			new Vector3D(-43120.573476366189, -9248.4646031342054, 42960.09875162087)
		};

		public override void Close()
		{
			base.Close();
			_draw.ClearLines();
		}
	}
}