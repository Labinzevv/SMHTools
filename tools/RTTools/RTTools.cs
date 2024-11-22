using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

public class RTTools : EditorWindow 
{
    //������ ���������
    private Vector2 scrollPosition;
    private Vector2 maxScroll = new Vector2(200, 200); // ������������ �������� ������ ���������

    //GUI 
    private static readonly Vector2Int size = new Vector2Int(250, 100);
    float splitterWidth = 5;

    //Quick change pivot point
    private bool b = true;
    bool enablePivotPoint;
    private Transform objParent;
    private Transform obj;
    int i;

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
    bool enableFindObjs = false;
    
    //selection Obj with material name
    string str3;
    bool enableFindMat = false;

    //selection random child Objs 
    bool enableRandom = false;

    //move obj ����������� ������� � ������� ������� (� ������� MenuTools)
    public static float distance;

    public static bool noAxis;
    public static bool axsisX;
    public static bool axsisY;
    public static bool axsisZ;

    static bool previousNoAxis;
    static bool previousAxsisX;
    static bool previousAxsisY;
    static bool previousAxsisZ;

    static bool newNoAxsis;
    static bool newAxsisX;
    static bool newAxsisY;
    static bool newAxsisZ;

    [MenuItem("MyTools/RTToolsOptions")]
    public static void Init()
    {
        RTTools window = GetWindow<RTTools>();
        window.minSize = size;
        window.maxSize = size;
        window.Show();
    }
    private void OnGUI()
    {
        //������ ������ ���������
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        //zoom
        GUILayout.Label("Zoom (HotKeys: Z-, X+)");
        speedZoom = EditorGUILayout.FloatField("Speed Zoom:", speedZoom);

        //spliter
        GUILayout.Box("", GUILayout.Height(splitterWidth), GUILayout.Height(splitterWidth),
        GUILayout.Height(splitterWidth), GUILayout.ExpandWidth(true));

        //move obj ����������� ������� � ������� ������� (� ������� MenuTools)
        GUILayout.Label("Move Object (HotKeys: Ctrl+Arrows)");
        distance = EditorGUILayout.FloatField("Distance:", distance);
        //����������� �� ��������� �� �������� ���� 
        //�������� ��� ����
        newNoAxsis = EditorGUILayout.Toggle("noAxis", noAxis);
        newAxsisX = EditorGUILayout.Toggle("axsisX", axsisX);
        newAxsisY = EditorGUILayout.Toggle("axsisY", axsisY);
        newAxsisZ = EditorGUILayout.Toggle("axsisZ", axsisZ);
        // �������� ������� (����� ��������� ��������)
        if (newNoAxsis && !previousNoAxis)
        {
            noAxis = true;
            axsisX = false;
            axsisY = false;
            axsisZ = false;
        }
        else if (newAxsisX && !previousAxsisX)
        {
            axsisX = true;
            axsisY = false;
            axsisZ = false;
            noAxis = false;
        }
        else if (newAxsisY && !previousAxsisY)
        {
            axsisY = true;
            axsisX = false;
            axsisZ = false;
            noAxis = false;
        }
        else if (newAxsisZ && !previousAxsisZ)
        {
            axsisZ = true;
            axsisX = false;
            axsisY = false;
            noAxis = false;
        }
        // ���������� ���������� ���������
        previousNoAxis = newNoAxsis;
        previousAxsisX = newAxsisX;
        previousAxsisY = newAxsisY;
        previousAxsisZ = newAxsisZ;

        //spliter
        GUILayout.Box("",GUILayout.Height(splitterWidth), GUILayout.Height(splitterWidth), 
        GUILayout.Height(splitterWidth),GUILayout.ExpandWidth(true));

        //Quick change pivot point
        //GUILayout.Label("Quick change pivot point");
        GUILayout.Label("Quick change pivot point (HotKeys: Shift+Z - create pivot, Shift+X - delete pivot)");
        enablePivotPoint = EditorGUILayout.Toggle("Enable Tool", enablePivotPoint);

        //spliter
        GUILayout.Box("", GUILayout.Height(splitterWidth), GUILayout.Height(splitterWidth),
        GUILayout.Height(splitterWidth), GUILayout.ExpandWidth(true));

        //moveObjTo...
        GUILayout.Label("Move object To... (HotKeys: G-moving obj, H-move To obj)");
        enableRotation = EditorGUILayout.Toggle("Enable Rotation", enableRotation);

        //spliter
        GUILayout.Box("", GUILayout.Height(splitterWidth), GUILayout.Height(splitterWidth),
        GUILayout.Height(splitterWidth), GUILayout.ExpandWidth(true));

        //selection Obj with tag
        GUILayout.Label("Selection Object with tag");
        str = GUILayout.TextField(str);
        if (GUILayout.Button("find"))
        {
            enableFind = true;
        }

        //selection Obj with mesh filter name
        GUILayout.Label("Selection Object with mesh filter name");
        str2 = GUILayout.TextField(str2);
        if (GUILayout.Button("find"))
        {
            enableFindObjs = true;
        }

        //selection Obj with material name
        GUILayout.Label("Selection Object with mesh material name");
        str3 = GUILayout.TextField(str3);
        if (GUILayout.Button("find"))
        {
            enableFindMat = true;
        }

        //selection random child Objs 
        GUILayout.Label("Selection random child Objects ");
        if (GUILayout.Button("select"))
        {
            enableRandom = true;
        }

        //FindStyle
        //GUILayout.BeginHorizontal(GUI.skin.FindStyle("Toolbar"));
        //filter = GUILayout.TextField(filter, GUI.skin.FindStyle("ToolbarSeachTextField"));
        //if (GUILayout.Button("", GUI.skin.FindStyle("ToolbarSeachCancelButton")))
        //{
        //    filter = "";
        //    GUI.FocusControl(null);
        //}
        //GUILayout.EndHorizontal();

        //����� ������ ���������
        EditorGUILayout.EndScrollView();
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

            //������ �����
            //List<GameObject> objectsInScene = new List<GameObject>();
            //Object[] gos = FindObjectsOfTypeAll(typeof(GameObject));

            //foreach (GameObject go in gos as GameObject[])
            //{
            //    if (go.GetComponent<MeshFilter>())
            //    {
            //        if (go.GetComponent<MeshFilter>().sharedMesh.name == "Cube")
            //        {
            //            objectsInScene.Add(go);
            //            Selection.objects = objectsInScene.ToArray();
            //        }
            //    }
            //    enableFindObjs = false;
            //}
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

        //selection Obj with material name
        if (enableFindMat == true)
        {
            //���� ������ �� ���������� ��������
            //List<GameObject> objectsInScene = new List<GameObject>();
            //foreach (GameObject go in Selection.objects) 
            //{
            //    if (go.GetComponent<MeshRenderer>())
            //    {
            //        if (go.GetComponent<MeshRenderer>().sharedMaterial.name == str3/*go.GetComponent<MeshFilter>().sharedMesh == null*/)
            //        {
            //            objectsInScene.Add(go);
            //            Selection.objects = objectsInScene.ToArray();
            //        }
            //    }
            //    enableFindMat = false;
            //}

            //���� �� ����� ����������� �������
            List<GameObject> objectsInScene = new List<GameObject>();
            Transform selectedObjectTransform = Selection.activeTransform;
            int childCount = selectedObjectTransform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                Transform childTransform = selectedObjectTransform.GetChild(i);
                if (childTransform.GetComponent<MeshRenderer>() && childTransform.GetComponent<MeshRenderer>().sharedMaterial.name == str3)
                {
                    if (childTransform.GetComponent<MeshRenderer>().sharedMaterial.name == str3/*go.GetComponent<MeshFilter>().sharedMesh == null*/)
                    {
                        objectsInScene.Add(childTransform.gameObject);
                    }
                }
            }
            Selection.objects = objectsInScene.ToArray();
            enableFindMat = false;
        }

        //selection random child Objs 
        if (enableRandom == true)
        {
            List<GameObject> children = new List<GameObject>();
            Transform selectedObjectTransform = Selection.activeTransform;
            int childCount = selectedObjectTransform.childCount;

            for (int i = 0; i < childCount; i++)
            {
                Transform childTransform = selectedObjectTransform.GetChild(i);
                children.Add(childTransform.gameObject);
            }
            for (int i = 0; i < children.Count; i++)
            {
                int randomIndex = Random.Range(0, children.Count);
                GameObject randomChild = children[randomIndex];

                // ������� ��������� �������� ������ �� ������, ����� �� �� ��� ������ ��������
                children.RemoveAt(randomIndex);
            }
            Selection.objects = children.ToArray();
            enableRandom = false;
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

        ////view ��������� �� ���������� ������ � �����
        //Transform t;
        //t = Selection.activeTransform;
        //if (t != null)
        //{
        //    SceneView view = SceneView.lastActiveSceneView;
        //    view.LookAt(t.position);
        //}

        ////������ � ���������� Event e
        //Event e = Event.current;
        //if (e.type == EventType.KeyDown)
        //{
        //}


        //Quick change pivot point (�������� ���� Quick change pivot point (��������� �����������).
        //�������� ������ "�������� ����� �������� ��������� ������".
        //shift+z - ������� ������-����� � ������� ������� ����. ������ "�������� ����� �������� ��������� ������" ���������� ��������� � ������-�����
        //���������� "�������� ����� �������� ��������� ������" �� ����� �����
        //shift+z - ������ ��������� ���� ��������� �������-������ � ������������ �� ��� ����� � ��������. ������-����� ���������. ���� Quick change pivot point �����������.
        if (enablePivotPoint == true)
        {
            if (Selection.activeObject != null)
            {
                if (Tools.pivotMode == PivotMode.Pivot)
                {
                    Event e = Event.current;
                    Ray worldRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                    if (Event.current.Equals(Event.KeyboardEvent("#z"))) //shift+z
                    {
                        if (b == true)
                        {
                            if (Selection.activeTransform.parent != null)
                            {
                                // ��������� ������������� �������
                                objParent = Selection.activeTransform.parent;
                                obj = Selection.activeTransform;
                                i = obj.GetSiblingIndex();
                            }

                            if (Selection.activeTransform.parent == null)
                            {
                                // ��������� ������� ������� � ��������
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
                    if (Event.current.Equals(Event.KeyboardEvent("#x"))) //shift+x
                    {

                        obj.transform.parent = null;
                        GameObject _destroy = GameObject.Find("pivotPointObject");
                        DestroyImmediate(_destroy); //DestroyImmediate �������� ������� � editMode

                        //���������� ������ � ��� ����������� ����� � ��������
                        obj.SetSiblingIndex(i);  
                        if (objParent != null)
                        {
                            //���������� ������ � ��� ����������� parent
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


//����� ��� ���������� �������� � editMode (����� ������� ���� MyTools/Options)
//public class changePivotPoint : EditorWindow 
//{
//    private Vector2 _scenePosition;

//    [MenuItem("MyTools/Change Pivot Point")]
//    static void Init()
//    {
//        changePivotPoint window = GetWindow<changePivotPoint>();
//        window.Show();
//    }

//    private void OnEnable() { SceneView.duringSceneGui += SceneViewDuring; }

//    private void OnDisable() { SceneView.duringSceneGui -= SceneViewDuring; }

//    private void SceneViewDuring(SceneView scene)
//    {
//        var e = Event.current;
//        if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Space)
//        {
//            float pixelsPerPoint = EditorGUIUtility.pixelsPerPoint;
//            Vector2 mouse = e.mousePosition;
//            mouse.x *= pixelsPerPoint;
//            mouse.y = scene.camera.pixelHeight - mouse.y * pixelsPerPoint;
//            _scenePosition = scene.camera.ScreenToWorldPoint(mouse);

//            Repaint();
//        }
//    }
//    void OnGUI()
//    {
//        EditorGUILayout.LabelField("Scene: ", _scenePosition.ToString());
//    }
//}

