using UnityEngine;
using UnityEditor;
using System.Linq;
// Для поиска скриптов в сцене по объектам (открыть окно в меню tools выбрать скрипт и положить его в поле окна. После этого нажать кнопку поиска и 
// в консоли появится сообщение с сылкой на нужный объект.)
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
        GUILayout.Label("Выберите скрипт для поиска", EditorStyles.boldLabel);
        scriptToFind = (MonoScript)EditorGUILayout.ObjectField("Скрипт:", scriptToFind, typeof(MonoScript), false);

        if (GUILayout.Button("Найти объекты в сцене") && scriptToFind != null)
        {
            FindObjects();
        }
    }

    private void FindObjects()
    {
        if (scriptToFind == null)
        {
            Debug.LogError("Скрипт не выбран!");
            return;
        }

        string scriptName = scriptToFind.name;
        Debug.Log("Ищу объекты с компонентом: " + scriptName);

        var foundObjects = FindObjectsByType<Component>(FindObjectsSortMode.None)
            .Where(c => c.GetType().Name == scriptName)
            .Select(c => c.gameObject)
            .Distinct()
            .ToList();

        if (foundObjects.Count == 0)
        {
            Debug.LogWarning("Объекты с этим скриптом не найдены.");
        }
        else
        {
            foreach (var obj in foundObjects)
            {
                Debug.Log("Найден объект: " + obj.name, obj);
            }
        }
    }
}
