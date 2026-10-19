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

            string key = "BPG_" + label.Value;
            string line = "BPG_Placement".Translate() + ": " + key.Translate();
            __result = string.IsNullOrEmpty(__result) ? line : __result.TrimEnd() + "\n\n" + line;
        }
    }
}
