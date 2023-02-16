using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(physicSimulator))]
public class physicSimulatorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        physicSimulator script = (physicSimulator)target;
        if (GUILayout.Button("start simulation"))
        {
            script.General();
        }
    }

}
