using SkunkWorks.Thraxus.Common.Enums;

namespace SkunkWorks.Thraxus.Common.BaseClasses
{
	public abstract class BaseServerSessionComp : BaseSessionComp
	{
		protected BaseServerSessionComp(string sessionName, bool noUpdate = true) : base(sessionName, SessionCompType.Server, noUpdate) { }
	}

}
