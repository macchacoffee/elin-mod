# Elin with AI - Individual Backgrounds

Elin with AIのCharacter Backgroundを、キャラクターのテンプレート単位だけでなく個体単位でも設定できるようにします。

Character Background編集UIの各カードに `Common` / `Individual` の切替を追加します。

- `Common`: `Emmersive/Characters/{chara.UnifiedId}.txt`
- `Individual`: `Emmersive/Characters/Individuals/{Game.id}/{chara.uid}.txt`

Individual Backgroundが存在しない場合、会話時にはElin with AI本来のCommon Backgroundとfallback処理を使用します。
初めてIndividualのEditを開くと、現在有効なCommon Backgroundを初期値として個体ファイルを作成します。
IndividualをResetすると個体ファイルを削除し、Common Backgroundへ戻ります。

Individual Backgroundはワールドごとに分離されます。

Elin with AIが必須です。
