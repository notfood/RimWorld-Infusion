﻿using System;

using RimWorld;
using Verse;

namespace Infused
{
	public class QualityChances
	{
		public float awful;
		public float shoddy;
		public float poor;
		public float normal;
		public float good;
		public float superior;
		public float excellent;
		public float masterwork;
		public float legendary;
	}

	public class TechLevelRange
	{
		public TechLevel min = TechLevel.Undefined;
		public TechLevel max = TechLevel.Archotech;
	}

	public class ChanceDef : Verse.Def
	{
		public TechLevelRange techLevel = new TechLevelRange();
		public ThingFilter match = new ThingFilter ();
		public QualityChances chances = new QualityChances();

		public bool Allows(Thing thing) {
			TechLevel thingTech = thing.def.techLevel;

			return thingTech >= techLevel.min
				&& thingTech <= techLevel.max
				&& match.Allows (thing);
		}

		public float Chance(QualityCategory qc) {
			switch ( qc )
			{
			case QualityCategory.Awful:
				return chances.awful;
			case QualityCategory.Poor:
				return chances.poor;
			case QualityCategory.Normal:
				return chances.normal;
			case QualityCategory.Good:
				return chances.good;
			case QualityCategory.Excellent:
				return chances.excellent;
			case QualityCategory.Masterwork:
				return chances.masterwork;
			case QualityCategory.Legendary:
				return chances.legendary;
			default:
				return 0f;
			}
		}

		public override void ResolveReferences ()
		{
			base.ResolveReferences ();

			this.match.ResolveReferences ();
		}
	}
}

