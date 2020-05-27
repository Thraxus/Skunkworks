using SkunkWorks.Thraxus.Common.Enums;

namespace SkunkWorks.Thraxus.Common.BaseClasses
{
	public abstract class BaseClientSessionComp : BaseSessionComp
	{
		protected BaseClientSessionComp(string sessionName, bool noUpdate = true) : base(sessionName, SessionCompType.Client, noUpdate) { }
	}
}
