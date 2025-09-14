using UnityEngine;
using UnityEditor;

// Material Property Debugger � ���������� ���������.
// ��� ������:
// 1. ��������� ������� �������� ����� � ����.
// 2. ���������� ��� �������� ��������� (���, ���, ��������).
// 3. ���������� �� � ������� ������������� ������.
// 4. �������� ������ � Unity Editor, � ���� �� ��������.

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
        GUILayout.Label("�������� ������� ���������", EditorStyles.boldLabel);

        // ���� ��� ������ ���������
        materialToInspect = (Material)EditorGUILayout.ObjectField(
            "��������:", materialToInspect, typeof(Material), false);

        GUILayout.Space(10);

        if (materialToInspect == null)
        {
            EditorGUILayout.HelpBox("�������� �������� ��� ��������.", MessageType.Info);
            return;
        }

        // ������ ���������� (�� �������)
        if (GUILayout.Button("�������� ������ �������"))
        {
            Repaint();
        }

        GUILayout.Space(10);

        // ���������� �������� ���������
        Shader shader = materialToInspect.shader;
        int count = ShaderUtil.GetPropertyCount(shader);

        EditorGUILayout.LabelField(
            $"�������� ��������� '{materialToInspect.name}' (Shader: {shader.name})",
            EditorStyles.boldLabel);

        GUILayout.Space(5);

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(300));

        for (int i = 0; i < count; i++)
        {
            string propName = ShaderUtil.GetPropertyName(shader, i);
            ShaderUtil.ShaderPropertyType propType = ShaderUtil.GetPropertyType(shader, i);
            string propDesc = ShaderUtil.GetPropertyDescription(shader, i);

            EditorGUILayout.LabelField($"{i}: {propName}", $"{propType} � {propDesc}");
        }

        EditorGUILayout.EndScrollView();
    }
}

