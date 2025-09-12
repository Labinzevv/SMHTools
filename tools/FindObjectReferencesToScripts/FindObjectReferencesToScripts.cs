using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

// FindObjectToScripts � ���������� ��������� Unity.
// ��� ������:
// 1. ������������ ������ ��� ������� � �����.
// 2. ������ ������� ��� ���� �� ���� �������� �� �����, ��� ����������� ���� ������ (����� ������ �� GameObject/Component).
// 3. ���������� ������� �� 2 ���������:
//    - "����������� �������" (���� ������ ����� �� ����� �������, �� ����� �������� ����).
//    - "��������� �������" (���� ������ �������� � �������� ������ ��������).
// 4. ��������� ������� ��������� � ������� � ������������ �������� � ���������� ������ � ��������.
// 5. ������ ����� ��������� ������ ��������� � ������ ���������/�����������.
// 6. ���� ������ ����������� ��������� � �����, ����� ��������� ������ ���� ��������� ������.
public class FindObjectReferencesToScripts : EditorWindow
{
    private string nameObject = ""; // ��� �������, �� �������� ����� �����

    // �����-���������: ������ ������ � ������-�������� (Object -> Script -> Field)
    private class Result
    {
        public GameObject gameObject;
        public string description;
    }

    // ������ �����������
    private List<Result> ownScripts = new List<Result>();          // ����������� �������
    private List<Result> externalReferences = new List<Result>();  // ��������� �������

    // ������� � ��������� ������������
    private Vector2 scrollExternal;
    private Vector2 scrollOwn;
    private bool foldExternal = true;
    private bool foldOwn = true;

    [MenuItem("MyTools/FindObjectReferencesToScripts")]
    public static void ShowWindow()
    {
        GetWindow<FindObjectReferencesToScripts>("FindObjectReferencesToScripts");
    }

    // �������� ����� ��������� ���������� ����
    private void OnGUI()
    {
        GUILayout.Label("����� ���������� ������� � ����� ��������", EditorStyles.boldLabel);

        // ���� ��� ����� ����� �������
        nameObject = EditorGUILayout.TextField("��� �������:", nameObject);

        // ������ ������� ������
        if (GUILayout.Button("�����"))
        {
            FindReferences();
        }

        GUILayout.Space(10);

        // ������ ����������� � �����
        if (GUILayout.Button("���������� ��������� � �����"))
        {
            CopyResultsToClipboard();
        }

        GUILayout.Space(10);

        //��������� ������� �� ���������, � ������� ��������� ��� ������� ������
        foldExternal = EditorGUILayout.Foldout
            (foldExternal,
            $"��������� ������� �� ���������, � ������� ��������� ��� ������� ������: {externalReferences.Count}",
            true);

        if (foldExternal)
        {
            scrollExternal = EditorGUILayout.BeginScrollView(scrollExternal, GUILayout.Height(200)); // ~10 �����
            if (externalReferences.Count > 0)
            {
                foreach (var entry in externalReferences)
                {
                    if (GUILayout.Button(entry.description, EditorStyles.helpBox))
                    {
                        Selection.activeObject = entry.gameObject;
                        EditorGUIUtility.PingObject(entry.gameObject);
                    }
                }
            }
            else
            {
                EditorGUILayout.LabelField("��� ��������� ������.", EditorStyles.miniLabel);
            }
            EditorGUILayout.EndScrollView();
        }

        GUILayout.Space(10);

        //����������� ������� ������� ������, � ������� ��������� ��� ������� ������
        foldOwn = EditorGUILayout.Foldout
            (foldOwn,
            $"����������� ������� ������� ������, � ������� ��������� ��� ������� ������: {ownScripts.Count}",
            true);

        if (foldOwn)
        {
            scrollOwn = EditorGUILayout.BeginScrollView(scrollOwn, GUILayout.Height(200)); // ~10 �����
            if (ownScripts.Count > 0)
            {
                foreach (var entry in ownScripts)
                {
                    if (GUILayout.Button(entry.description, EditorStyles.helpBox))
                    {
                        Selection.activeObject = entry.gameObject;
                        EditorGUIUtility.PingObject(entry.gameObject);
                    }
                }
            }
            else
            {
                EditorGUILayout.LabelField("��� ����������� ������.", EditorStyles.miniLabel);
            }
            EditorGUILayout.EndScrollView();
        }
    }

    // ����� ������: �������� �� ���� �������� ����� � ���� ����,
    // ��� ���� ������ �� ������ � ��������� ������.
    private void FindReferences()
    {
        ownScripts.Clear();
        externalReferences.Clear();

        if (string.IsNullOrEmpty(nameObject))
        {
            Debug.LogWarning("������� ��� ������� ��� ������!");
            return;
        }

        // ������� ������� ������ �� �����
        GameObject target = GameObject.Find(nameObject);
        if (target == null)
        {
            Debug.LogWarning($"������ � ������ '{nameObject}' �� ������ � �����!");
            return;
        }

        // �������� ��� ������� � ����� (������� �����)
        GameObject[] allObjects = GetAllSceneObjects();

        foreach (GameObject obj in allObjects)
        {
            // ����� ��� ������� �� �������
            MonoBehaviour[] scripts = obj.GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour script in scripts)
            {
                if (script == null) continue;

                // ����� ��� ���� �������
                FieldInfo[] fields = script.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                foreach (FieldInfo field in fields)
                {
                    if (typeof(Object).IsAssignableFrom(field.FieldType))
                    {
                        Object value = field.GetValue(script) as Object;
                        if (value != null)
                        {
                            // ���������: ��� ������� ������ ��� ��� ���������
                            if (value == target || (value is Component comp && comp.gameObject == target))
                            {
                                string description = $"{obj.name} -> {script.GetType().Name} -> {field.Name}";
                                var result = new Result { gameObject = obj, description = description };

                                // ���� ������ � ������� �� ����� ������� � ��� "�����������" ������
                                if (obj == target)
                                    ownScripts.Add(result);
                                else
                                    externalReferences.Add(result);
                            }
                        }
                    }
                }
            }
        }

        Debug.Log(
            $"����� �������� ��� '{nameObject}': " +
            $"{externalReferences.Count} ��������� ������, " +
            $"{ownScripts.Count} �����������."
        );
    }

    // �������� ��������� ������ � ����� ������
    private void CopyResultsToClipboard()
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine($"���������� ������ ��� �������: {nameObject}");
        sb.AppendLine();

        sb.AppendLine("��������� �������:");
        if (externalReferences.Count > 0)
        {
            foreach (var entry in externalReferences)
                sb.AppendLine(entry.description);
        }
        else
        {
            sb.AppendLine("��� ��������� ������.");
        }
        sb.AppendLine();

        sb.AppendLine("����������� �������:");
        if (ownScripts.Count > 0)
        {
            foreach (var entry in ownScripts)
                sb.AppendLine(entry.description);
        }
        else
        {
            sb.AppendLine("��� ����������� ������.");
        }

        EditorGUIUtility.systemCopyBuffer = sb.ToString();
        Debug.Log("��������� ������ ���������� � ����� ������!");
    }

    // �������� ��� ������� ������� ����� (������� �����).
    private GameObject[] GetAllSceneObjects()
    {
        List<GameObject> objects = new List<GameObject>();
        foreach (GameObject root in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
        {
            CollectChildren(root, objects);
        }
        return objects.ToArray();
    }

    // ���������� ��������� ������ � ���� ��� ����� � ������.
    private void CollectChildren(GameObject obj, List<GameObject> list)
    {
        list.Add(obj);
        foreach (Transform child in obj.transform)
        {
            CollectChildren(child.gameObject, list);
        }
    }
}
