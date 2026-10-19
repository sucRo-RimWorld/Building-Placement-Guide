using RimWorks.Quickstarts;
using Verse;

namespace BuildingPlacementGuide.Screen16
{
    public sealed class ArchitectScreenQuickstart : AbstractQuickstart
    {
        public static bool IsPostLoaded { get; private set; }
        public override TaggedString description { get { return "Capture real Architect selection readout with BPG"; } }
        public override int mapSize { get { return 50; } }
        public override string seed { get { return "BPG-ARCHITECT-VISUAL-16"; } }
        public override void PostLoaded()
        {
            IsPostLoaded = true;
            Log.Message("BPG_SCREEN16_QUICKSTART_READY");
        }
    }
}
