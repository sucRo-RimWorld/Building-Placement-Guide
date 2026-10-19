using RimWorks.Quickstarts;
using RimWorld;
using Verse;

namespace BuildingPlacementGuide.VisualCheck
{
    public sealed class BPGArchitectQuickstart : AbstractQuickstart
    {
        public override TaggedString description { get { return "Building Placement Guide Architect selection screenshot"; } }
        public override int mapSize { get { return 50; } }
        public override string seed { get { return "BPG-VISUAL-1-6"; } }
        public override void PostLoaded()
        {
            ThingDef def = DefDatabase<ThingDef>.GetNamedSilentFail("TableStonecutter");
            if (def == null) { Log.Error("BPG_VISUAL_MISSING_STONECUTTER"); return; }
            Find.MainTabsRoot.SetCurrentTab(MainButtonDefOf.Architect);
            Find.DesignatorManager.Select(new Designator_Build(def));
            string placement = new Designator_Build(def).Desc;
            if (System.Environment.GetEnvironmentVariable("BPG_VISUAL_LANG") == "Japanese")
            {
                if (!placement.StartsWith("設置環境: 屋内推奨", System.StringComparison.Ordinal))
                {
                    Log.Error("BPG_VISUAL_JA_TRANSLATION_FAILED: " + placement);
                    return;
                }
                Log.Message("BPG_VISUAL_JA_TRANSLATION_PASS firstline=" + placement.Split('\n')[0]);
            }
            Log.Message("BPG_VISUAL_ARCHITECT_SELECTED");
        }
    }
}
