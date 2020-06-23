using System.Text;
using Sandbox.ModAPI;
using SkunkWorks.Thraxus.Common.BaseClasses;
using VRage.Game.GUI.TextPanel;

namespace SkunkWorks.Thraxus.GridControl.Models
{
	public class DebugScreens : BaseClosableClass
	{
		private IMyTextSurface _leftSurface;
		private IMyTextSurface _rightSurface;

		public void AddLeftScreen(IMyTextSurface leftSurface)
		{
			if (_leftSurface != null) return;
			_leftSurface = leftSurface;
			_leftSurface.ContentType = ContentType.TEXT_AND_IMAGE;
		}

		public void AddRightScreen(IMyTextSurface rightSurface)
		{
			if (_rightSurface != null) return;
			_rightSurface = rightSurface;
			_rightSurface.ContentType = ContentType.TEXT_AND_IMAGE;
		}

		public void WriteToLeft(StringBuilder value, bool append = false)
		{
			_leftSurface?.WriteText(value, append);
		}

		public void WriteToRight(StringBuilder value, bool append = false)
		{
			_rightSurface?.WriteText(value, append);
		}
	}
}
