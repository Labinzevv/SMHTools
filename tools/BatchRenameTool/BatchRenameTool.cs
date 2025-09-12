using UnityEngine;
using UnityEditor;
//массово дабавить/удалить символы в иенах объектов.
//Как пользоваться:
//Сохранить скрипт в папку Editor в проекте Unity (например Assets/Editor/BatchRenameTool.cs)
//В Unity зайти в меню Tools - Batch Rename Tool.
public class BatchRenameTool : EditorWindow
{
    // Поля для поиска и замены
    bool findInHierarchy = false; //Тогл для поиска по иерархии
    string findText = "";
    string replaceText = "";

    // Поля для добавления текста
    string addText = "";
    int insertPosition = 0; // позиция для добавления текста

    [MenuItem("MyTools/Batch Rename Tool")]
    public static void ShowWindow()
    {
        GetWindow<BatchRenameTool>("Batch Rename");
    }

    void OnGUI()
    {
        GUILayout.Label("Batch Rename Tool", EditorStyles.boldLabel);

        // Тогл для поиска по иерархии
        findInHierarchy = EditorGUILayout.Toggle("Find to Hierarchy", findInHierarchy);

        // Поиск и замена
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

        // Добавление текста
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

    //Метод для рекурсивного поиска и замены по иерархии
    void ReplaceTextInHierarchy()
    {
        foreach (GameObject rootObj in Selection.gameObjects)
        {
            if (rootObj == null) continue;
            ReplaceTextRecursive(rootObj);
        }
        Debug.Log("Change in hierarchy - ok");
    }

    // Рекурсивная функция замены текста в имени
    void ReplaceTextRecursive(GameObject obj)
    {
        obj.name = obj.name.Replace(findText, replaceText);

        foreach (Transform child in obj.transform)
        {
            ReplaceTextRecursive(child.gameObject); // рекурсивно обходим всех потомков
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