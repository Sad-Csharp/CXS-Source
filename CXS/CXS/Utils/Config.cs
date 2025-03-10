using System.IO;
using KSL.API;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

namespace CXS.Utils;

public class Config
{
    public static Config Instance { get; private set; }
    private static readonly string ConfigPath = Path.Combine(Directory.GetParent(Application.dataPath)?.FullName ?? string.Empty, "kino", "config", "CXS.json");

    static Config()
    {
        Instance = new Config();
    }

    public void TryLoadConfig()
    {
        if (!File.Exists(ConfigPath))
        {
            Instance = new Config();
            Instance.Save();
            Kino.Log.Error("Config file not found, creating new one.");
        }
        else
        {
            Instance = File.ReadAllText(ConfigPath).FromJson<Config>();
            Kino.Log.Warning("Config file loaded.");
        }
    }

    public void Save()
    {
        File.WriteAllText(ConfigPath, this.ToJson());
        Kino.Log.Warning("Config file saved.");
    }
}