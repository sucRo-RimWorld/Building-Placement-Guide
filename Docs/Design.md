# Building Placement Guide — design (initial implementation)

## Goal

Show the building's placement environment **as soon as its Architect menu button is clicked**, as the **first line of the selection information panel** (`Designator_Build.Desc`), not in the separate detailed Info Card. Long vanilla or Mod descriptions must not hide the placement line below the fold. Never modify placement rules, work speeds, or production.

Examples:

- Placement: Indoor preferred / 設置環境：屋内推奨
- Placement: No roof allowed / 設置環境：屋根不可
- Placement: No restrictions / 設置環境：制約なし

Preserve the original building description below this short line, separated by one blank line. Do not show speed percentages, process implementation details, or framework names.

## Classification vocabulary

Indoor required / 屋内必須, Outdoor required / 屋外必須, Indoor preferred / 屋内推奨, Outdoor preferred / 屋外推奨, Roof required / 屋根必須, No roof allowed / 屋根不可, No restrictions / 制約なし, Cannot determine / 判定不能.

In particular, **制約なし** is the only Japanese label for *No restrictions*; never use 指定なし.

**No roof** is not identical to **Outdoors**. They must not be conflated.

## Initial automatic detection (1.6)

1. Explicit `PlacementGuideExtension.label` overrides automatic inference.
2. `canBeUsedUnderRoof=false` or `PlaceWorker_NotUnderRoof` => Roof forbidden.
3. `PlaceWorker_OnlyUnderRoof` => Roof required (when such a place worker exists).
4. Processor Framework `CompProperties_Processor` => Unrestricted *as far as the standard roof/bench checks can establish*. Suppress irrelevant Building_WorkTable outdoors penalties on autonomous processing.
5. Standard worktables with active `StatPart_WorkTableOutdoors` => Indoor preferred.
6. Research benches with active `StatPart_Outdoors` => Indoor preferred.
7. Other production buildings => Cannot determine. Unrelated generic furniture/walls receive no extra line.

Never infer an outdoor/indoor **requirement** from an efficiency preference. The automatic classifier does not yet detect every hardcoded runtime restriction or hybrid Processor Framework + bill-work equipment.

## Extension for nonstandard Mods

Other Mods can explicitly annotate a `ThingDef`:

```xml
<modExtensions>
  <li Class="BuildingPlacementGuide.PlacementGuideExtension">
    <label>OutdoorRequired</label>
  </li>
</modExtensions>
```

The extension requires Building Placement Guide to be installed when used directly; external compatibility patches should be conditional on its packageId.

## Loaded-game acceptance (2026-10-19)

Verified with original licensed RimWorld Linux 1.6, a disposable copy, and a separate test-only assembly invoking the *actual patched* `Designator_Build.Desc` method. [Successful native run #39226208941](https://github.com/sucRo-RimWorld/Ancient-Medieval-Japan-Project/actions/runs/39226208941), BPG source commit `3897296446cc87f455ef1ac1dad700db535b299a`, production DLL SHA-256 `2a9e24051fbe2454597b746b47a5007eef0d58dfd10172f85d6430ac0d33ecd9`.

| Loaded provider | Actual ThingDef | Expected tooltip value | Result |
| --- | --- | --- | --- |
| Vanilla | TableStonecutter | Indoor preferred | PASS |
| Vanilla | SimpleResearchBench | Indoor preferred | PASS |
| MO | TableStonecutter | Indoor preferred | PASS |
| MO | SimpleResearchBench | Indoor preferred | PASS |
| MO | DankPyon_Smelter | No restrictions | PASS |
| MO | DankPyon_DryingRack_Small | No roof allowed | PASS |
| MO | DankPyon_CharcoalPile | No roof allowed | PASS |

Both loaded games: process exit 0, Unity errors 0, all assertions succeeded. The test did not manipulate a player's save or permanently stage any Mod.

## Coverage not yet demonstrated

- Actual human visual review of text layout in Architect menu.
- Explicit extension-based overrides in a separate third-party Mod.
- Other custom processing systems or hybrid Processor Framework workbench/bill behavior.
- Non-Vanilla room/hardcoded prerequisites beyond the heuristics listed above.
- RimWorld 1.7 and localization appearance under Japanese UI.
- Construction/building placement does not require a regression test for this description-only patch, but remains unverified in this acceptance.

**Status:** compiled production DLL included, Vanilla/MO loaded-description verification PASS. Do not present this as exhaustive support for all Mod-specific placement rules.

## First-line display change (2026-10-19)

Architect selection previously appended its placement label to the end of the description; with long Mod descriptions it might not be immediately visible. The patch now **prepends** the line to `Designator_Build.Desc`, before existing text. The native test asserts it starts with the expected placement label and retains the original description, rather than merely searching for the label anywhere in the text. Visual screenshots of the GUI itself remain a separate acceptance item.

## First-line loaded-game verification (2026-10-19)

The rebuilt DLL SHA-256 `dd38531186382aff74ccf418fef512b0cef1b136000d7d579012240568935d9a` compiled against the owned original game (warnings 0, errors 0; [build #39227436183](https://github.com/sucRo-RimWorld/Ancient-Medieval-Japan-Project/actions/runs/39227436183)). The strengthened test requires `Designator_Build.Desc` to **start** with `Placement: <label>` followed by a blank line and also include the original building description. [Vanilla/MO loaded game test #39227719975](https://github.com/sucRo-RimWorld/Ancient-Medieval-Japan-Project/actions/runs/39227719975): 2/2 Vanilla + 5/5 MO checks PASS, both processes exit 0, zero Unity errors. This validates the loaded method's output, not screenshots of the on-screen Architect panel.
