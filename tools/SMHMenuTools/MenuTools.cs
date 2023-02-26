using UnityEngine;
using UnityEditor;

public class MenuTools : EditorWindow
{
    //переключение режима отображени€
    static DrawCameraMode drawMode;
    [MenuItem("MyTools/Draw mode/Shaded &1")]
    static void WireFrame()
    {
        drawMode = DrawCameraMode.Textured;
        SceneView.lastActiveSceneView.cameraMode = SceneView.GetBuiltinCameraMode(drawMode);
    }
    [MenuItem("MyTools/Draw mode/ShadedWireframe &2")]
    static void Shaded()
    {
        drawMode = DrawCameraMode.TexturedWire;
        SceneView.lastActiveSceneView.cameraMode = SceneView.GetBuiltinCameraMode(drawMode);
    }
    [MenuItem("MyTools/Draw mode/Wireframe &3")]
    static void ShadedWireFrame()
    {
        drawMode = DrawCameraMode.Wireframe;
        SceneView.lastActiveSceneView.cameraMode = SceneView.GetBuiltinCameraMode(drawMode);
    }

    //копирование, вставка, удаление массива компонентов
    static Component[] copiedComponents;
    [MenuItem("MyTools/Components/Copy All Components &C")]
    static void Copy()
    {
        copiedComponents = Selection.activeGameObject.GetComponents<Component>();
    }
    [MenuItem("MyTools/Components//Paste All Components &V")]
    static void Paste()
    {
        foreach (var targetGameObject in Selection.gameObjects)
        {
            if (!targetGameObject || copiedComponents == null) continue;
            foreach (var copiedComponent in copiedComponents)
            {
                if (!copiedComponent) continue;

                Undo.RecordObject(targetGameObject, "UndoPaste");
                UnityEditorInternal.ComponentUtility.CopyComponent(copiedComponent);
                UnityEditorInternal.ComponentUtility.PasteComponentAsNew(targetGameObject);
            }
        }
    }
    [MenuItem("MyTools/Components/Remove All Components &X")]
    static void Remove()
    {
        foreach (var targetGameObjectRemove in Selection.gameObjects)
        {
            Undo.RecordObject(targetGameObjectRemove, "UndoRemove");
            UnityEditorInternal.ComponentUtility.DestroyComponentsMatching(targetGameObjectRemove, c => !(c is Transform));
        }
    }
}


