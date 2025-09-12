using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

// FindObjectToScripts — инструмент редактора Unity.
// Что делает:
// 1. Пользователь вводит имя объекта в сцене.
// 2. Скрипт находит все поля во всех скриптах на сцене, где упоминается этот объект (через ссылки на GameObject/Component).
// 3. Результаты делятся на 2 категории:
//    - "Собственные скрипты" (если скрипт висит на самом объекте, по имени которого ищем).
//    - "Сторонние объекты" (если объект упомянут в скриптах других объектов).
// 4. Найденные объекты выводятся в списках с возможностью кликнуть и подсветить объект в иерархии.
// 5. Списки имеют отдельные полосы прокрутки и кнопки «Свернуть/Развернуть».
// 6. Есть кнопка «Копировать результат в буфер», чтобы сохранить список всех найденных ссылок.
public class FindObjectReferencesToScripts : EditorWindow
{
    private string nameObject = ""; // Имя объекта, по которому ведем поиск

    // Класс-результат: хранит объект и строку-описание (Object -> Script -> Field)
    private class Result
    {
        public GameObject gameObject;
        public string description;
    }

    // Списки результатов
    private List<Result> ownScripts = new List<Result>();          // Собственные скрипты
    private List<Result> externalReferences = new List<Result>();  // Сторонние объекты

    // Скроллы и состояния сворачивания
    private Vector2 scrollExternal;
    private Vector2 scrollOwn;
    private bool foldExternal = true;
    private bool foldOwn = true;

    [MenuItem("MyTools/FindObjectReferencesToScripts")]
    public static void ShowWindow()
    {
        GetWindow<FindObjectReferencesToScripts>("FindObjectReferencesToScripts");
    }

    // Основной метод отрисовки интерфейса окна
    private void OnGUI()
    {
        GUILayout.Label("Поиск упоминаний объекта в полях скриптов", EditorStyles.boldLabel);

        // Поле для ввода имени объекта
        nameObject = EditorGUILayout.TextField("Имя объекта:", nameObject);

        // Кнопка запуска поиска
        if (GUILayout.Button("Найти"))
        {
            FindReferences();
        }

        GUILayout.Space(10);

        // Кнопка копирования в буфер
        if (GUILayout.Button("Копировать результат в буфер"))
        {
            CopyResultsToClipboard();
        }

        GUILayout.Space(10);

        //СТОРОННИЕ ОБЪЕКТЫ СО СКРИПТАМИ, В КОТОРЫЕ ДОБАВЛЕНО ИМЯ ОБЪЕКТА ПОИСКА
        foldExternal = EditorGUILayout.Foldout
            (foldExternal,
            $"Сторонние объекты со скриптами, в которые добавлено имя объекта поиска: {externalReferences.Count}",
            true);

        if (foldExternal)
        {
            scrollExternal = EditorGUILayout.BeginScrollView(scrollExternal, GUILayout.Height(200)); // ~10 строк
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
                EditorGUILayout.LabelField("Нет сторонних ссылок.", EditorStyles.miniLabel);
            }
            EditorGUILayout.EndScrollView();
        }

        GUILayout.Space(10);

        //СОБСТВЕННЫЕ СКРИПТЫ ОБЪЕКТА ПОИСКА, В КОТОРЫЕ ДОБАВЛЕНО ИМЯ ОБЪЕКТА ПОИСКА
        foldOwn = EditorGUILayout.Foldout
            (foldOwn,
            $"Собственные скрипты объекта поиска, в которые добавлено имя объекта поиска: {ownScripts.Count}",
            true);

        if (foldOwn)
        {
            scrollOwn = EditorGUILayout.BeginScrollView(scrollOwn, GUILayout.Height(200)); // ~10 строк
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
                EditorGUILayout.LabelField("Нет собственных ссылок.", EditorStyles.miniLabel);
            }
            EditorGUILayout.EndScrollView();
        }
    }

    // Метод поиска: проходит по всем объектам сцены и ищет поля,
    // где есть ссылка на объект с введенным именем.
    private void FindReferences()
    {
        ownScripts.Clear();
        externalReferences.Clear();

        if (string.IsNullOrEmpty(nameObject))
        {
            Debug.LogWarning("Введите имя объекта для поиска!");
            return;
        }

        // Находим целевой объект по имени
        GameObject target = GameObject.Find(nameObject);
        if (target == null)
        {
            Debug.LogWarning($"Объект с именем '{nameObject}' не найден в сцене!");
            return;
        }

        // Получаем все объекты в сцене (включая детей)
        GameObject[] allObjects = GetAllSceneObjects();

        foreach (GameObject obj in allObjects)
        {
            // Берем все скрипты на объекте
            MonoBehaviour[] scripts = obj.GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour script in scripts)
            {
                if (script == null) continue;

                // Берем все поля скрипта
                FieldInfo[] fields = script.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                foreach (FieldInfo field in fields)
                {
                    if (typeof(Object).IsAssignableFrom(field.FieldType))
                    {
                        Object value = field.GetValue(script) as Object;
                        if (value != null)
                        {
                            // Проверяем: это целевой объект или его компонент
                            if (value == target || (value is Component comp && comp.gameObject == target))
                            {
                                string description = $"{obj.name} -> {script.GetType().Name} -> {field.Name}";
                                var result = new Result { gameObject = obj, description = description };

                                // Если ссылка в скрипте на самом объекте — это "собственный" скрипт
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
            $"Поиск завершён для '{nameObject}': " +
            $"{externalReferences.Count} сторонних ссылок, " +
            $"{ownScripts.Count} собственных."
        );
    }

    // Копирует результат поиска в буфер обмена
    private void CopyResultsToClipboard()
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine($"Результаты поиска для объекта: {nameObject}");
        sb.AppendLine();

        sb.AppendLine("Сторонние объекты:");
        if (externalReferences.Count > 0)
        {
            foreach (var entry in externalReferences)
                sb.AppendLine(entry.description);
        }
        else
        {
            sb.AppendLine("Нет сторонних ссылок.");
        }
        sb.AppendLine();

        sb.AppendLine("Собственные скрипты:");
        if (ownScripts.Count > 0)
        {
            foreach (var entry in ownScripts)
                sb.AppendLine(entry.description);
        }
        else
        {
            sb.AppendLine("Нет собственных ссылок.");
        }

        EditorGUIUtility.systemCopyBuffer = sb.ToString();
        Debug.Log("Результат поиска скопирован в буфер обмена!");
    }

    // Получает все объекты текущей сцены (включая детей).
    private GameObject[] GetAllSceneObjects()
    {
        List<GameObject> objects = new List<GameObject>();
        foreach (GameObject root in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
        {
            CollectChildren(root, objects);
        }
        return objects.ToArray();
    }

    // Рекурсивно добавляет объект и всех его детей в список.
    private void CollectChildren(GameObject obj, List<GameObject> list)
    {
        list.Add(obj);
        foreach (Transform child in obj.transform)
        {
            CollectChildren(child.gameObject, list);
        }
    }
}
