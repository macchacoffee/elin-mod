---
name: harmony-transpiler-review
description: Elin の Harmony Transpiler を、元のゲーム処理・IL・stack balance・アップデート耐性まで含めてレビューするときに使う。Transpiler の正しさ、壊れやすさ、Prefix/Postfix 代替可能性を調べる依頼向け。明示されない限りコードは変更しない。
---

# Harmony Transpiler Review

## 目的

Transpiler を「コードとしてそれっぽいか」ではなく、実際の vanilla IL と制御フローに対して正しく作用するか確認する。

## 手順

1. Patch 対象の type / method / overload を特定する。
2. 利用可能なゲーム本体ソースまたは decompile 結果から、対象メソッドの元実装を確認する。
3. 元の高水準制御フローを短く説明する。
4. Transpiler の matcher が、元 IL のどの instruction sequence を狙っているか対応付ける。
5. 追加・削除・置換される instruction ごとに evaluation stack の入出力を確認する。
6. branch target、label、exception block を移動・破壊していないか確認する。
7. local / argument index をハードコードしている場合、その前提が必要か確認する。
8. matcher が広すぎて別の類似パターンへ一致しないか確認する。
9. matcher が狭すぎる場合も、単に緩くするのではなく何を invariant として使うべきか考える。
10. Elin 更新で壊れそうな前提を列挙する。
11. Prefix / Postfix で同じ変更が可能か確認する。ただし vanilla 処理の大量コピーが必要なら Transpiler 維持を正当な選択肢とする。
12. このリポジトリの Harmony version で提案 API が利用可能か確認する。

## 出力

以下の順で報告する。

1. **結論**: 問題なし / 要注意 / 修正推奨
2. **Transpiler が実際に変更している処理**
3. **問題点**: correctness を最優先し、重要度順
4. **アップデートで壊れやすい前提**
5. **代替案**: 本当に有利な場合だけ Prefix / Postfix 等を提案
6. **確認できなかった点**

コード変更が明示されていない場合は、レビューだけで終了する。
