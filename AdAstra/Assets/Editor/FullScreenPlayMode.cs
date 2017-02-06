using UnityEditor;
using UnityEngine;
using System.Collections;
using Assets.Editor;

[InitializeOnLoad]
public class FullscreenPlayMode : MonoBehaviour
{
    //The size of the toolbar above the game view, excluding the OS border.
    private static int tabHeight = 22;
    

    static FullscreenPlayMode()
    {
        var value = EditorPrefs.GetBool(FullscreenPlayModeMenu.EditorPrefName);
        if (!value) return;

        EditorApplication.playmodeStateChanged -= CheckPlayModeState;
        EditorApplication.playmodeStateChanged += CheckPlayModeState;
    }

    private static void CheckPlayModeState()
    {
        if (EditorApplication.isPlaying)
        {
            FullScreenGameWindow();
        }
        else
        {
            CloseGameWindow();
        }
    }

    private static EditorWindow GetMainGameView()
    {
        EditorApplication.ExecuteMenuItem("Window/Game");
        var type = System.Type.GetType("UnityEditor.GameView,UnityEditor");
        var getMainGameView = type.GetMethod("GetMainGameView", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        var res = getMainGameView.Invoke(null, null);
        return (EditorWindow)res;
    }

    private static void FullScreenGameWindow()
    {
        var gameView = GetMainGameView();
        gameView.titleContent.text = "Game (Stereo)";
        var newPos = new Rect(0, 0 - tabHeight, Screen.currentResolution.width, 
            Screen.currentResolution.height + tabHeight);
        gameView.position = newPos;
        gameView.minSize = new Vector2(Screen.currentResolution.width, 
            Screen.currentResolution.height + tabHeight);
        gameView.maxSize = gameView.minSize;
        gameView.position = newPos;
    }

    private static void CloseGameWindow()
    {
        var gameView = GetMainGameView();
        gameView.Close();

        //TODO Error fix - when exiting play mode "tick" from menu is removed - I'm re-applying it here
        Menu.SetChecked(FullscreenPlayModeMenu.MenuPath, true);
    }
}