using UnityEngine;

namespace Macchacoffee.ElinMods.NoPCC.Mod;

internal sealed class NoPCCCharaRenderer : CharaRenderer
{
    public override void Draw(RenderParam p, ref Vector3 v, bool drawShadow)
    {
        if (owner.isDead)
        {
            // 死亡中の描画後処理は本体の画面同期に任せ、復活後に通常の描画を再開する。
            return;
        }

        base.Draw(p, ref v, drawShadow);
    }
}
