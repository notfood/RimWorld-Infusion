using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;

namespace Infused
{
    public class MapComponent_InfusionManager : MapComponent
    {
		public MapComponent_InfusionManager (Map map) : base(map)
        {
            // Clear old labels
            InfusionLabelManager.ReInit();
        }

        //Draw infusion label on map
        public override void MapComponentOnGUI()
        {
            if (Find.CameraDriver.CurrentZoom != CameraZoomRange.Closest) return;
            if (InfusionLabelManager.Drawee.Count == 0)
                return;

            foreach (var current in InfusionLabelManager.Drawee) {
                if (current.parent.Map != map) {
                    continue;
                }

                // skip fogged
                if (map.fogGrid.IsFogged (current.parent.Position)) {
                    continue;
                }

                GenMapUI.DrawThingLabel (GenMapUI.LabelDrawPosFor (current.parent, -0.66f), current.InfusedLabel, current.InfusedLabelColor);
            }
        }
    }
}
