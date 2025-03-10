using System;
using System.IO;
using System.Linq;
using System.Reflection;
using KSL.API;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

namespace CXS.Utils;

public static class UI
{
    public static GUISkin UniversalSkin;

    public static void TryLoadSkin()
    {
        try
        {
            byte[] bundleData = LoadSkinFromMemory("retroskin");
            AssetBundle asset = AssetBundle.LoadFromMemory(bundleData);
            UniversalSkin = asset.LoadAsset<GUISkin>("Windows98");
        }
        catch (Exception ex)
        {
            Kino.Log.Error("Unable to load skin, error: " + ex.Message);
        }
    }

    private static Byte[] LoadSkinFromMemory(string resourceName)
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        using Stream stream = assembly.GetManifestResourceStream(assembly.GetManifestResourceNames().FirstOrDefault(x => x.EndsWith(resourceName)));

        if (stream == null) return null;

        byte[] data = new byte[stream.Length];
        _ = stream.Read(data, 0, data.Length);
        return data;
    }

    public static Rect CenterWindow(Rect windowRect)
    {
        windowRect.x = (Screen.width - windowRect.width) / 2;
        windowRect.y = (Screen.height - windowRect.height) / 2;
        return windowRect;
    }

    public static bool IncrementedSlider(ref float value, float min, float max, float increment = 1,
        params GUILayoutOption[] options)
    {
        float newVal = GUILayout.HorizontalSlider(value, min, max, options);
        if (Mathf.Approximately(newVal, value))
            return false;

        value = Mathf.Round(newVal / increment) * increment;
        return true;
    }

    public static bool ButtonToggle(ref bool value, string label, params GUILayoutOption[] options)
    {
        Color prevColor = GUI.backgroundColor;
        GUI.backgroundColor = value ? Color.green : Color.red;
        bool clicked = GUILayout.Button(label, options);
        if (clicked)
            value = !value;
        GUI.backgroundColor = prevColor;
        return clicked;
    }

    public static bool Toggle(ref bool value, GameObject targetObject, string label, params string[] fieldNames)
    {
        if (targetObject == null)
            return false;
        Color prevColor = GUI.backgroundColor;
        GUI.backgroundColor = value ? Color.green : Color.red;
        bool clicked = GUILayout.Toggle(value, label);
        if (clicked)
            value = !value;
        GUI.backgroundColor = prevColor;
        if (clicked)
        {
            foreach (Component component in targetObject.GetComponents<Component>())
            {
                Type type = component.GetType();
                foreach (string fieldName in fieldNames)
                {
                    FieldInfo field = type.GetField(fieldName);
                    if (field != null)
                        field.SetValue(component, value);
                }
            }
        }
        
        return clicked;
    }
}