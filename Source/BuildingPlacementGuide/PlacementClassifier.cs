using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace BuildingPlacementGuide
{
    // Mod authors may attach this extension to special buildings whose
    // placement behavior is implemented by custom code.
    public sealed class PlacementGuideExtension : DefModExtension
    {
        public PlacementLabel label = PlacementLabel.Undetermined;
    }

    public enum PlacementLabel
    {
        Undetermined,
        IndoorRequired,
        OutdoorRequired,
        IndoorPreferred,
        OutdoorPreferred,
        RoofRequired,
        RoofForbidden,
        Unrestricted
    }

    internal static class PlacementClassifier
    {
        // Cache decisions, not translated strings. Language can change at runtime.
        private static readonly Dictionary<ThingDef, PlacementLabel?> Cache =
            new Dictionary<ThingDef, PlacementLabel?>();

        internal static PlacementLabel? Classify(ThingDef def)
        {
            if (def == null || def.category != ThingCategory.Building)
                return null;

            PlacementLabel? result;
            if (Cache.TryGetValue(def, out result))
                return result;

            result = Detect(def);
            Cache.Add(def, result);
            return result;
        }

        private static PlacementLabel? Detect(ThingDef def)
        {
            PlacementGuideExtension explicitRule = def.GetModExtension<PlacementGuideExtension>();
            if (explicitRule != null)
                return explicitRule.label;

            // A roof restriction is not the same as an outdoors-only rule:
            // a walled room can also have uncovered cells.
            if (!def.canBeUsedUnderRoof || HasPlaceWorker(def, "PlaceWorker_NotUnderRoof"))
                return PlacementLabel.RoofForbidden;

            if (HasPlaceWorker(def, "PlaceWorker_OnlyUnderRoof"))
                return PlacementLabel.RoofRequired;

            // Processor Framework uses independent processing ticks. Do not
            // misclassify its processor-only worktables from Building_WorkTable.
            if (HasProcessorFrameworkComp(def))
                return PlacementLabel.Unrestricted;

            if (def.thingClass != null &&
                typeof(Building_WorkTable).IsAssignableFrom(def.thingClass) &&
                HasStatPart("WorkTableWorkSpeedFactor", "StatPart_WorkTableOutdoors"))
                return PlacementLabel.IndoorPreferred;

            // Research facilities use a different outdoors penalty stat.
            if (def.thingClass != null &&
                def.thingClass.Name.IndexOf("ResearchBench", StringComparison.OrdinalIgnoreCase) >= 0 &&
                HasStatPart("ResearchSpeedFactor", "StatPart_Outdoors"))
                return PlacementLabel.IndoorPreferred;

            // Custom production systems should not silently be marked as safe
            // for every environment when their logic cannot be inspected.
            if (def.designationCategory != null &&
                def.designationCategory.defName == "Production")
                return PlacementLabel.Undetermined;

            // Avoid filling ordinary walls/furniture with unverified claims.
            return null;
        }

        private static bool HasProcessorFrameworkComp(ThingDef def)
        {
            if (def.comps == null)
                return false;

            foreach (CompProperties comp in def.comps)
            {
                if (comp != null &&
                    comp.GetType().FullName == "ProcessorFramework.CompProperties_Processor")
                    return true;
            }
            return false;
        }

        private static bool HasPlaceWorker(ThingDef def, string typeName)
        {
            if (def.PlaceWorkers == null)
                return false;

            foreach (PlaceWorker worker in def.PlaceWorkers)
            {
                if (worker != null && worker.GetType().Name == typeName)
                    return true;
            }
            return false;
        }

        private static bool HasStatPart(string statName, string partName)
        {
            StatDef stat = DefDatabase<StatDef>.GetNamedSilentFail(statName);
            if (stat == null || stat.parts == null)
                return false;

            foreach (StatPart part in stat.parts)
            {
                if (part != null && part.GetType().Name == partName)
                    return true;
            }
            return false;
        }
    }
}
