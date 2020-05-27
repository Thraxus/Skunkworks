using SkunkWorks.Thraxus.Common.BaseClasses;
using SkunkWorks.Thraxus.Common.Enums;

namespace SkunkWorks.Thraxus.Models
{
	public class ExampleModelWithEventLog : LogBaseEvent
	{
		private const string ModelName = "ExampleModelWithEventLog"; // If only we could use reflection!
		public ExampleModelWithEventLog() : base(ModelName) { }

		public void ExampleOfClassWritingToOwnersLog()
		{
			WriteToLog("ExampleOfClassWritingToOwnersLog", "Some Message", LogType.General);
		}
	}
}