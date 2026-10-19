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
            Log.Message("BPG_VISUAL_ARCHITECT_SELECTED");
        }
    }
}
