using System.Collections;
using Sfs2X.Entities.Data;
using SyncMultiplayer;
using UnityEngine;

namespace CXS;

public static class Sync
{
    private static bool init_;

    public static void Init()
    {
        if (init_)
            return;
        NetworkGame game = NetworkController.InstanceGame;
        if (game == null)
            return;
        game.packetHandler.Subscribe(PacketId.Chat, HandleScaleAdjustment);
        init_ = true;
    }

    public static bool SendTo(SmartfoxDataPackage data, int player) => Send(data, [player]);

    private static bool Send(SmartfoxDataPackage data, int[] recipients = null)
    {
        NetworkGame game = NetworkController.InstanceGame;
        NetworkPlayer localPlayer = game?.LocalPlayer;
        if (localPlayer == null || localPlayer.NetworkID == -1)
            return false;
        data.Add("0", (byte)PacketId.Chat);
        if (recipients == null || recipients.Length == 0)
            game.Client.Send(data, true);
        else
            game.Client.Send(data, true, recipients);
        return true;
    }

    private static void HandleScaleAdjustment(NetworkPlayer sender, SmartfoxDataPackage data)
    {
        if (!data.Data.ContainsKey("carScaleX") ||
            !data.Data.ContainsKey("carScaleY") ||
            !data.Data.ContainsKey("carScaleZ") ||
            !data.Data.ContainsKey("carScaleDuration"))
            return;

        if (sender.userCar == null)
            return;

        ISFSObject scaleData = data.Data;
        float x = scaleData.GetFloat("carScaleX");
        float y = scaleData.GetFloat("carScaleY");
        float z = scaleData.GetFloat("carScaleZ");
        float duration = scaleData.GetFloat("carScaleDuration");

        if (duration <= 0)
            duration = 0.1f;

        Vector3 initialScale = sender.userCar.transform.localScale;
        Vector3 targetScale = new Vector3(Mathf.Clamp(x, .1f, 1.5f), Mathf.Clamp(y, .1f, 1.5f), Mathf.Clamp(z, .1f, 1.5f));
        sender.userCar.StartCoroutine(ScaleCarOverTime(sender.userCar, initialScale, targetScale, duration));
    }

    private static IEnumerator ScaleCarOverTime(RaceCar car, Vector3 initialScale, Vector3 targetScale, float duration)
    {
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            car.transform.localScale = Vector3.Lerp(initialScale, targetScale, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        // Ensure the final scale is exactly the target
        car.transform.localScale = targetScale;
    }

    public static void SendSizeAdjustmentData(float x, float y, float z, float duration = 5f)
    {
        if (States.mainCurrent is not SyncNetFreerideRaceModeState)
            return;
        
        SFSObject packet = SFSObject.NewInstance();
        packet.PutFloat("carScaleX", x);
        packet.PutFloat("carScaleY", y);
        packet.PutFloat("carScaleZ", z);
        packet.PutFloat("carScaleDuration", duration);
        Send(new SmartfoxDataPackage(packet));
    }

    public static IEnumerator SendDelayedAdjustmentData(float x, float y, float z, float duration = 5f)
    {
        if (States.mainCurrent is not SyncNetFreerideRaceModeState)
            yield break;
        
        yield return new WaitForSeconds(5f);
        SFSObject packet = SFSObject.NewInstance();
        packet.PutFloat("carScaleX", x);
        packet.PutFloat("carScaleY", y);
        packet.PutFloat("carScaleZ", z);
        packet.PutFloat("carScaleDuration", duration);
        Send(new SmartfoxDataPackage(packet));
    }
}