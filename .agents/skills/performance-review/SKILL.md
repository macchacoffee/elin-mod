---
name: performance-review
description: Elin Mod の処理について、hot path、呼び出し頻度、allocation、Reflection、LINQ、Regex、文字列処理、キャッシュの費用対効果を確認するときに使う。「重い？」「最適化した方がいい？」「旧版と新版を比較して」といった依頼向け。
---

# Performance Review

## 原則

見た目だけで「重い」と決めつけず、頻度と測定値を優先する。
小さな CPU コストを消すために state invalidation や lifetime 管理を過度に複雑化しない。

## 手順

1. 対象メソッドの呼び出し頻度を確認する。
2. per-frame / per-hover / per-combat-action / rare-path のどれか分類する。
3. loop 内や高頻度経路の allocation を探す。
4. Reflection、LINQ、Regex、string interpolation / concatenation、delegate 作成、collection 再構築を確認する。
5. 既に cache されている情報を再計算していないか確認する。
6. 新しい cache を提案する場合、invalidation 条件と memory/lifetime コストを先に説明する。
7. `docs/performance-notes.md` に既存測定があるか確認する。
8. Stopwatch 等の測定値がある場合は Count / Total / Average / Max と測定条件を比較する。
9. Max の単発 spike だけで回帰判定しない。
10. 最適化によって correctness や UI 更新タイミングが変わらないか確認する。

## 優先順位

1. 明確な algorithmic 問題
2. hot path の大きな allocation / 重複処理
3. 高頻度 Reflection / Regex / collection build
4. 小さな micro-optimization

## 出力

1. **結論**: 今すぐ対応 / 測定してから / 現状維持
2. **実際にコストになりそうな箇所**
3. **コストが小さそうな箇所**
4. **推奨する測定方法**
5. **最適化する場合の最小変更案**
6. **複雑性とのトレードオフ**
