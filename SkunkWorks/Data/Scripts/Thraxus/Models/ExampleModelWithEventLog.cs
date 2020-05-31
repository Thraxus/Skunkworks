using SkunkWorks.Thraxus.Common.BaseClasses;
using SkunkWorks.Thraxus.Common.Enums;

namespace SkunkWorks.Thraxus.Models
{
	public class ExampleModelWithEventLog : BaseClosableLoggingClass
	{
		protected override string Id { get; } = "ExampleModelWithEventLog"; // If only we could use reflection!

		public override void Close()
		{
			base.Close(); // Call base.Close(); first to ensure the class has only been closed once. 
			// put all specific to this class closing logic here
		}

		public void ExampleOfClassWritingToOwnersLog()
		{
			WriteToLog("ExampleOfClassWritingToOwnersLog", "Some Message", LogType.General);
		}
	}
}