using HarmonyLib;
using RimWorld;
using Verse;

namespace BuildingPlacementGuide
{
    [StaticConstructorOnStartup]
    internal static class BuildingPlacementGuideBootstrap
    {
        static BuildingPlacementGuideBootstrap()
        {
            new Harmony("sucro.buildingplacementguide").PatchAll();
        }
    }

    [HarmonyPatch(typeof(Designator_Build), "Desc", MethodType.Getter)]
    internal static class DesignatorBuildDescriptionPatch
    {
        private static void Postfix(Designator_Build __instance, ref string __result)
        {
            ThingDef thingDef = __instance.PlacingDef as ThingDef;
            PlacementLabel? label = PlacementClassifier.Classify(thingDef);
            if (!label.HasValue)
                return;

            // The selected command's panel already renders the guidance at its top.
            // Do not repeat it in the descriptive text immediately below the costs.
            if (Find.DesignatorManager != null &&
                object.ReferenceEquals(Find.DesignatorManager.SelectedDesignator, __instance))
                return;

            string key = "BPG_" + label.Value;
            string line = "BPG_Placement".Translate() + ": " + key.Translate();
            // The Architect selection card can show only the beginning of a long description.
            // Keep the placement condition visible without opening the detailed Info Card.
            __result = string.IsNullOrEmpty(__result) ? line : line + "\n\n" + __result;
        }
    }
}
