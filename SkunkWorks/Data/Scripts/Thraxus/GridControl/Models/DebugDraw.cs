using SkunkWorks.Thraxus.GridControl.DataTypes;
using SkunkWorks.Thraxus.GridControl.Interfaces;
using VRage.Collections;
using VRage.Game;

namespace SkunkWorks.Thraxus.GridControl.Models
{
	public class DebugDraw : IUpdate
	{
		// MyTransparentGeometry 
		// MySimpleObjectDraw
		private readonly ConcurrentCachingList<DrawLine> _drawLines = new ConcurrentCachingList<DrawLine>();
		
		public void Update(long tick)
		{
			DrawLines();
		}

		private void DrawLines()
		{
			foreach (DrawLine line in _drawLines)
			{
				//MySimpleObjectDraw.DrawLine(line.From, line.To, line.Material, ref line.Color, line.Thickness);
				MyTransparentGeometry.AddLineBillboard(line.Material, line.Color, line.From, line.DirectionNormalized, line.Length, line.Thickness);
			}
		}

		public void DrawLine(DrawLine line)
		{
			//MySimpleObjectDraw.DrawLine(line.From, line.To, line.Material, ref line.Color, 1);
			MyTransparentGeometry.AddLineBillboard(line.Material, line.Color, line.From, line.DirectionNormalized, line.Length, line.Thickness);
		}

		public void AddLine(DrawLine line)
		{
			_drawLines.Add(line);
			_drawLines.ApplyAdditions();
		}

		public void RemoveLine(DrawLine line)
		{
			_drawLines.Remove(line);
			_drawLines.ApplyRemovals();
		}

		public void ClearLines()
		{
			_drawLines.ClearList();
			_drawLines.ApplyRemovals();
		}
	}
}
