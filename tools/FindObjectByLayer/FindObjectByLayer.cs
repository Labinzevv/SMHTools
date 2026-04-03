using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
/// <summary>
/// FindObjectByLayer — редакторский инструмент для поиска объектов в текущей сцене по указанному слою.
/// 
/// Логика работы:
/// 1. Пользователь вводит имя слоя в окне инструмента.
/// 2. При нажатии кнопки "Найти":
///    - Проверяется существование слоя.
///    - Если слой не существует — выводится сообщение в консоль.
///    - Если существует — выполняется поиск всех объектов (включая неактивные) с этим слоем.
/// 3. Найденные объекты:
///    - Добавляются в список в окне инструмента.
///    - Выделяются в Hierarchy.
/// 4. Если объекты не найдены — выводится сообщение в консоль.
/// </summary>
public class FindObjectByLayer : EditorWindow
{
    private string layerName = "";
    private Vector2 scroll;

    private List<GameObject> foundObjects = new List<GameObject>();

    [MenuItem("MyTools/Find Object By Layer")]
    public static void ShowWindow()
    {
        GetWindow<FindObjectByLayer>("FindObjectByLayer");
    }

    private void OnGUI()
    {
        GUILayout.Label("Find Object By Layer", EditorStyles.boldLabel);

        EditorGUILayout.Space();

        layerName = EditorGUILayout.TextField("Specify layer", layerName);

        if (GUILayout.Button("Find"))
        {
            FindObjects();
        }

        EditorGUILayout.Space();
        GUILayout.Label("Found objects:", EditorStyles.boldLabel);

        scroll = EditorGUILayout.BeginScrollView(scroll);

        foreach (var obj in foundObjects)
        {
            if (obj != null)
            {
                EditorGUILayout.ObjectField(obj, typeof(GameObject), true);
            }
        }

        EditorGUILayout.EndScrollView();
    }

    private void FindObjects()
    {
        foundObjects.Clear();

        int layer = LayerMask.NameToLayer(layerName);

        //слоя не существует
        if (layer == -1)
        {
            Debug.LogWarning($"Layer \"{layerName}\" does not exist");
            return;
        }

        //Поиск объектов (включая неактивные объекты)
        GameObject[] allObjects = FindObjectsByType<GameObject>( FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (var obj in allObjects)
        {
            if (obj.layer == layer)
            {
                foundObjects.Add(obj);
            }
        }

        //объектов нет
        if (foundObjects.Count == 0)
        {
            Debug.LogWarning($"No objects with layer \"{layerName}\" found in the scene");
            return;
        }

        //выделяем в иерархии
        Selection.objects = foundObjects.ToArray();

        Debug.Log($"Objects found: {foundObjects.Count}");
    }
}