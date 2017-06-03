using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Verse;
using RimWorld;

namespace Infused
{
	class InfusedMod : Mod
	{
		public InfusedMod(ModContentPack mcp) : base (mcp)
		{
			Harmony.HarmonyInstance.Create ("rimworld.infused").PatchAll (Assembly.GetExecutingAssembly ());

			LongEventHandler.ExecuteWhenFinished (Inject);
		}

		private static void Inject ()
		{
			var defs = (
				from def in DefDatabase<ThingDef>.AllDefs
				where (def.IsMeleeWeapon || def.IsRangedWeapon || def.IsApparel)
				&& def.HasComp (typeof (CompQuality)) && !def.HasComp (typeof (CompInfusion))
				select def
			);

			var tabType = typeof (ITab_Infusion);
			var tab = InspectTabManager.GetSharedInstance (tabType);
			var compProperties = new CompProperties { compClass = typeof (CompInfusion) };

			foreach (var def in defs) {
				def.comps.Add (compProperties);
				#if DEBUG
				Log.Message ("Infused :: Component added to " + def.label);
				#endif

				if (def.inspectorTabs == null || def.inspectorTabs.Count == 0) {
					def.inspectorTabs = new List<Type> ();
					def.inspectorTabsResolved = new List<InspectTabBase> ();
				}

				def.inspectorTabs.Add (tabType);
				def.inspectorTabsResolved.Add (tab);
			}
			#if DEBUG
			Log.Message ("Infused :: Injected " + defs.Count() + " " + DefDatabase<ThingDef>.AllDefs.Count());
			#endif
		}

	}

}