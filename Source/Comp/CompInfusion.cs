using System;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;
using Verse.Sound;

namespace Infused
{
	public class CompInfusion : ThingComp
	{
		private static readonly SoundDef InfusionSound = SoundDef.Named( "Infusion_Infused" );

		// Is our infusion newly created? Only for notifications.
		private bool isNew;

		private string prefixDefName, suffixDefName;
		private Def prefix, suffix;

		private string infusedLabel;
		private Color infusedLabelColor;

		public InfusionSet Infusions {
			get { return new InfusionSet (prefix, suffix); }
		}

		public bool Infused {
			get { return prefix != null || suffix != null; }
		}

		public string InfusedLabel {
			get { return infusedLabel; }
		}

		public Color InfusedLabelColor {
			get { return infusedLabelColor; }
		}

		public void InitializeInfusionPrefix(InfusionTier tier)
		{
			InitializeInfusion (InfusionType.Prefix, tier, out prefix);
		}
		public void InitializeInfusionSuffix(InfusionTier tier)
		{
			InitializeInfusion (InfusionType.Suffix, tier, out suffix);
		}

		public void InitializeInfusion(InfusionType type, InfusionTier tier, out Def infusion)  {
			if ( !
				(from t in DefDatabase< Def >.AllDefs
					where
					t.tier == tier &&
					t.type == type &&
					t.MatchItemType( parent.def )
					select t
				).TryRandomElement( out infusion ) )
			{
				//No infusion available from defs
				Log.Warning( "Infused :: Couldn't find any " + type + "InfusionDef! Tier: " + tier );
			}

			isNew = true;
		}

		private void throwMote()
		{
			CompQuality compQuality = parent.TryGetComp<CompQuality> ();
			if (compQuality == null) {
				return;
			}
			string qualityLabel = compQuality.Quality.GetLabel();

			var msg = new StringBuilder();
			msg.Append( qualityLabel + " " );
			if ( parent.Stuff != null )
			{
				msg.Append( parent.Stuff.LabelAsStuff + " " );
			}
			msg.Append( parent.def.label );
            Messages.Message( ResourceBank.StringInfusionMessage.Translate( msg ), new RimWorld.Planet.GlobalTargetInfo(parent), MessageTypeDefOf.SilentInput );
			InfusionSound.PlayOneShotOnCamera();

			MoteMaker.ThrowText( parent.Position.ToVector3Shifted(), this.parent.Map, ResourceBank.StringInfused,
			                       GenInfusionColor.Legendary );

			isNew = false;
		}

		public override void PostSpawnSetup (bool respawningAfterLoad)
		{
			base.PostSpawnSetup(respawningAfterLoad);

			if (Infused) {
				//When there is only suffix
				if (suffix != null)
				{
					infusedLabelColor = suffix.tier.InfusionColor();
				}
				//When there is only prefix
				else if (prefix != null)
				{
					infusedLabelColor = prefix.tier.InfusionColor();
				}
				//When there are both prefix and suffix
				else
				{
					infusedLabelColor = MathUtility.Max(prefix.tier, suffix.tier).InfusionColor();
				}

				infusedLabel = "";
				if (prefix != null)
				{
					infusedLabel += prefix.labelShort;
					if (suffix != null)
						infusedLabel += " ";
				}
				if (suffix != null)
					infusedLabel += suffix.labelShort;

				InfusionLabelManager.Register (this);

				// We only throw notifications for newly spawned items.
				if (isNew)
					throwMote ();
			}
		}

		public override void PostExposeData()
		{
			base.PostExposeData();

			// This is ugly, I need a better solution.
			if (prefix != null)
				prefixDefName = prefix.defName;
			if (suffix != null)
				suffixDefName = suffix.defName;

			Scribe_Values.Look (ref prefixDefName, "prefix", null);
			Scribe_Values.Look (ref suffixDefName, "suffix", null);

			if (prefix == null)
				prefix = Def.Named(prefixDefName);
			if (suffix == null)
				suffix = Def.Named(suffixDefName);

#if DEBUG
			if ( (prefixDefName != null && prefix == null) || (suffixDefName != null && suffix == null) )
			{

				Log.Warning( "Infused :: Could not find some of InfusionDef." + prefix + "/" + suffix );
			}
#endif
		}

		public override void PostDeSpawn (Map map)
		{
			base.PostDeSpawn (map);

			if ( Infused )
			{
				InfusionLabelManager.DeRegister( this );
			}
		}

		public override bool AllowStackWith( Thing other )
		{
			return false;
		}

		public override string GetDescriptionPart()
		{
			return base.GetDescriptionPart() + "\n" + parent.GetInfusionDesc();
		}

		public override string TransformLabel (string label)
		{
			// When this function is called, our infusion is no longer new.
			isNew = false;

			if (Infused) {
				return parent.GetInfusedLabel ();
			} else {
				return base.TransformLabel (label);
			}
		}
	}
}
