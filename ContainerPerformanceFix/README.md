# Container Performance Fix

コンテナのソート時に行われるスタック候補の探索を最適化します。

`UIInventory.Sort()` のスタック統合ループだけを Harmony Transpiler で置き換え、
その後の配置リセット、ソート、Redraw はゲーム本体の処理をそのまま使用します。
