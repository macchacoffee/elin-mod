# Somewhat Enhanced Display 固有方針

このディレクトリでは、リポジトリルートの `AGENTS.md` に加えて以下を適用する。

## 目的

- この Mod は主に HoverGuide と HealthBar の表示を拡張する。
- 情報量を増やすだけでなく、ゲームプレイ中の視認性、レイアウト安定性、更新タイミングを重視する。
- vanilla UI の挙動は、機能上必要な部分だけを変更する。

## HealthBar

- HealthBar 更新処理は毎フレーム実行されるため、追加処理の頻度と allocation を意識する。
- `HP <= 0` だけを死亡判定として扱わない。Elin にはマナの体など、HP と死亡状態を単純に同一視できない仕組みがある。
- target tracking、fade-out、target lock、target removal、death は相互作用するため、どれか一つを変更した場合も他の状態遷移を確認する。
- Tween を変更・停止する場合は、Kill 時の complete 有無、参照のクリア、表示値の即時反映順序を確認する。
- 現在の毎フレーム polling は、単に毎フレームであるという理由だけでイベント駆動へ置き換えない。変更前に `docs/performance-notes.md` の実測値を確認する。

## HoverGuide

- `HoverGuide.Show` は比較的高コストな UI 処理として扱う。
- 同一 state から同じ文字列を繰り返し生成・解析しないようにする。ただしキャッシュ invalidation が複雑になる場合は、実測コストと比較して判断する。
- 表示項目追加では、縦方向のジャンプ、行数増加、情報の優先順位も考慮する。
- キャラクター情報とアイテム情報では必要な情報密度が異なるため、単純に項目数を揃えることを目標にしない。

## 参照保持

- UI が追跡する `Card` / `Chara` などの参照は、不要に lifetime を延長しない。
- 長期保持する参照では、必要に応じて WeakReference を検討し、対象破棄後の扱いも確認する。

## Harmony / IL

- `WidgetMouseover` など vanilla メソッド全体を Mod 側へ複製するより、必要な位置だけを変更する限定的な Patch を優先する。
- Transpiler 変更時は、元 instruction sequence と注入位置を確認する。
- 注入位置を選んだ理由がコードから明確でない場合は、将来の更新確認に役立つコメントを残す。
- unused な引数や stack 値を IL で除去する場合、前後の stack balance を必ず確認する。

## Performance

- `HealthBars.Update`、`HoverGuide.Show`、`CharaHoverText` 周辺を変更する場合は `docs/performance-notes.md` を参照する。
- 既存の測定値より遅くなったように見える場合、単発の Max だけではなく Count / Total / Average と測定条件を比較する。
