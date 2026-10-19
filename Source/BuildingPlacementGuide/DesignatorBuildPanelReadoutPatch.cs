using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace BuildingPlacementGuide
{
    // The selected Architect building's readout is separate from Desc
    // (the tool's tooltip/description). Show guidance at selection time.
    [HarmonyPatch(typeof(Designator_Build), "DrawPanelReadout")]
    internal static class DesignatorBuildPanelReadoutPatch
    {
        private static void Prefix(Designator_Build __instance, ref float curY, float width)
        {
            ThingDef thingDef = __instance.PlacingDef as ThingDef;
            PlacementLabel? label = PlacementClassifier.Classify(thingDef);
            if (!label.HasValue)
                return;

            string text = "BPG_Placement".Translate() + ": " +
                          ("BPG_" + label.Value).Translate();
            GameFont font = Text.Font;
            Text.Font = GameFont.Small;
            float height = Text.CalcHeight(text, width - 6f);
            Widgets.Label(new Rect(0f, curY, width - 6f, height), text);
            Text.Font = font;
            curY += height + 4f;
        }
    }
}
