using UnityEngine;
using UnityEditor;

// Material Property Debugger — инструмент редактора.
// Что делает:
// 1. Позволяет выбрать материал прямо в окне.
// 2. Показывает все свойства материала (имя, тип, описание).
// 3. Отображает их в удобном скроллируемом списке.
// 4. Работает только в Unity Editor, в билд не попадает.

public class MaterialPropertyDebugger : EditorWindow
{
    private Material materialToInspect;
    private Vector2 scrollPos;

    [MenuItem("MyTools/Material Property Debugger")]
    public static void ShowWindow()
    {
        GetWindow<MaterialPropertyDebugger>("Material Debugger");
    }

    private void OnGUI()
    {
        GUILayout.Label("Проверка свойств материала", EditorStyles.boldLabel);

        // Поле для выбора материала
        materialToInspect = (Material)EditorGUILayout.ObjectField(
            "Материал:", materialToInspect, typeof(Material), false);

        GUILayout.Space(10);

        if (materialToInspect == null)
        {
            EditorGUILayout.HelpBox("Выберите материал для проверки.", MessageType.Info);
            return;
        }

        // Кнопка обновления (по желанию)
        if (GUILayout.Button("Обновить список свойств"))
        {
            Repaint();
        }

        GUILayout.Space(10);

        // Отображаем свойства материала
        Shader shader = materialToInspect.shader;
        int count = ShaderUtil.GetPropertyCount(shader);

        EditorGUILayout.LabelField(
            $"Свойства материала '{materialToInspect.name}' (Shader: {shader.name})",
            EditorStyles.boldLabel);

        GUILayout.Space(5);

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(300));

        for (int i = 0; i < count; i++)
        {
            string propName = ShaderUtil.GetPropertyName(shader, i);
            ShaderUtil.ShaderPropertyType propType = ShaderUtil.GetPropertyType(shader, i);
            string propDesc = ShaderUtil.GetPropertyDescription(shader, i);

            EditorGUILayout.LabelField($"{i}: {propName}", $"{propType} — {propDesc}");
        }

        EditorGUILayout.EndScrollView();
    }
}

