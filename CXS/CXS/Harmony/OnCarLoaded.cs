using HarmonyLib;
using KSL.API;

namespace CXS.Harmony;

public class OnCarLoaded
{
    // stored default values
    public static float FrontDownForce;
    public static float RearDownForce;
    public static float CarMass;
    
    [HarmonyPrefix]
    [HarmonyPatch(typeof(RaceCar), "OnCarLoaded")]
    // ReSharper disable once InconsistentNaming
    public static void OnCarLoadPrefix(RaceCar __instance)
    {
        if (__instance == null)
            return;

        if (States.mainCurrent is not SyncNetFreerideRaceModeState)
            return;
        
        switch (__instance.isLocalPlayer)
        {
            // TODO: Fixed the delayed sync. it don't work. it claims to send but its not..
            case false:
                Sync.SendDelayedAdjustmentData(__instance.transform.localScale.x, __instance.transform.localScale.y, __instance.transform.localScale.z, CarScaler.ScaleAnimationDuration);
                //__instance.StartCoroutine(Sync.SendDelayedAdjustmentData(__instance.transform.localScale.x, __instance.transform.localScale.y, __instance.transform.localScale.z, CarScaler.ScaleAnimationDuration));
                Kino.Log.Info("Adjustment data sent after delay!");
                break;
            
            case true:
                Kino.Log.Info(__instance.carX.aeroRearDownforce + " Rear Downforce, " + __instance.carX.aeroFrontDownforce + " Front Downforce");
                Kino.Log.Info($"Current Car Mass: {__instance.carX.m_carDesc.weight.mass}");
                RearDownForce = __instance.carX.aeroRearDownforce;
                FrontDownForce = __instance.carX.aeroFrontDownforce;
                CarMass = __instance.carX.m_carDesc.weight.mass;
                Main.CarMass = __instance.carX.m_carDesc.weight.mass;
                Main.FrontDownforce = __instance.carX.aeroFrontDownforce;
                Main.RearDownforce = __instance.carX.aeroRearDownforce;
                break;
        }
    }
}

















































