---
name: elin-bug-investigation
description: Elin Mod の不具合や不可解な挙動について、関連 call site、state lifecycle、cache、vanilla の特殊処理まで追って原因を特定するときに使う。「原因を調べて」「なぜ反映されない？」「この状態はいつ変わる？」といった依頼向け。明示されない限り修正は行わない。
---

# Elin Bug Investigation

## 原則

修正案を先に決めず、まず原因と状態遷移を特定する。
Elin では特殊処理がハードコードされている場合があるため、一般的な仕組みだけを見て結論を出さない。

## 手順

1. ユーザーが観測した現象と期待動作を整理する。
2. 現象に直接関係する entry point を特定する。
3. 対象 state / field / property の定義を確認する。
4. write site を検索し、どこで値が生成・更新されるか追う。
5. read site を検索し、どこで古い値が利用されうるか追う。
6. cache がある場合は、build / reuse / invalidate / rebuild の条件を確認する。
7. lifecycle に依存する場合は constructor、Awake/Start 相当、Goal 作成、Map 移動、Save/Load など必要な境界を追う。
8. ID や Feat などを直接比較する hard-coded special case がないか検索する。
9. Mod Patch が vanilla invariant を崩していないか確認する。
10. 最小の原因仮説を作り、コード上の根拠と照合する。
11. 原因が確定した後にだけ修正候補を比較する。

## 修正候補を比較する観点

- correctness
- vanilla behavior への影響範囲
- cache / lifecycle との整合性
- Harmony Patch の壊れやすさ
- performance
- 他 Mod から観測される state への影響
- Save/Load 後の挙動

## 出力

1. **原因**: 確定 / 有力 / 未確定を明示
2. **状態遷移**: どこで何が古くなるか
3. **根拠となるコード**
4. **修正候補と推奨案**
5. **副作用として確認すべきケース**
6. **不足している情報**

明示的な実装依頼がない限りファイルは変更しない。
