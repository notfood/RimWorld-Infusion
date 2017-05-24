using System.Text;
using RimWorld;
using UnityEngine;
using Verse;

namespace Infused
{
    internal class ITab_Infusion : ITab
    {
        private static readonly Vector2 WinSize = new Vector2( 400, 550 );

        private static CompInfusion SelectedCompInfusion
        {
            get
            {
                var thing = Find.Selector.SingleSelectedThing;
				if (thing != null) {
					return thing.TryGetComp< CompInfusion > ();
				} else {
					return null;
				}
            }
        }

		public override bool IsVisible {
			get { return SelectedCompInfusion != null && SelectedCompInfusion.Infused; }
		}

        public ITab_Infusion()
        {
            size = WinSize;
            labelKey = "TabInfusion";
        }

        protected override void FillTab()
        {
            //Label
            var rectBase = new Rect( 0f, 0f, WinSize.x, WinSize.y ).ContractedBy( 10f );
            var rectLabel = rectBase;
            Text.Font = GameFont.Medium;
            var label = GetRectLabel();
            Widgets.Label( rectLabel, label );

            //Quality
            var rectQuality = rectBase;
            rectQuality.yMin += Text.CalcHeight (label, rectBase.width);
            Text.Font = GameFont.Small;
            QualityCategory qc;
            SelectedCompInfusion.parent.TryGetQuality( out qc );

			var subLabelBuilder = new StringBuilder();
	        subLabelBuilder.Append( qc.GetLabel().CapitalizeFirst() )
	                    .Append( " " )
	                    .Append( ResourceBank.StringQuality )
	                    .Append( " " );
            if ( SelectedCompInfusion.parent.Stuff != null )
            {
	            subLabelBuilder.Append( SelectedCompInfusion.parent.Stuff.LabelAsStuff ).Append( " " );
            }
	        subLabelBuilder.Append( SelectedCompInfusion.parent.def.label );
			var subLabel = subLabelBuilder.ToString();

            Widgets.Label( rectQuality, subLabel);
            GUI.color = Color.white;
			
			//Infusion descriptions
			Text.Anchor = TextAnchor.UpperLeft;
			var rectDesc = rectBase;
			rectDesc.yMin += rectQuality.yMin + Text.CalcHeight(subLabel, rectBase.width);
            Text.Font = GameFont.Small;
            Widgets.Label( rectDesc, SelectedCompInfusion.parent.GetInfusionDesc() );
        }

        private static string GetRectLabel()
        {
			var infusions = SelectedCompInfusion.Infusions;
			var prefix = infusions.prefix;
			var suffix = infusions.suffix;

            Color color;
			if ( prefix != null && suffix == null )
            {
                color = prefix.tier.InfusionColor();
            }
			else if ( suffix != null && prefix == null )
            {
                color = suffix.tier.InfusionColor();
            }
            else
            {
                color = MathUtility.Max( prefix.tier, suffix.tier ).InfusionColor();
            }

            GUI.color = color;
			return SelectedCompInfusion.parent.GetInfusedLabel ().CapitalizeFirst();
        }
    }
}
