using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

public class RTTools : EditorWindow 
{
    //Quick change pivot point
    private bool b = true;
    bool enablePivotPoint;
    private Transform objParent;
    private Transform obj;
    int i;

    //GUI
    private static readonly Vector2Int size = new Vector2Int(250, 100);
    float splitterWidth = 5;

    //zoom
    static float speedZoom = 5;

    //moveObjTo...
    private GameObject objMove;
    bool enableRotation;

    //selection Obj with tag
    string str;
    bool enableFind = false;
    GameObject[] objsFind;

    //selection Obj with mesh filter name
    string str2;
    MeshFilter meshFilter;
    bool enableFindObjs;

    //[MenuItem("MyTools/Change Pivot Point")]
    [MenuItem("MyTools/Options")]
    public static void Init()
    {
        RTTools window = GetWindow<RTTools>();
        window.minSize = size;
        window.maxSize = size;
        window.Show();
    }
    private void OnGUI()
    {
        //zoom
        GUILayout.Label("Zoom");
        speedZoom = EditorGUILayout.FloatField("Speed Zoom:", speedZoom);

        //spliter
        GUILayout.Box("",GUILayout.Height(splitterWidth), GUILayout.Height(splitterWidth), 
        GUILayout.Height(splitterWidth),GUILayout.ExpandWidth(true));

        //Quick change pivot point
        //GUILayout.Label("Quick change pivot point");
        enablePivotPoint = EditorGUILayout.Toggle("Quick change pivot point", enablePivotPoint);

        //spliter
        GUILayout.Box("", GUILayout.Height(splitterWidth), GUILayout.Height(splitterWidth),
        GUILayout.Height(splitterWidth), GUILayout.ExpandWidth(true));

        //moveObjTo...
        GUILayout.Label("Move obj To...");
        enableRotation = EditorGUILayout.Toggle("Enable Rotation", enableRotation);

        //spliter
        GUILayout.Box("", GUILayout.Height(splitterWidth), GUILayout.Height(splitterWidth),
        GUILayout.Height(splitterWidth), GUILayout.ExpandWidth(true));

        //selection Obj with tag
        GUILayout.Label("Selection Obj with tag");
        str = GUILayout.TextField(str);
        if (GUILayout.Button("find"))
        {
            enableFind = true;
        }

        //selection Obj with mesh filter name
        GUILayout.Label("selection Obj with mesh filter name");
        str2 = GUILayout.TextField(str2);
        if (GUILayout.Button("find"))
        {
            enableFindObjs = true;
        }
    }
    private void OnEnable() { SceneView.duringSceneGui += SceneViewDuring; }
    private void OnDisable() { SceneView.duringSceneGui -= SceneViewDuring; }
    private void SceneViewDuring(SceneView scene)
    {
        //selection Obj with mesh filter name
        if (enableFindObjs == true)
        {
            List<GameObject> objectsInScene = new List<GameObject>();
            foreach (GameObject go in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[])
            {
                if (go.GetComponent<MeshFilter>())
                {
                    if (go.GetComponent<MeshFilter>().sharedMesh.name == str2/*go.GetComponent<MeshFilter>().sharedMesh == null*/)
                    {
                        objectsInScene.Add(go);
                        Selection.objects = objectsInScene.ToArray();
                    }
                }
                enableFindObjs = false;
            }
        }

        //selection Obj with tag
        if (str != "")
        {
            if (enableFind == true)
            {
                objsFind = GameObject.FindGameObjectsWithTag(str);
                Selection.objects = objsFind;
                str = "";
                enableFind = false;
            }
        }
        else
        {
            enableFind = false;
        }   

        //moveObjTo... # - shift, % - ctrl, & - alt, SPACE
        if (Event.current.Equals(Event.KeyboardEvent("g")))
        {
            objMove = Selection.activeGameObject;
        }
        if (Event.current.Equals(Event.KeyboardEvent("h")))
        {
            if (objMove != null)
            {
                Undo.RecordObject(objMove.transform, "UndoChangeTransformObj");
                objMove.transform.position = Selection.activeTransform.position;
                if (enableRotation == true)
                {
                    objMove.transform.rotation = Selection.activeTransform.rotation;
                }
                Selection.activeObject = objMove;
            }
        }

        //zoom
        SceneView view = SceneView.lastActiveSceneView;
        if (Event.current.Equals(Event.KeyboardEvent("x")))
        {   
            view.size -= Time.deltaTime * speedZoom;
        }
        if (Event.current.Equals(Event.KeyboardEvent("z")))
        {
            view.size += Time.deltaTime * speedZoom;
        }

        //Quick change pivot point
        if (enablePivotPoint == true)
        {
            if (Selection.activeObject != null)
            {
                if (Tools.pivotMode == PivotMode.Pivot)
                {
                    Event e = Event.current;
                    Ray worldRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                    if (Event.current.Equals(Event.KeyboardEvent("#z")))
                    {
                        if (b == true)
                        {
                            if (Selection.activeTransform.parent != null)
                            {
                                // получение родительского объекта
                                objParent = Selection.activeTransform.parent;
                                obj = Selection.activeTransform;
                                i = obj.GetSiblingIndex();
                            }

                            if (Selection.activeTransform.parent == null)
                            {
                                // получение индекса объекта в иерархии
                                obj = Selection.activeTransform;
                                i = obj.GetSiblingIndex();
                            }

                            obj = Selection.activeTransform;
                            GameObject pivotObj = new GameObject("pivotPointObject");
                            //pivotObj.transform.position = new Vector3(worldRay.origin.x, Selection.activeTransform.position.y, worldRay.origin.z);
                            pivotObj.transform.position = new Vector3(worldRay.origin.x, worldRay.origin.y, worldRay.origin.z);
                            Selection.activeTransform.parent = pivotObj.transform;
                            b = false;
                        }
                    }
                    if (e.type == EventType.KeyUp)
                    {
                        GameObject active = GameObject.Find("pivotPointObject");
                        Selection.activeObject = active;
                        b = true;
                    }
                    if (Event.current.Equals(Event.KeyboardEvent("#x")))
                    {

                        obj.transform.parent = null;
                        GameObject _destroy = GameObject.Find("pivotPointObject");
                        DestroyImmediate(_destroy); //DestroyImmediate удаление объекта в editMode

                        //возвращает объект в его изначальное место в иерархии
                        obj.SetSiblingIndex(i);  
                        if (objParent != null)
                        {
                            //возвращает объект в его изначальный parent
                            obj.transform.parent = objParent;
                            objParent = null;
                            obj.SetSiblingIndex(i);
                        }
                        enablePivotPoint = false;
                        //scene.Repaint();
                    }
                }
                if (Tools.pivotMode != PivotMode.Pivot && Event.current.Equals(Event.KeyboardEvent("#z")))
                {
                    Tools.pivotMode = PivotMode.Pivot;
                }
            }
            if (Selection.activeObject == null && Event.current.Equals(Event.KeyboardEvent("#z")))
            {
                Debug.Log("Select Object");
            } 
        }
    }
}

