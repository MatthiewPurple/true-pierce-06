using MelonLoader;
using HarmonyLib;
using Il2Cpp;
using true_pierce_06;

[assembly: MelonInfo(typeof(TruePierce06), "Pierce everything (ver. 0.6)", "1.0.0", "Matthiew Purple")]
[assembly: MelonGame("アトラス", "smt3hd")]

namespace true_pierce_06;
public class TruePierce06 : MelonMod
{
    private static bool hasPierce; // Remembers if the attacker has Pierce or Son's Oath/Raidou the Eternal

    // Before getting the effectiveness of a skill
    [HarmonyPatch(typeof(nbCalc), nameof(nbCalc.nbGetKoukaType))]
    private class Patch
    {
        public static void Prefix(ref int sformindex, ref int nskill)
        {
            // If the skill in question is NOT a self-switch (from Zephhyr's mod) nor Analyze
            if (nbMainProcess.nbGetUnitWorkFromFormindex(sformindex) != null)
            {
                // 357 = Pierce and 361 = Son's Oath/Raidou the Eternal
                hasPierce = nskill != 71 && (nbMainProcess.nbGetUnitWorkFromFormindex(sformindex).skill.Contains(357) || nbMainProcess.nbGetUnitWorkFromFormindex(sformindex).skill.Contains(361));
            }
        }
    }

    // After getting the effectiveness of an attack on 1 target
    [HarmonyPatch(typeof(nbCalc), nameof(nbCalc.nbGetAisyo))]
    private class Patch2
    {
        public static void Postfix(ref uint __result, ref int attr)
        {
            // If the attack has Pierce (or equivalent) and it's resisted/blocked/drained/repelled
            if (hasPierce && (__result < 100 || (__result >= 65536 && __result < 2147483648)))
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
            if (id == 357) __result = "All attacks ignore \nall resistances."; // New skill description for Pierce
        }
    }
}
