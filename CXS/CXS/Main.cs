using System;
using System.Reflection;
using CXS.Harmony;
using HUD;
using KSL.API;
using UnityEngine;

namespace CXS;

[KSLMeta("CXS", "1.0.0", "Mizar")]
public class Main : BaseMod
{
    #region Declarations

    public static RaceCar LocalPlayerCar => !PlayerCarControl.instance ? GameObject.Find("CarPositionMarker").GetComponent<RaceCar>() : PlayerCarControl.instance.car;
    private Rect windowRect_ = new Rect(0, 0, 200, 200);
    private bool showWindow_;
    public static string ButtonText;

    #endregion

    private void Start()
    {
        Utils.UI.TryLoadSkin();
        windowRect_ = Utils.UI.CenterWindow(windowRect_);
        Patcher.Init();
        Patcher.TryPatch(typeof(OnCarLoaded));
        Kino.Input.Bind(KeyCode.None, ToggleUI, "Toggle UI");
        Kino.Input.Bind(KeyCode.None, ScaleCar, "Scale Car");
        Kino.Input.Bind(KeyCode.None, ResetCarScale, "Reset Scale");
    }

    private void Update()
    {
        Sync.Init();
    }

    private void OnGUI()
    {
        if (!showWindow_)
            return;

        if (Utils.UI.UniversalSkin != null)
            GUI.skin = Utils.UI.UniversalSkin;

        windowRect_ = GUILayout.Window(GetHashCode(), windowRect_, DrawUI, "CSX");
    }

    private void DrawUI(int windowID)
    {
        // Scale settings
        using (new GUILayout.VerticalScope("box"))
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("<b>Car Scaler</b>");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Label($"Scale Animation Speed: {CarScaler.ScaleAnimationDuration}");
            Utils.UI.IncrementedSlider(ref CarScaler.ScaleAnimationDuration, 1f, 10f);
            
            GUILayout.Space(5f);
            
            // All Scales       
            GUILayout.BeginHorizontal();                                                                                             
            if (GUILayout.Button("<"))
            {
                CarScaler.LocalScaleX = Mathf.Clamp(CarScaler.LocalScaleX - .1f, .1f, 1.5f);
                CarScaler.LocalScaleY = Mathf.Clamp(CarScaler.LocalScaleY - .1f, .1f, 1.5f);
                CarScaler.LocalScaleZ = Mathf.Clamp(CarScaler.LocalScaleZ - .1f, .1f, 1.5f);
            }
            
            GUILayout.FlexibleSpace();
            GUILayout.Label("All Scales");
            GUILayout.FlexibleSpace();
            
            if (GUILayout.Button(">"))
            {
                CarScaler.LocalScaleX = Mathf.Clamp(CarScaler.LocalScaleX + .1f, .1f, 1.5f);
                CarScaler.LocalScaleY = Mathf.Clamp(CarScaler.LocalScaleY + .1f, .1f, 1.5f);
                CarScaler.LocalScaleZ = Mathf.Clamp(CarScaler.LocalScaleZ + .1f, .1f, 1.5f);
            }
            GUILayout.EndHorizontal();
            
            // X Scale
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("<"))
            {
                CarScaler.LocalScaleX = Mathf.Clamp(CarScaler.LocalScaleX - .1f, .1f, 1.5f);
            }

            GUILayout.FlexibleSpace();
            GUILayout.Label($"X Scale: {CarScaler.LocalScaleX:F1}");
            GUILayout.FlexibleSpace();
            
            if (GUILayout.Button(">" , GUILayout.ExpandWidth(false)))
            {
                CarScaler.LocalScaleX = Mathf.Clamp(CarScaler.LocalScaleX + .1f, .1f, 1.5f);
            }
            GUILayout.EndHorizontal();
                
            // Y Scale
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("<"))
            {
                CarScaler.LocalScaleY = Mathf.Clamp(CarScaler.LocalScaleY - .1f, .1f, 1.5f);
            }

            GUILayout.FlexibleSpace();
            GUILayout.Label($"Y Scale: {CarScaler.LocalScaleY:F1}");
            GUILayout.FlexibleSpace();
            
            if (GUILayout.Button(">"))
            {
                CarScaler.LocalScaleY = Mathf.Clamp(CarScaler.LocalScaleY + .1f, .1f, 1.5f);
            }
            GUILayout.EndHorizontal();
                
            // Z Scale
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("<"))
            {
                CarScaler.LocalScaleZ = Mathf.Clamp(CarScaler.LocalScaleZ - .1f, .1f, 1.5f);
            }

            GUILayout.FlexibleSpace();
            GUILayout.Label($"Z Scale: {CarScaler.LocalScaleZ:F1}");
            GUILayout.FlexibleSpace();
            
            if (GUILayout.Button(">"))
            {
                CarScaler.LocalScaleZ = Mathf.Clamp(CarScaler.LocalScaleZ + .1f, .1f, 1.5f);
            }
            GUILayout.EndHorizontal();
            
            if (GUILayout.Button(CarScaler.ScaleAnimationRunning ? ButtonText : "Scale Car"))
            {
                ScaleCar();
            }
            
            if (GUILayout.Button(CarScaler.ScaleAnimationRunning ? ButtonText : "Reset Scale"))
            {
                ResetCarScale();
            }
        }
        
        GUI.DragWindow();
    }
    
    public override void OnAdditionalAboutUIDraw()
    {
        Kino.UI.Label("A basic mod to adjust the local scale of your car with sync!");
        Kino.UI.Label("If you find any bugs or have any suggestions, please let me know.");
        Kino.UI.Label("Discord: Sad_User");
    }
    
    public override Texture2D GetIcon()
    {
        Assembly executingAssembly = Assembly.GetExecutingAssembly();
        return Kino.Utils.LoadEmbeddedTexture(executingAssembly, "CXS.Resources.icon.png");
    }
    
    private void ToggleUI()
    {
        showWindow_ = !showWindow_;
    }

    private void ScaleCar()
    {
        if (LocalPlayerCar == null)
            return;
        
        if (LocalPlayerCar.transform.localScale == new Vector3(CarScaler.LocalScaleX, CarScaler.LocalScaleY, CarScaler.LocalScaleZ))
            return;
        
        if (States.mainCurrent is GarageGUIState)
        {
            GUICommonGodVoice.ShowText("Join a lobby or training session first!", 3f);
            return;
        }
                
        if (CarScaler.ScaleAnimationRunning)
            GUICommonGodVoice.ShowText("Scale animation in progress, please wait.", 3f);
                
        StartCoroutine(CarScaler.ScaleCarTimer(LocalPlayerCar, CarScaler.LocalScaleX, CarScaler.LocalScaleY, CarScaler.LocalScaleZ, CarScaler.ScaleAnimationDuration));
    }

    private void ResetCarScale()
    {
        if (LocalPlayerCar == null)
            return;
        
        if (States.mainCurrent is GarageGUIState)
        {
            GUICommonGodVoice.ShowText("Join a lobby or training session first!", 3f);
            return;
        }
                
        CarScaler.LocalScaleX = 1f;
        CarScaler.LocalScaleY = 1f;
        CarScaler.LocalScaleZ = 1f;
        StartCoroutine(CarScaler.ScaleCarTimer(LocalPlayerCar, 1f, 1f, 1f));
        try
        {
            if (States.mainCurrent is SyncNetFreerideRaceModeState)
                UINotifications.instance.Add("Scale reset!", Color.green);
            else
                GUICommonGodVoice.ShowText("Scale reset!", 3f);
        }
        catch (Exception e)
        {
            Kino.Log.Error("Unable to show notification in reset scale, error: " + e.Message);
        }
    }
    
    private void OnApplicationQuit()
    {
        Utils.Config.Instance.Save();
    }
}