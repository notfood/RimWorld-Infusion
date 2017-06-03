using System;

using Harmony;

using Verse;
using RimWorld;

namespace Infused
{
	[HarmonyPatch (typeof (CompQuality))]
	[HarmonyPatch ("SetQuality")]
	[HarmonyPatch (new Type [] { typeof (QualityCategory), typeof (ArtGenerationContext) })]
	class CompQualityPatch
	{
		static void Postfix (CompQuality __instance, QualityCategory q, ArtGenerationContext source)
		{
			// Can we be infused?
			CompInfusion compInfusion = __instance.parent.TryGetComp<CompInfusion> ();
			if (compInfusion != null) {
				var thing = __instance.parent;
				var def = __instance.parent.def;
				// Get those Infusions rolling
				var prefix = roll (thing, q);
				var suffix = roll (thing, q);

				var tierMult = def.techLevel < TechLevel.Industrial ? 3 : 1;

				if (prefix)
					compInfusion.InitializeInfusionPrefix (GenInfusion.GetTier (q, tierMult));
				if (suffix)
					compInfusion.InitializeInfusionSuffix (GenInfusion.GetTier (q, tierMult));
				if (prefix || suffix)
					__instance.parent.HitPoints = __instance.parent.MaxHitPoints;
			}
		}

		private static bool roll (Thing thing, QualityCategory qc)
		{
			var chance = GenInfusion.GetInfusionChance (thing, qc);
			var rand = Rand.Value;
			#if DEBUG
			Log.Message ("Infused :: Rolled " + ((rand < chance) ? "success" : "failure") + " " + rand + " < " + chance + " for " + thing + " and " + qc);
			#endif
			return rand < chance;
		}
	}
}
