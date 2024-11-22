using UnityEditor;
using UnityEngine;

public class RenameChildrenEditor : EditorWindow
{
    private static readonly Vector2Int size = new Vector2Int(250, 100);
    private string childrenPrefix;
    private int startIndex;

    [MenuItem("MyTools/Rename children")] 
    public static void ShowWindow()
    {
        EditorWindow window = GetWindow<RenameChildrenEditor>();
        window.minSize = size;
        window.maxSize = size;
    }

    private void OnGUI()
    {
        childrenPrefix = EditorGUILayout.TextField("Children prefix", childrenPrefix);
        startIndex = EditorGUILayout.IntField("Start index", startIndex);
        if (GUILayout.Button("Rename children of parrent"))
        {
            GameObject[] selectObjects = Selection.gameObjects;
            for (int objIndex = 0; objIndex < selectObjects.Length; objIndex++)
            {
                Transform selectObjectT = selectObjects[objIndex].transform;
                for (int childIndex = 0, i = startIndex; childIndex < selectObjectT.childCount; childIndex++)
                {
                    selectObjectT.GetChild(childIndex).name = $"{childrenPrefix}{i++}";
                }
            }
        }
        if (GUILayout.Button("Rename selected objects"))
        {
            GameObject[] selectedObjects = Selection.gameObjects;
            for (int objIndex = 0, i = startIndex; objIndex < selectedObjects.Length; objIndex++, i++)
            {
                selectedObjects[objIndex].name = $"{childrenPrefix}{i}";
            }
        }
    }
}
