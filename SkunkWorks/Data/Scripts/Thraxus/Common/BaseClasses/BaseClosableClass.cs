﻿using System;

namespace SkunkWorks.Thraxus.Common.BaseClasses
{
	public abstract class BaseClosableClass
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