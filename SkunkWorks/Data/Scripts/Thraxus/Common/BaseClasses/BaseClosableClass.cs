using System;
using SkunkWorks.Thraxus.Common.Interfaces;
using SkunkWorks.Thraxus.GridControl.Interfaces;

namespace SkunkWorks.Thraxus.Common.BaseClasses
{
	public abstract class BaseClosableClass : IClose
	{
		public event Action<BaseClosableClass> OnClose;

		public bool IsClosed;

		public virtual void Close()
		{
			if (IsClosed) return;
			IsClosed = true;
			OnClose?.Invoke(this);
		}
	}
}