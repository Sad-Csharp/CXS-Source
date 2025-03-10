using HarmonyLib;

namespace CXS.Harmony;

public class OnCarLoaded
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(RaceCar), "OnCarLoaded")]
    // ReSharper disable once InconsistentNaming
    public static void OnCarLoadPrefix(RaceCar __instance)
    {
        if (__instance == null)
            return;

        if (States.mainCurrent is not SyncNetFreerideRaceModeState)
            return;
        
        if (!__instance.isLocalPlayer)
            Sync.SendDelayedAdjustmentData(__instance.transform.localScale.x, __instance.transform.localScale.y, __instance.transform.localScale.z, CarScaler.ScaleAnimationDuration);
            
    }
}

















































