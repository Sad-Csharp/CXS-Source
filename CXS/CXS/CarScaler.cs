using System.Collections;
using KSL.API;
using UnityEngine;

namespace CXS;

public static class CarScaler
{
    public static float LocalScaleX = 1f;
    public static float LocalScaleY = 1f;
    public static float LocalScaleZ = 1f;
    public static float ScaleAnimationDuration = 1f;
    public static bool ScaleAnimationRunning;
    
    public static IEnumerator ScaleCarTimer(RaceCar car, float x, float y, float z, float duration = 1f)
    {
        if (car == null)
            yield break;

        ScaleAnimationRunning = true;

        float elapsedTime = 0;
        Vector3 initialScale = car.transform.localScale;
        Vector3 targetScale = new(Mathf.Clamp(x, .1f, 1.5f), Mathf.Clamp(y, .1f, 1.5f), Mathf.Clamp(z, .1f, 1.5f));

        Main.ButtonText = "Scaling...";

        while (elapsedTime < duration)
        {
            car.transform.localScale = Vector3.Lerp(initialScale, targetScale, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            Main.ButtonText = $"Scaling... ({duration - elapsedTime:F2}s)";
            yield return null;
        }

        // Ensure the final scale is exactly the target
        car.transform.localScale = targetScale;

        ScaleAnimationRunning = false;

        if (States.mainCurrent is not SyncNetFreerideRaceModeState)
        {
            Kino.Log.Error("SyncNetFreerideRaceModeState is null, unable to send adjustment data.");
            yield break;
        }

        Sync.SendSizeAdjustmentData(Main.LocalPlayerCar.transform.localScale.x, Main.LocalPlayerCar.transform.localScale.y, Main.LocalPlayerCar.transform.localScale.z, ScaleAnimationDuration);
    }
}