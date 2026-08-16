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
