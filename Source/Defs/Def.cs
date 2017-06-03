using System;
using System.Collections.Generic;
using System.Linq;

using RimWorld;
using Verse;

namespace Infused
{
	public enum InfusionType
	{
		Undefined,
		Prefix,
		Suffix
	}

	public enum InfusionTier
	{
		Undefined,
		Common,
		Uncommon,
		Rare,
		Epic,
		Legendary,
		Artifact
	}

	public class InfusionAllowance
	{
		public bool melee = true;
		public bool ranged = true;
		public bool apparel = false;
	}

	public class StatMod
	{
		public float offset;
		public float multiplier = 1;

		public override string ToString ()
		{
			return string.Format ("[StatMod offset={0}, multiplier={1}]", offset, multiplier);
		}
	}

	public class Def : Verse.Def
	{
		#region XML Data
		public string labelShort = "#NN";
		public Dictionary<StatDef, StatMod> stats = new Dictionary<StatDef, StatMod>();

		public InfusionType type = InfusionType.Undefined;
		public InfusionTier tier = InfusionTier.Undefined;

		public InfusionAllowance allowance = new InfusionAllowance();
		#endregion

		/// Get matching StatMod for given StatDef. Returns false when none.
		public bool TryGetStatValue(StatDef stat, out StatMod mod)
		{
			return stats.TryGetValue (stat, out mod);
		}

		public bool MatchItemType( ThingDef def )
		{
			return def.IsMeleeWeapon && allowance.melee
				|| def.IsRangedWeapon && allowance.ranged
				|| def.IsApparel && allowance.apparel;
		}

		public override void ResolveReferences ()
		{
			base.ResolveReferences ();

			// search if we already added the StatPart
			Predicate<StatPart> predicate = (StatPart part) => part.GetType () == typeof(StatPart_InfusionModifier);

			foreach(StatDef statDef in stats.Keys) {
				if (statDef.parts == null) {
					statDef.parts = new List<StatPart> (1);
				} else if (statDef.parts.Any(predicate)) {
					continue;
				}

				statDef.parts.Add (new StatPart_InfusionModifier(statDef));
			}
		}

		public static Def Named( string defName )
		{
			return defName != null ? DefDatabase< Def >.GetNamed( defName ) : null;
		}

	}


}
