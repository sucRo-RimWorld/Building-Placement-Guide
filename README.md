# Building Placement Guide

建築前に、設備の屋内・屋外の設置条件や推奨環境を短く表示する RimWorld QoL Mod。

Display indoor/outdoor placement guidance in building tooltips before construction.

## 表示 / Labels

屋内必須 / 屋外必須 / 屋内推奨 / 屋外推奨 / 屋根必須 / 屋根不可 / 制約なし / 判定不能

作業速度の数値や Processor Framework などの内部情報は表示しない。設置適性だけを示す。

## 現在の開発状態 / Status

初期ソース実装（未ビルド・未実ゲーム検証）。**現時点で導入可能な完成版ではありません。**

- RimWorld 1.6 向けの建築メニューツールチップ拡張。
- Vanilla の通常作業台と研究設備、屋根不可設備を自動判定。
- Processor Framework による自動処理設備は、通常の作業台ペナルティを適用したと誤表示しない。
- 判定できない独自設備を安易に「制約なし」にしない。
- 明示的な Mod 拡張により判定結果を上書き可能。

判定対象・既知の制限・検証項目は [Docs/Design.md](Docs/Design.md) を参照。

## Build

RimWorld の所有する DLL はリポジトリに同梱しません。SDK と所有するゲームの Managed アセンブリ、Harmony が必要です。

```powershell
dotnet build Source/BuildingPlacementGuide/BuildingPlacementGuide.csproj -c Release -p:RimWorldManagedDir="C:/Games/RimWorld/RimWorldWin64_Data/Managed" -p:HarmonyDllPath="C:/Games/RimWorld/Mods/Harmony/Current/Assemblies/0Harmony.dll"
```

ビルド成功時は `1.6/Assemblies/BuildingPlacementGuide.dll` にコピーされます。ビルドの成功と実ゲームでの挙動は未確認です。
