using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MapGenerator))]
public class MapEditor : Editor
{
    Editor _settingsEditor;

    private bool _showSettings = true;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        MapGenerator map = target as MapGenerator;

        DrawSettingsEditor(map);

        if (GUILayout.Button("Generate Map"))
        {
            map.GenerateMap();
        }
    }

    private void DrawSettingsEditor(MapGenerator map)
    {
        if (map.MapSettings == null) return;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Fast Settings Editing", EditorStyles.boldLabel);

        CreateCachedEditor(map.MapSettings, null, ref _settingsEditor);

        EditorGUI.BeginChangeCheck();

        _showSettings = EditorGUILayout.Foldout(_showSettings, "Map Settings", true);
        if (_showSettings)
        {
            _settingsEditor.OnInspectorGUI();
        }

        if (EditorGUI.EndChangeCheck())
        {
            map.GenerateMap();
        }
    }

    private void OnDisable()
    {
        if (_settingsEditor != null)
        {
            DestroyImmediate(_settingsEditor);
        }
    }
}
