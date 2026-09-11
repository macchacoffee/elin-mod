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

### `StackKey` 拡張後の合成ケース比較

対象:

- 約3,000 item に対して、旧 `(id, idMaterial)` key と、`Thing.CanStackTo()` が
  `Trait.CanStackTo()` より前に厳密一致を要求する全フィールドを含む key を比較。
- Case A は旧 key ごとに100件、新 keyでは `c_idRefCard` により10件ずつに分かれる合成分布。
- Case B は1,800件の一意 item と、旧 keyが同一の死体1,200件を用意し、死体は
  `c_idRefCard` により12 bucket（各100件）へ分かれる合成分布。

測定条件:

- .NET SDK 10.0.112 / Windows
- bucket 構築と、統合が一度も成功しない場合の全候補列挙を計測。
- warmup 5回後、各30回の中央値。
- `TryStackTo()` 本体、Elin / Unity Mono、実セーブデータを使った測定ではない。
  候補数の変化を中心に見るための合成マイクロベンチであり、ゲーム内絶対時間は保証しない。

| ケース | 旧最大 bucket | 拡張後最大 bucket | 旧候補数 | 拡張後候補数 | 旧中央値 | 拡張後中央値 |
|---|---:|---:|---:|---:|---:|---:|
| A | 100 | 10 | 297,000 | 27,000 | 2,139.30 us | 1,696.90 us |
| B | 1,200 | 100 | 1,438,800 | 118,800 | 8,354.30 us | 2,277.20 us |

互換性モデル確認:

- 追加した各 key フィールド単独の差異で候補が分離されることを確認。
- 染色差異と `Trait.CanStackTo()` 後の状態は key に含めず、最終判定まで到達することを確認。
- `compress` 成功時の count / decay / enc / elements 更新を確認。
- 40 item × 1,000 seed のランダム入力で、vanilla 順序モデル、旧 key、拡張 key の
  最終状態と `Trait.CanStackTo()` 相当の呼び出し順序が一致。

結論:

- 同じ `(id, idMaterial)` が集中する場合でも、trait 判定前に必ず不一致となる候補を
  bucket 構築時点で除外できる。
- 実際の改善幅は保存されている参照カードなどの分布に依存するため、上記の倍率を
  実セーブデータへそのまま当てはめない。

### per-bucket merge / live compaction の合成ケース比較

対象:

- StackKey と bucket 内の source / target / restart 順序を変えず、container 全体ではなく
  初出順の bucket ごとにmergeを完結させる方式を旧方式と比較。
- mergeで残ったdestroy済み参照は、32件以上かつbucket Listの25%以上になった時点で
  restart前に `RemoveAll()` する。

測定条件:

- .NET SDK 10.0.112 / Windows
- warmup 3回後、各15回の中央値。
- Case A: 3,000 Thing、30 bucket、最大100件、全候補がスタック不可。
- Case B: 3,000 Thing、1,801 bucket、最大1,200件。最大bucketは4件ずつスタック可能な
  300グループで、merge成功900回。
- Case C: 1,200 Thingが同じbucketに属し、全件がスタック可能。merge成功1,199回。
- Elin / Unity Monoや実セーブではなく、探索構造と状態更新を模した合成モデル。
  絶対時間はゲーム内性能を保証しない。

| ケース | 旧 TryStackTo相当 | 新 TryStackTo相当 | 旧destroyed skip | 新destroyed skip | compact回数 / 削除数 | 旧中央値 | 新中央値 |
|---|---:|---:|---:|---:|---:|---:|---:|
| A | 297,000 | 297,000 | 0 | 0 | 0 / 0 | 3,413.60 us | 3,399.90 us |
| B | 80,820,600 | 80,820,600 | 81,404,550 | 12,933,364 | 4 / 821 | 1,270,998.90 us | 885,518.30 us |
| C | 1,199 | 1,199 | 719,400 | 206,254 | 11 / 1,177 | 4,111.40 us | 1,342.70 us |

Case Cの新方式でcompactionを無効にした場合:

- TryStackTo相当: 1,199
- destroyed skip: 1,436,402
- 中央値: 8,097.50 us

互換性モデル確認:

- 4件の同一bucketで `A -> B`, `B -> C`, `C -> D` の順にmergeし、UID Dが生存。
- mixed / compress ON・OFF / food / corpse / equipment / decayed / hotbar相当状態を含む
  40 Thing × 1,000 seedで旧方式と新方式を比較。
- Thing数、生存uid、id、idMaterial、Num、decay、encLV、elements、invX、invYが一致。
- bucketごとのmerge方向とsource / target順も一致。

結論:

- 同じbucket内の比較を省略しないため、TryStackTo相当回数は旧方式と同一。
- mergeがないCase Aは実質同等。mergeが多いCase Bでは全体restartの除去、Case Cでは
  live compactionによるdestroy済み参照の除去が主な改善要因になった。
- Case Cのdestroyed skipは旧方式比約71%、compactionなしの新方式比約86%減少した。

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
