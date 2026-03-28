using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

namespace ModUtility.Patch;

/// <summary>Harmonyパッチ用ユーティリティ</summary>
public static class ModPatchTools
{
    /// <summary>yeild returnのメソッドに対応するMoveNextメソッドを取得する</summary>
    /// <param name="instructions">yeild returnのメソッドのCodeInstructionリスト</param>
    /// <returns>MoveNextメソッド</returns>
    public static MethodInfo? FindYeildMoveNextMethod(IEnumerable<CodeInstruction> instructions)
    {
        // メソッドがyield returnの場合、コンパイラが内部クラスとMoveNextメソッドを生成する
        // ILから内部クラスを特定し、MoveNextメソッドを取得する
        var matcher = new CodeMatcher(instructions);
        matcher.MatchStartForward(new CodeMatch(OpCodes.Newobj));
        if (matcher.Instruction.operand is not ConstructorInfo ctor)
        {
            return null;
        }
        return AccessTools.Method(ctor.DeclaringType, "MoveNext");
    }
}
