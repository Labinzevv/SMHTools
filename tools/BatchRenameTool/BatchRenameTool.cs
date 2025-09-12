using UnityEngine;
using UnityEditor;
//������� ��������/������� ������� � ����� ��������.
//��� ������������:
//��������� ������ � ����� Editor � ������� Unity (�������� Assets/Editor/BatchRenameTool.cs)
//� Unity ����� � ���� Tools - Batch Rename Tool.
public class BatchRenameTool : EditorWindow
{
    // ���� ��� ������ � ������
    bool findInHierarchy = false; //���� ��� ������ �� ��������
    string findText = "";
    string replaceText = "";

    // ���� ��� ���������� ������
    string addText = "";
    int insertPosition = 0; // ������� ��� ���������� ������

    [MenuItem("MyTools/Batch Rename Tool")]
    public static void ShowWindow()
    {
        GetWindow<BatchRenameTool>("Batch Rename");
    }

    void OnGUI()
    {
        GUILayout.Label("Batch Rename Tool", EditorStyles.boldLabel);

        // ���� ��� ������ �� ��������
        findInHierarchy = EditorGUILayout.Toggle("Find to Hierarchy", findInHierarchy);

        // ����� � ������
        GUILayout.Label("Find and change text:");
        findText = EditorGUILayout.TextField("Find:", findText);
        replaceText = EditorGUILayout.TextField("Change to:", replaceText);

        if (GUILayout.Button("Change"))
        {
            if (findInHierarchy)
                ReplaceTextInHierarchy();
            else
                ReplaceTextInSelectedObjects();
        }

        GUILayout.Space(10);

        // ���������� ������
        GUILayout.Label("Add text in name:");
        addText = EditorGUILayout.TextField("Text:", addText);
        insertPosition = EditorGUILayout.IntField("Position for paste (0 = start):", insertPosition);

        if (GUILayout.Button("Add text"))
        {
            AddTextToSelectedObjects();
        }
    }

    void ReplaceTextInSelectedObjects()
    {
        foreach (GameObject obj in Selection.gameObjects)
        {
            if (obj == null) continue;
            obj.name = obj.name.Replace(findText, replaceText);
        }
        Debug.Log("Change - ok");
    }

    //����� ��� ������������ ������ � ������ �� ��������
    void ReplaceTextInHierarchy()
    {
        foreach (GameObject rootObj in Selection.gameObjects)
        {
            if (rootObj == null) continue;
            ReplaceTextRecursive(rootObj);
        }
        Debug.Log("Change in hierarchy - ok");
    }

    // ����������� ������� ������ ������ � �����
    void ReplaceTextRecursive(GameObject obj)
    {
        obj.name = obj.name.Replace(findText, replaceText);

        foreach (Transform child in obj.transform)
        {
            ReplaceTextRecursive(child.gameObject); // ���������� ������� ���� ��������
        }
    }

    void AddTextToSelectedObjects()
    {
        foreach (GameObject obj in Selection.gameObjects)
        {
            if (obj == null) continue;
            string name = obj.name;
            int pos = Mathf.Clamp(insertPosition, 0, name.Length);
            obj.name = name.Insert(pos, addText);
        }
        Debug.Log("add text - ok.");
    }
}