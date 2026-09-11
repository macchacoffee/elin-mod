# パフォーマンス測定記録

性能に関する設計を変更する前に、既に測定済みの結果がないかここを確認する。
単純に「毎フレーム」「Reflection」「Regex」などの語だけで高コストと判断しない。

## Somewhat Enhanced Display

### HealthBars.Update polling 導入後の測定

HealthBar の対象状態を毎フレーム確認する設計について、Stopwatch ベースの測定を実施済み。

#### Case A

旧版:

- `HealthBars.Update`: samples なし
- `HoverGuide.Show`: Count=580, Total=337.669 ms, Average=582.188 us, Max=1050.500 us
- `CharaHoverText`: Count=551, Total=6.762 ms, Average=12.272 us, Max=176.100 us

新版:

- `HealthBars.Update`: Count=3,214, Total=55.213 ms, Average=17.179 us, Max=79.400 us
- `HoverGuide.Show`: Count=572, Total=355.402 ms, Average=621.332 us, Max=5119.800 us
- `CharaHoverText`: Count=570, Total=5.313 ms, Average=9.321 us, Max=166.200 us

#### Case B: target 変更を多めに行った測定

旧版:

- `HealthBars.Update`: samples なし
- `HoverGuide.Show`: Count=1,239, Total=821.926 ms, Average=663.379 us, Max=4826.300 us
- `CharaHoverText`: Count=1,126, Total=12.619 ms, Average=11.207 us, Max=171.800 us

新版:

- `HealthBars.Update`: Count=7,395, Total=103.364 ms, Average=13.978 us, Max=1671.300 us
- `HoverGuide.Show`: Count=1,277, Total=782.433 ms, Average=612.712 us, Max=1718.600 us
- `CharaHoverText`: Count=1,252, Total=11.586 ms, Average=9.254 us, Max=106.200 us

### 現時点の判断

- `HealthBars.Update` の毎フレーム polling には追加コストがあるが、平均値は約 14〜17 us / call の測定結果だった。
- 同じ測定で `HoverGuide.Show` は約 600 us / call 規模であり、HealthBar polling だけを「毎フレームだから」という理由で再設計する根拠にはならない。
- polling を削除・イベント駆動化する場合は、性能だけでなく「ダメージ発生時にすぐアニメーションが始まる」「lock / fade / death の状態を正しく追従する」という現在の挙動を維持できるか確認する。
- Max は一時的な spike の影響を受けるため、Average / Total / Count と測定条件を優先して比較する。

## Container Performance Fix

### `(id, idMaterial)` bucket 探索のマイクロベンチ

対象:

- `UIInventory.Sort()` 冒頭のスタック候補探索を模した、全 item の key が異なるケース。
- vanilla 相当の全組み合わせ走査と、`Dictionary<StackKey, ...>` 構築・lookup を比較。

測定条件:

- .NET SDK 10.0.112 / Windows
- 各 item の `TryStackTo()` 本体は呼ばず、候補探索構造だけを測定。
- 小さい Count は反復回数を増やし、Count 500 以上は各20回の平均。
- Elin / Unity Mono 上の実測ではないため、絶対時間や倍率はゲーム内性能を保証しない。

| Count | 全組み合わせ走査 | bucket | 比率 |
|---:|---:|---:|---:|
| 100 | 189.82 us | 14.23 us | 13.3x |
| 500 | 4,270.70 us | 73.59 us | 58.0x |
| 1,000 | 12,373.55 us | 143.92 us | 86.0x |
| 2,000 | 20,716.97 us | 191.34 us | 108.3x |
| 4,000 | 92,056.31 us | 419.98 us | 219.2x |

小規模時の結果:

- Count 8: 全走査 1.40 us / bucket 1.30 us
- Count 16: 全走査 4.17 us / bucket 2.38 us
- Count 24: 全走査 9.25 us / bucket 3.53 us

判断:

- 全 item の key が異なる主対象では、Count に伴う探索コストの増加を大幅に抑えられる。
- Dictionary allocation の利益が小さい Count 16 以下は、GC を増やさないため vanilla 相当探索を維持する。
- 同一 key だがスタック不可能な item ばかりの場合は、bucket 内探索が O(n²) のまま残る。

## 測定を追加するときの形式

```text
対象:
変更内容:
測定条件:
ゲーム version:
サンプル数:
Total:
Average:
Max:
GC / allocation の観察:
結論:
```
