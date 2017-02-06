using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Curver))]
public class CurverUpd : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
         
        var curver = GameObject.Find("Linex").GetComponent<Curver>();
        if (GUILayout.Button("Build Object"))
        {
            curver.Refresh();
        }
    }
}