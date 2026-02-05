/*
 * ChangeHierarchy
 *
 * Editor-инструмент для Unity (работает только в редакторе).
 *
 * Предназначен для управления порядком объектов в Hierarchy
 * у родительского GameObject (пустышки).
 *
 * Поддерживаемые режимы работы:
 *
 * 1) Reverse order
 *    Разворачивает порядок дочерних объектов родителя целиком.
 *    Пример:
 *      Было:  A, B, C, D
 *      Стало: D, C, B, A
 *
 * 2) Reverse only selected
 *    Разворачивает порядок ТОЛЬКО выбранных объектов,
 *    при условии, что они являются соседями (siblings)
 *    и имеют одного и того же родителя.
 *    Остальные объекты в иерархии не затрагиваются.
 *
 * 3) Sort by numeric suffix in object names
 *    Сортирует объекты по числовому суффиксу в имени объекта
 *    в порядке возрастания.
 *
 *    Суффикс — это:
 *    • один или более символов,
 *    • расположенных ПОСЛЕ последнего символа '_' (underscore)
 *      в имени объекта.
 *
 *    Примеры:
 *      platformCap_1           суффикс: 1
 *      platformCap_12          суффикс: 12
 *      mesh_LOD_3              суффикс: 3
 *      some_object_version_42  суффикс: 42
 *
 *    Примеры сортировки:
 *      Было: 2, 6, 4, 9, 1, 5
 *      Стало: 1, 2, 4, 5, 6, 9
 *
 *      Было: 6, 4, 2, 1
 *      Стало: 1, 2, 4, 6
 *
 *    Объекты без корректного числового суффикса
 *    (отсутствует '_' или суффикс не является числом)
 *    автоматически перемещаются в конец списка.
 *
 * Меню:
 *    MyTools / Change Hierarchy
 *
 * Инструмент хорошо работает в связке с RenameChildrenEditor,
 * позволяя сначала корректно переименовать объекты,
 * а затем автоматически отсортировать их в иерархии.
 */

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

public class ChangeHierarchy : EditorWindow
{
    bool applyOnlyToSelected = false;
    bool sortWithSuffixName = false;

    [MenuItem("MyTools/Change Hierarchy")]
    static void OpenWindow()
    {
        GetWindow<ChangeHierarchy>("Change Hierarchy");
    }

    void OnGUI()
    {
        GUILayout.Label("Change Hierarchy", EditorStyles.boldLabel);
        GUILayout.Space(5);

        applyOnlyToSelected = EditorGUILayout.Toggle(
            "Применить к выделенным объектам",
            applyOnlyToSelected
        );

        sortWithSuffixName = EditorGUILayout.Toggle(
            "Sort With Suffix Name",
            sortWithSuffixName
        );

        GUILayout.Space(10);

        if (GUILayout.Button("Apply"))
        {
            Apply();
        }
    }

    void Apply()
    {
        if (!applyOnlyToSelected)
        {
            //ВСЕ ДЕТИ PARENT
            GameObject parent = Selection.activeGameObject;

            if (parent == null)
            {
                Debug.LogError("ChangeHierarchy: выбери parent объект.");
                return;
            }

            Transform t = parent.transform;

            if (t.childCount < 2)
            {
                Debug.LogWarning("ChangeHierarchy: у parent меньше 2 детей.");
                return;
            }

            Undo.RegisterFullObjectHierarchyUndo(parent, "Change Hierarchy");

            List<Transform> children = GetChildrenList(t);

            ApplyOrder(children, t);
            EditorUtility.SetDirty(parent);
        }
        else
        {
            //ТОЛЬКО ВЫДЕЛЕННЫЕ
            Transform[] selected = Selection.transforms;

            if (selected.Length < 2)
            {
                Debug.LogWarning("ChangeHierarchy: выбери минимум 2 объекта.");
                return;
            }

            Transform parent = selected[0].parent;

            if (parent == null)
            {
                Debug.LogError("ChangeHierarchy: у объектов нет parent.");
                return;
            }

            if (selected.Any(t => t.parent != parent))
            {
                Debug.LogError("ChangeHierarchy: все объекты должны иметь одного parent.");
                return;
            }

            Undo.RegisterFullObjectHierarchyUndo(parent.gameObject, "Change Hierarchy");

            List<Transform> selectedOrdered = selected
                .OrderBy(t => t.GetSiblingIndex())
                .ToList();

            ApplyOrder(selectedOrdered, parent);
            EditorUtility.SetDirty(parent.gameObject);
        }

        Debug.Log("ChangeHierarchy: операция выполнена.");
    }

    //ОСНОВНАЯ ЛОГИКА

    void ApplyOrder(List<Transform> targets, Transform parent)
    {
        List<Transform> result;

        if (sortWithSuffixName)
        {
            result = targets
                .OrderBy(t => ExtractNumericSuffix(t.name))
                .ToList();
        }
        else
        {
            result = targets.AsEnumerable().Reverse().ToList();
        }

        int startIndex = targets[0].GetSiblingIndex();

        for (int i = 0; i < result.Count; i++)
        {
            result[i].SetSiblingIndex(startIndex + i);
        }
    }

    List<Transform> GetChildrenList(Transform parent)
    {
        List<Transform> list = new List<Transform>();
        for (int i = 0; i < parent.childCount; i++)
            list.Add(parent.GetChild(i));
        return list;
    }

    //ИЗВЛЕЧЕНИЕ СУФФИКСА

    int ExtractNumericSuffix(string name)
    {
        int underscoreIndex = name.LastIndexOf('_');

        if (underscoreIndex == -1 || underscoreIndex == name.Length - 1)
            return int.MaxValue;

        string suffix = name.Substring(underscoreIndex + 1);

        if (int.TryParse(suffix, out int value))
            return value;

        return int.MaxValue;
    }
}