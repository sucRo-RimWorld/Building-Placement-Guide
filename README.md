# Indoor or Outdoor? – 屋内・屋外設置ガイド

建築メニューで設備をクリックした際の簡易説明欄の先頭に、屋内・屋外の設置条件や推奨環境を表示する RimWorld QoL Mod。詳細情報（i）を開く必要はありません。

Display indoor/outdoor placement guidance **at the top of the Architect selection description**, as soon as a building is selected. No detailed info panel required.

## 表示 / Labels

屋内必須 / 屋外必須 / 屋内推奨 / 屋外推奨 / 屋根必須 / 屋根不可 / 制約なし / 判定不能

作業速度の数値や Processor Framework などの内部情報は表示しない。設置適性だけを示す。

## 現在の開発状態 / Status

**RimWorld 1.6:** DLL ビルド済み。建築メニューの簡易説明欄の**先頭行**へ設置条件を表示。Vanilla・MOの実ゲーム読込テストで先頭表示と元の説明文の維持を 7/7 PASS、Unity ERROR 0、両構成 exit 0 で確認（[Actions #39227719975](https://github.com/sucRo-RimWorld/Ancient-Medieval-Japan-Project/actions/runs/39227719975)）。

`1.6/Assemblies/BuildingPlacementGuide.dll` を同梱。Harmony が必要。ゲーム画面上のツールチップ表示を人間が確認する最終目視、RimWorld 1.7、および未知の他Mod全般の網羅検証は未実施。

- RimWorld 1.6 向けの、建築メニュー選択時に出る簡易説明欄の拡張。
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

ビルド成功時は `1.6/Assemblies/BuildingPlacementGuide.dll` にコピーされます。リポジトリには実際の 1.6 ビルド成果物を収録済み（[ビルドログ #39227436183](https://github.com/sucRo-RimWorld/Ancient-Medieval-Japan-Project/actions/runs/39227436183)、警告0・エラー0）。
