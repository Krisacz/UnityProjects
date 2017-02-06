using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor
{
    [InitializeOnLoad]
    public class FullscreenPlayModeMenu
    {
        public const string EditorPrefName = "FullScreenPlayMode";
        private const string Root = "KS-Tools";
        private const string MenuItem = "Full Screen in Play Mode";
        public const string MenuPath = Root + "/" + MenuItem;

        [MenuItem(MenuPath)]
        private static void NewMenuOption()
        {
            var answer = EditorUtility.DisplayDialog(
                "Full Screen",
                "Would you like to ENABLE Full Screen in Play Mode?",
                "Yes", "No");
            EditorPrefs.SetBool(EditorPrefName, answer);
            Menu.SetChecked(MenuPath, answer);
        }
    }
}
