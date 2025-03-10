using System;
using KSL.API;

namespace CXS.Harmony;

public static class Patcher
{
    private static HarmonyLib.Harmony patcher_;

    public static void Init()
    {
        if (patcher_ != null)
            return;

        patcher_ = new HarmonyLib.Harmony("CXS.patcher");
        Kino.Log.Warning("Patcher initialized.");
    }

    public static void TryPatch(Type type)
    {
        try
        {
            patcher_.PatchAll(type);
            Kino.Log.Warning("Patch applied for " + type.Name);
        }
        catch (Exception ex)
        {
            Kino.Log.Error("Unable to apply patches for " + type.Name + ", error: " + ex.Message);
        }
    }
}