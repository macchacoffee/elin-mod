# Mod 開発スクリプト

すべてのコマンドはリポジトリルートから実行する。

## ビルド

```powershell
.\tools\BuildMod.ps1 -Mod SomewhatEnhancedDisplay
.\tools\BuildMod.ps1 -Project .\NoPCC\NoPCC.csproj
```

通常実行は csproj が解決した `$(ElinGamePath)\Package\Mod_<AssemblyName>` へ出力し、DLL と `package` 配下の資産が配置されたことを確認する。

ゲームの Mod 配置先へ書き込まずに確認するときは、`artifacts\<Mod名>` を使う。

```powershell
.\tools\BuildMod.ps1 -Mod SomewhatEnhancedDisplay -ValidationOutput
```

## SourceSheet の検証

```powershell
.\tools\TestSourceSheet.ps1 -Mod SomewhatEnhancedDisplay
.\tools\TestSourceSheet.ps1 -Mod FoodEffectMultiplier
```

`SourceSheet.xlsx` がない Mod は `Skipped` と表示して正常終了する。翻訳を追加した変更では、期待する Source ID を指定する。

```powershell
.\tools\TestSourceSheet.ps1 -Mod SomewhatEnhancedDisplay -RequiredSourceId @(
  'mc_sed_reverseManaBodyHealthBar',
  'mc_sed_tooltipReverseManaBodyHealthBar'
)
```
