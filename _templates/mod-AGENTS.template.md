# <Mod名> 固有方針

このディレクトリでは、リポジトリルートの `AGENTS.md` に加えて以下を適用する。
このファイルには「現在の実装の説明」ではなく、この Mod で継続して守りたい不変条件を中心に書く。

## 目的

- この Mod が何を変更するか。
- 何を変更しないか。

## Vanilla behavior

- 維持するべき vanilla の挙動。
- 意図的に変更する挙動。

## State / lifecycle

- 特に注意する state。
- cache / SaveLoad / Map change / Goal 再作成などの境界。

## Harmony

- Patch 方針。
- 壊れやすい injection point や前提。

## Performance

- hot path。
- allocation を避けたい箇所。
- 既存の測定資料がある場合は docs への参照。

## Compatibility

- 他 Mod から観測される状態。
- public member や外部連携で維持すべき契約。

## Verification

- 変更時に必ず確認するゲーム内ケース。
