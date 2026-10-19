# Building Placement Guide — design (initial implementation)

## Goal

Show the building's placement environment *before* the player constructs it, using only a single short line in the Architect menu tooltip. Never modify placement rules, work speeds, or production.

Examples:

- Placement: Indoor preferred / 設置環境：屋内推奨
- Placement: No roof allowed / 設置環境：屋根不可
- Placement: No restrictions / 設置環境：制約なし

Do not show speed percentages, process implementation details, or framework names.

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

## Acceptance checks (not yet executed)

- Vanilla smithy/stonecutter: indoor preferred; building unchanged.
- MO DankPyon_Smelter: unrestricted (Processor Framework, not the apparent workbench outdoors penalty).
- MO drying rack/charcoal pile: roof forbidden.
- Vanilla workbench and research bench outdoors stat detection behaves as intended.
- In a Mod using a custom/unrecognized production mechanism, show Cannot determine rather than fabricate No restrictions.
- Modded special rules via extension override the inferred category.
- Japanese and English tooltip strings; no duplicated lines in combination with Relevant Stats In Description.
- Save/load is unaffected; no simulation ticks or map scans added.

**Status:** source implementation only; compile and actual in-game acceptance have not been performed. Do not treat these expected results as PASS.
