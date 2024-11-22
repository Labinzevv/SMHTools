using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

public class MenuTools : EditorWindow
{
    //переключение режима отображения
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

    //перемещение объекта с помощью стрелок.
    //Выставить значение шага во вкладке MyTools-RTTools-MoveObj-Distance. Перемещать объект ctrl + стрелки клавиатуры
    static RTTools RTTools;
    static float distance;
    static float x;
    static float y;
    static float z;
    static SceneView scene;

    [MenuItem("MyTools/move Obj/move left %LEFT")]
    static void moveLeft()
    {
        distance = RTTools.distance;
        x = Selection.activeTransform.position.x;
        y = Selection.activeTransform.position.y;
        z = Selection.activeTransform.position.z;

        //перемещение зависящее от проекции вида
        if (RTTools.noAxis == true)
        {
            if (SceneView.lastActiveSceneView.rotation == Quaternion.Euler(90f, 0f, 0f)    //Top
                 || SceneView.lastActiveSceneView.rotation == Quaternion.Euler(0f, 0f, 0f)     //Back
                 || SceneView.lastActiveSceneView.rotation == Quaternion.Euler(-90f, 0f, 0f))  //bottom
            {
                Selection.activeTransform.position = new Vector3(x - distance, y, z);
            }
            //left
            if (SceneView.lastActiveSceneView.rotation == Quaternion.Euler(0f, 90f, 0f))
            {
                Selection.activeTransform.position = new Vector3(x, y, z + distance);
            }
            //Right
            if (SceneView.lastActiveSceneView.rotation == Quaternion.Euler(0f, -90f, 0f))
            {
                Selection.activeTransform.position = new Vector3(x, y, z - distance);
            }
            //front
            if (SceneView.lastActiveSceneView.rotation == Quaternion.Euler(0f, 180f, 0f))
            {
                Selection.activeTransform.position = new Vector3(x + distance, y, z);
            }
        }
      
    }
    [MenuItem("MyTools/move Obj/move right %RIGHT")]
    static void moveRight()
    {
        distance = RTTools.distance;
        x = Selection.activeTransform.position.x;
        y = Selection.activeTransform.position.y;
        z = Selection.activeTransform.position.z;
        //перемещение зависящее от проекции вида
        if (RTTools.noAxis == true)
        {
            if (SceneView.lastActiveSceneView.rotation == Quaternion.Euler(90f, 0f, 0f)   //Top
                   || SceneView.lastActiveSceneView.rotation == Quaternion.Euler(0f, 0f, 0f)    //Back
                   || SceneView.lastActiveSceneView.rotation == Quaternion.Euler(-90f, 0f, 0f)) //bottom
            {
                Selection.activeTransform.position = new Vector3(x + distance, y, z);
            }
            //left
            if (SceneView.lastActiveSceneView.rotation == Quaternion.Euler(0f, 90f, 0f))
            {
                Selection.activeTransform.position = new Vector3(x, y, z - distance);
            }
            //Right
            if (SceneView.lastActiveSceneView.rotation == Quaternion.Euler(0f, -90f, 0f))
            {
                Selection.activeTransform.position = new Vector3(x, y, z + distance);
            }
            //front
            if (SceneView.lastActiveSceneView.rotation == Quaternion.Euler(0f, 180f, 0f))
            {
                Selection.activeTransform.position = new Vector3(x - distance, y, z);
            }
        }
    }
    [MenuItem("MyTools/move Obj/move up %UP")]
    static void moveUp()
    {
        distance = RTTools.distance;
        x = Selection.activeTransform.position.x;
        y = Selection.activeTransform.position.y;
        z = Selection.activeTransform.position.z;

        //перемещение не зависящее от проекции вида
        if (RTTools.axsisX == true)
        {
            Selection.activeTransform.position = new Vector3(x + distance, y, z);
        }
        else if (RTTools.axsisY == true)
        {
            Selection.activeTransform.position = new Vector3(x, y + distance, z);
        }
        else if (RTTools.axsisZ == true)
        {
            Selection.activeTransform.position = new Vector3(x, y, z + distance);
        }

        //перемещение зависящее от проекции вида
        if (RTTools.noAxis == true)
        {
            if (SceneView.lastActiveSceneView.rotation == Quaternion.Euler(0f, 180f, 0f) //front
        || SceneView.lastActiveSceneView.rotation == Quaternion.Euler(0f, -90f, 0f) //Right
        || SceneView.lastActiveSceneView.rotation == Quaternion.Euler(0f, 0f, 0f)   //Back
        || SceneView.lastActiveSceneView.rotation == Quaternion.Euler(0f, 90f, 0f)) //left
            {
                Selection.activeTransform.position = new Vector3(x, y + distance, z);
            }
            //Top
            if (SceneView.lastActiveSceneView.rotation == Quaternion.Euler(90f, 0f, 0f))
            {
                Selection.activeTransform.position = new Vector3(x, y, z + distance);
            }
            //bottom
            if (SceneView.lastActiveSceneView.rotation == Quaternion.Euler(-90f, 0f, 0f))
            {
                Selection.activeTransform.position = new Vector3(x, y, z - distance);
            }
        }
    }
    [MenuItem("MyTools/move Obj/move down %DOWN")]
    static void moveDown()
    {
        distance = RTTools.distance;
        x = Selection.activeTransform.position.x;
        y = Selection.activeTransform.position.y;
        z = Selection.activeTransform.position.z;

        //перемещение не зависящее от проекции вида
        if (RTTools.axsisX == true)
        {
            Selection.activeTransform.position = new Vector3(x - distance, y, z);
        }
        else if (RTTools.axsisY == true)
        {
            Selection.activeTransform.position = new Vector3(x, y - distance, z);
        }
        else if (RTTools.axsisZ == true)
        {
            Selection.activeTransform.position = new Vector3(x, y, z - distance);   
        }

        //перемещение зависящее от проекции вида
        if (RTTools.noAxis == true)
        {
            if (SceneView.lastActiveSceneView.rotation == Quaternion.Euler(0f, 0f, 0f)    //Back
                   || SceneView.lastActiveSceneView.rotation == Quaternion.Euler(0f, 90f, 0f)   //left
                   || SceneView.lastActiveSceneView.rotation == Quaternion.Euler(0f, -90f, 0f)  //Right
                   || SceneView.lastActiveSceneView.rotation == Quaternion.Euler(0f, 180f, 0f)) //front
            {
                Selection.activeTransform.position = new Vector3(x, y - distance, z);
            }
            //Top
            if (SceneView.lastActiveSceneView.rotation == Quaternion.Euler(90f, 0f, 0f))
            {
                Selection.activeTransform.position = new Vector3(x, y, z - distance);
            }
            //bottom
            if (SceneView.lastActiveSceneView.rotation == Quaternion.Euler(-90f, 0f, 0f))
            {
                Selection.activeTransform.position = new Vector3(x, y, z + distance);
            }
        }
    }

    //копирование, вставка, удаление массива компонентов
    static Component[] copiedComponents;
    [MenuItem("MyTools/Components/Copy All Components &C")]
    static void CopyAll()
    {
        copiedComponents = Selection.activeGameObject.GetComponents<Component>();
    }
    [MenuItem("MyTools/Components//Paste All Components &V")]
    static void PasteAll()
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
    static void RemoveAll()
    {
        foreach (var targetGameObjectRemove in Selection.gameObjects)
        {
            Undo.RecordObject(targetGameObjectRemove, "UndoRemove");
            UnityEditorInternal.ComponentUtility.DestroyComponentsMatching(targetGameObjectRemove, c => !(c is Transform));
        }
    }

    ////копировать и вставить значения компонента transform  # - shift, % - ctrl, & - alt _a -хоткей отдельная буква(любая перед которой стоит символ "_")
    //static GameObject obj;
    //[MenuItem("MyTools//Move obj To.../Select obj A &S")]
    //static void copyTransform()
    //{
    //    obj = Selection.activeGameObject;
    //}
    //[MenuItem("MyTools/Move obj To.../Move obj A to obg B &D")]
    //static void pasteTransform()
    //{
    //    if (obj != null)
    //    {
    //        Undo.RecordObject(obj.transform, "UndoChangeTransformObj");
    //        obj.transform.position = Selection.activeTransform.position;
    //        //obj.transform.rotation = Selection.activeTransform.rotation;
    //    }
    //}

    ////копировать и вставить значения компонента transform второй вариант
    //static Vector3 copyPos;
    //static Quaternion copyRot;
    //[MenuItem("MyTools/Transform value/Copy transform &S")]
    //static void copyTransform()
    //{
    //    copyPos = Selection.activeTransform.position;
    //    copyRot = Selection.activeTransform.rotation;
    //}
    //[MenuItem("MyTools/Transform value/Paste transform &D")]
    //static void pasteTransform()
    //{
    //    Selection.activeTransform.position = copyPos;
    //    Selection.activeTransform.rotation = copyRot;
    //}

    //Класс для выполнения действий в editMode (когда активно окно MyTools/Options)
    //[MenuItem("MyTools/Options")]
    //public static void Init()
    //{
    //    CameraTools window = GetWindow<CameraTools>();
    //    window.Show();
    //}
    //private void OnGUI()
    //{

    //}
    //private void OnEnable() { SceneView.duringSceneGui += SceneViewDuring; }
    //private void OnDisable() { SceneView.duringSceneGui -= SceneViewDuring; }
    //private void SceneViewDuring(SceneView scene)
    //{

    //}
}


