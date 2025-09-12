using UnityEngine;
using UnityEditor;
using System.Linq;
// ��� ������ �������� � ����� �� �������� (������� ���� � ���� tools ������� ������ � �������� ��� � ���� ����. ����� ����� ������ ������ ������ � 
// � ������� �������� ��������� � ������ �� ������ ������.)
public class FindObjectsWithScript : EditorWindow
{
    private MonoScript scriptToFind;

    [MenuItem("MyTools/Find Objects With Script")]
    public static void ShowWindow()
    {
        GetWindow(typeof(FindObjectsWithScript));
    }

    private void OnGUI()
    {
        GUILayout.Label("�������� ������ ��� ������", EditorStyles.boldLabel);
        scriptToFind = (MonoScript)EditorGUILayout.ObjectField("������:", scriptToFind, typeof(MonoScript), false);

        if (GUILayout.Button("����� ������� � �����") && scriptToFind != null)
        {
            FindObjects();
        }
    }

    private void FindObjects()
    {
        if (scriptToFind == null)
        {
            Debug.LogError("������ �� ������!");
            return;
        }

        string scriptName = scriptToFind.name;
        Debug.Log("��� ������� � �����������: " + scriptName);

        var foundObjects = FindObjectsByType<Component>(FindObjectsSortMode.None)
            .Where(c => c.GetType().Name == scriptName)
            .Select(c => c.gameObject)
            .Distinct()
            .ToList();

        if (foundObjects.Count == 0)
        {
            Debug.LogWarning("������� � ���� �������� �� �������.");
        }
        else
        {
            foreach (var obj in foundObjects)
            {
                Debug.Log("������ ������: " + obj.name, obj);
            }
        }
    }
}
