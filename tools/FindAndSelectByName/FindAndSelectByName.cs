using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
//Скрипт для рекурсивного поиска и выделения объектов по заданному имени или части имени. 
//Работает так: Открываешь окно скрипта MyTools/Find And Select By Name.
//В поле Select Object sWit hName вписываешь полное имя или часть имени объектов, которые нужно найти. 
//Выделяешь родительский объект, в котором будет совершаться поиск. жмешь кнопку Find & Select.
//Выделяются все дочерние объекты с указанным полным именем или частью имени.
public class FindAndSelectByName : EditorWindow
{
    private string searchName = "";

    [MenuItem("MyTools/Find And Select By Name")]
    public static void ShowWindow()
    {
        GetWindow<FindAndSelectByName>("Find & Select");
    }

    private void OnGUI()
    {
        GUILayout.Label("Find and Select Child Objects", EditorStyles.boldLabel);

        // Поле для ввода имени/части имени
        searchName = EditorGUILayout.TextField("Select Object sWit hName", searchName);

        EditorGUILayout.Space();

        if (GUILayout.Button("Find And Select"))
        {
            FindAndSelect();
        }
    }

    private void FindAndSelect()
    {
        if (Selection.activeTransform == null)
        {
            EditorUtility.DisplayDialog("Error", "Please select a parent object in the Hierarchy.", "OK");
            return;
        }

        Transform parent = Selection.activeTransform;
        List<GameObject> foundObjects = new List<GameObject>();

        // Рекурсивный обход
        FindMatchingChildrenRecursive(parent, searchName, foundObjects);

        if (foundObjects.Count > 0)
        {
            Selection.objects = foundObjects.ToArray();
            Debug.Log($"Found and selected {foundObjects.Count} objects matching \"{searchName}\" under \"{parent.name}\".");
        }
        else
        {
            EditorUtility.DisplayDialog("No Results", $"No child objects found matching \"{searchName}\".", "OK");
        }
    }

    private void FindMatchingChildrenRecursive(Transform parent, string search, List<GameObject> results)
    {
        foreach (Transform child in parent)
        {
            if (child.name.Contains(search)) // частичное совпадение
            {
                results.Add(child.gameObject);
            }

            // рекурсия вглубь
            FindMatchingChildrenRecursive(child, search, results);
        }
    }
}
