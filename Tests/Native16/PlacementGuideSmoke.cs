using System;
using RimWorld;
using UnityEngine;
using Verse;

namespace BuildingPlacementGuide.NativeSmoke
{
    // Compiled into a disposable test-only assembly; never ship with the Mod.
    [StaticConstructorOnStartup]
    public static class PlacementGuideSmoke
    {
        static PlacementGuideSmoke()
        {
            LongEventHandler.ExecuteWhenFinished(Check);
        }

        private static void AssertLabel(string defName, string expected)
        {
            ThingDef def = DefDatabase<ThingDef>.GetNamedSilentFail(defName);
            if (def == null)
                throw new InvalidOperationException("Missing original ThingDef: " + defName);
            string desc = new Designator_Build(def).Desc;
            string wanted = "Placement: " + expected;
            if (desc == null || !desc.StartsWith(wanted + "\n\n", StringComparison.Ordinal) ||
                (def.description != null && !desc.Contains(def.description)))
                throw new InvalidOperationException(
                    "Incorrect first-line placement text or missing original description for " + defName + ": expected " +
                    wanted + ", actual=" + (desc ?? "<null>"));
            Log.Message("BPG_NATIVE16_CHECK " + defName + " -> " + expected);
        }

        private static void Check()
        {
            string profile = Environment.GetEnvironmentVariable("BPG_TEST_PROFILE") ?? "vanilla";
            try
            {
                AssertLabel("TableStonecutter", "Indoor preferred");
                AssertLabel("SimpleResearchBench", "Indoor preferred");
                if (profile == "mo")
                {
                    AssertLabel("DankPyon_Smelter", "No restrictions");
                    AssertLabel("DankPyon_DryingRack_Small", "No roof allowed");
                    AssertLabel("DankPyon_CharcoalPile", "No roof allowed");
                }
                else if (profile != "vanilla")
                    throw new InvalidOperationException("Invalid smoke profile");
                Log.Message("BPG_NATIVE16_PASS profile=" + profile);
            }
            catch (Exception e)
            {
                Log.Error("BPG_NATIVE16_FAIL profile=" + profile + ": " + e);
            }
            finally
            {
                Application.Quit();
            }
        }
    }
}
