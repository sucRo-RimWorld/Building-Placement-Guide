using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using RimWorks.Pickle;
using RimWorld;
using Verse;

namespace BuildingPlacementGuide.Screen16
{
    [PickleSteps]
    public sealed class ArchitectScreenSteps
    {
        private static Task GameThread(Action action)
        {
            var finished = new TaskCompletionSource<bool>();
            Action run = delegate
            {
                try
                {
                    if (!UnityData.IsInMainThread)
                        throw new InvalidOperationException("Screenshot UI was not on game thread");
                    action();
                    finished.SetResult(true);
                }
                catch (Exception e) { finished.SetException(e); }
            };
            var driver = typeof(PickleContext).Assembly.GetType("RimWorks.Pickle.Runtime.PickleDriver", true);
            var post = driver.GetMethod("Post", new[] { typeof(Action) });
            if (post == null) throw new MissingMethodException(driver.FullName, "Post(Action)");
            post.Invoke(null, new object[] { run });
            return finished.Task;
        }

        private static void Capture(string filename)
        {
            var module = Type.GetType("UnityEngine.ScreenCapture, UnityEngine.ScreenCaptureModule", false);
            if (module == null)
                throw new InvalidOperationException("Unity screenshot module missing");
            var capture = module.GetMethod("CaptureScreenshot", new[] { typeof(string) });
            if (capture == null)
                throw new MissingMethodException(module.FullName, "CaptureScreenshot(string)");
            capture.Invoke(null, new object[] { filename });
        }

        [Then("capture selected Architect equipment and its placement condition")]
        public async Task Run(PickleContext ctx)
        {
            await ctx.WaitUntil(() =>
            {
                Type t = Type.GetType(
                    "BuildingPlacementGuide.Screen16.ArchitectScreenQuickstart, BPG.Screen16.Quickstart");
                PropertyInfo prop = t == null ? null : t.GetProperty(
                    "IsPostLoaded", BindingFlags.Public | BindingFlags.Static);
                return prop != null && (bool)prop.GetValue(null, null);
            }, 90f);
            await ctx.WaitFrames(12);
            string folder = Environment.GetEnvironmentVariable("BPG_SCREENSHOT_DIR");
            ctx.Require(!string.IsNullOrEmpty(folder) && Directory.Exists(folder),
                "Private screenshot output directory absent");
            foreach (string defName in new[] { "TableStonecutter", "DankPyon_Smelter" })
            {
                string name = defName + ".png";
                string path = Path.Combine(folder, name);
                await GameThread(delegate
                {
                    ctx.Require(Find.CurrentMap != null, "Quickstart map was not loaded");
                    ThingDef def = DefDatabase<ThingDef>.GetNamedSilentFail(defName);
                    ctx.Require(def != null, "Requested equipment absent: " + defName);
                    Find.MainTabsRoot.SetCurrentTab(MainButtonDefOf.Architect);
                    var architect = (MainTabWindow_Architect)MainButtonDefOf.Architect.TabWindow;
                    var flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
                    var tabsField = typeof(MainTabWindow_Architect).GetField("desPanelsCached", flags);
                    var activeField = typeof(MainTabWindow_Architect).GetField("selectedDesPanel", flags);
                    ctx.Require(tabsField != null && activeField != null,
                        "Architect category panel fields unavailable");
                    var tabs = tabsField.GetValue(architect) as IEnumerable;
                    ctx.Require(tabs != null, "Architect category panels unavailable");
                    ArchitectCategoryTab production = null;
                    foreach (object item in tabs)
                    {
                        var candidate = item as ArchitectCategoryTab;
                        if (candidate != null && candidate.def != null &&
                            candidate.def.defName == "Production")
                        {
                            production = candidate;
                            break;
                        }
                    }
                    ctx.Require(production != null, "Production Architect category absent");
                    activeField.SetValue(architect, production);
                    var designator = new Designator_Build(def);
                    Find.DesignatorManager.Select(designator);
                    ctx.Require(Find.DesignatorManager.SelectedDesignator != null,
                        "No active Architect build selection");
                    ctx.Require(designator.Desc.StartsWith("設置環境: ") ||
                                designator.Desc.StartsWith("Placement: "),
                        "Placement information not first in selected designator");
                    Log.Message("BPG_SCREEN16_SELECTED " + defName + " first=" +
                                designator.Desc.Split('\n')[0]);
                });
                await ctx.WaitFrames(18);
                await GameThread(delegate { Capture(path); });
                await ctx.WaitUntil(() => File.Exists(path) && new FileInfo(path).Length > 5000, 30f);
                Log.Message("BPG_SCREEN16_CAPTURED " + name);
            }
            Log.Message("BPG_SCREEN16_PASS");
        }
    }
}
