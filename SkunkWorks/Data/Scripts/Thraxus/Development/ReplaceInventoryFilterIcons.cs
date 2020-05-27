using System;
using System.Linq;
using Sandbox.Definitions;
using VRage.Utils;

namespace SkunkWorks.Thraxus.Development
{
	public class ReplaceInventoryFilterIcons
	{
		public void Run()
		{
			try
			{
				foreach (MyCubeBlockDefinition reactorDef in MyDefinitionManager.Static.GetAllDefinitions().OfType<MyCubeBlockDefinition>().Where(myCubeBlockDefinition => myCubeBlockDefinition is MyReactorDefinition))
				{
					if (reactorDef.Id.SubtypeId != MyStringHash.GetOrCompute("ThraxusCustomReactor")) continue;
					((MyReactorDefinition)reactorDef).InventoryConstraint.m_useDefaultIcon = false;
					((MyReactorDefinition)reactorDef).InventoryConstraint.Icon = $@"Textures\GUI\Icons\filter_uranium.dds";
					((MyReactorDefinition)reactorDef).InventoryConstraint.UpdateIcon();
				}
			}
			catch (Exception e)
			{
				MyLog.Default.WriteLine($"SkunkWorksCore: ReplaceReactorMenu - Boom!!! {e}");
			}

		}
	}
}