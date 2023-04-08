using MelonLoader;
using HarmonyLib;
using Il2Cpp;
using true_pierce_06;

[assembly: MelonInfo(typeof(TruePierce06), "Pierce physical and magic [including Repel] (ver. 0.6)", "1.0.0", "Matthiew Purple")]
[assembly: MelonGame("アトラス", "smt3hd")]

namespace true_pierce_06;
public class TruePierce06 : MelonMod
{
    private static bool hasPierce; // Remembers if the attacker has Pierce or Son's Oath/Raidou the Eternal

    // Before getting the effectiveness of a skill
    [HarmonyPatch(typeof(nbCalc), nameof(nbCalc.nbGetKoukaType))]
    private class Patch
    {
        public static void Prefix(ref int sformindex)
        {
            // 357 = Pierce and 361 = Son's Oath/Raidou the Eternal
            hasPierce = nbMainProcess.nbGetUnitWorkFromFormindex(sformindex).skill.Contains(357) || nbMainProcess.nbGetUnitWorkFromFormindex(sformindex).skill.Contains(361);
        }
    }

    // After getting the effectiveness of an attack on 1 target
    [HarmonyPatch(typeof(nbCalc), nameof(nbCalc.nbGetAisyo))]
    private class Patch2
    {
        public static void Postfix(ref uint __result, ref int attr)
        {
            // If the attack has Pierce (or equivalent) and the attack is physical/magical and it's resisted/blocked/drained/repelled
            if (hasPierce && attr >= 0 && attr <= 4 && (__result < 100 || (__result > 999 && __result < 1000000000)))
            {
                __result = 100; // Forces the affinity to become "neutral"
                nbMainProcess.nbGetMainProcessData().d31_kantuu = 1; // Displays the "Pierced!" message
            }
        }
    }

    // After getting a skill description
    [HarmonyPatch(typeof(datSkillHelp_msg), nameof(datSkillHelp_msg.Get))]
    private class Patch3
    {
        public static void Postfix(ref int id, ref string __result)
        {
            if (id == 357) __result = "Physical attacks ignore \nall resistances."; // New skill description for Pierce
        }
    }
}
