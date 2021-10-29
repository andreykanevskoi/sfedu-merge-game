using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LevelBuilder : EditorWindow {
    FieldManager _fieldManager;

    [MenuItem("Window/Level Builder")]
    public static void ShowWindow() {
        GetWindow<LevelBuilder>("Level Builder");
    }

    private void HandleKeyboard() {
        Event current = Event.current;
        if (current.type != EventType.KeyDown)
            return;

        switch (current.keyCode) {
            case KeyCode.DownArrow:
                ChangeTargetsPosition(y: -1);
                current.Use();
                break;
            case KeyCode.UpArrow:
                ChangeTargetsPosition(y: 1);
                current.Use();
                break;
            case KeyCode.LeftArrow:
                ChangeTargetsPosition(x: -1);
                break;
            case KeyCode.RightArrow:
                ChangeTargetsPosition(x: 1);
                break;
            case KeyCode.PageUp:
                ChangeTargetsPosition(z: 1);
                break;
            case KeyCode.PageDown:
                ChangeTargetsPosition(z: -1);
                break;
        }
    }

    private void ChangeTargetsPosition(int x = 0, int y = 0, int z = 0) {
        foreach (GameObject obj in Selection.gameObjects) {
            Placeable placeable = obj.GetComponent<Placeable>();
            if (!placeable) return;

            Vector3Int position = placeable.currentCell;
            Vector3Int newPosition = new Vector3Int(position.x + x, position.y + y, position.z + z);

            var so = new SerializedObject(placeable);
            so.FindProperty("currentCell").vector3IntValue = newPosition;
            so.ApplyModifiedProperties();

            placeable.Position = _fieldManager.GetCellWorldPosition(newPosition);
        }
    }

    private void Ping() {
        Dictionary<Vector3Int, Placeable> positions = new Dictionary<Vector3Int, Placeable>();

        Placeable[] placeables = FindObjectsOfType<Placeable>();
        if (placeables.Length == 0) return;

        foreach (Placeable placeable in placeables) {
            if (!_fieldManager.HasTile(placeable.currentCell) || positions.ContainsKey(placeable.currentCell)) {
                EditorGUIUtility.PingObject(placeable);
                return;
            }

            positions.Add(placeable.currentCell, placeable);
        }
    }

    private void DeleteAll() {
        Dictionary<Vector3Int, Placeable> positions = new Dictionary<Vector3Int, Placeable>();

        Placeable[] placeables = FindObjectsOfType<Placeable>();
        if (placeables.Length == 0) return;

        foreach (Placeable placeable in placeables) {
            if (!_fieldManager.HasTile(placeable.currentCell) || positions.ContainsKey(placeable.currentCell)) {
                DestroyImmediate(placeable.gameObject);
                continue;
            }

            positions.Add(placeable.currentCell, placeable);
        }
    }

    private void ResetPositions() {
        Placeable[] placeables = FindObjectsOfType<Placeable>();
        if (placeables.Length == 0) return;

        foreach (Placeable placeable in placeables) {
            placeable.Position = _fieldManager.GetCellWorldPosition(placeable.currentCell);
        }
    }

    private void OnGUI() {
        _fieldManager = (FieldManager)EditorGUI.ObjectField(new Rect(3, 3, position.width - 6, 20), "Field Manager", _fieldManager, typeof(FieldManager), true);

        if (!_fieldManager) return;

        HandleKeyboard();

        EditorGUILayout.Space(30);

        GUILayout.Label("X-Y axis:");
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Space();
        if (GUILayout.Button("↑", GUILayout.Width(40))) ChangeTargetsPosition(y: 1);
        EditorGUILayout.Space();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Space();
        if (GUILayout.Button("←", GUILayout.Width(40))) ChangeTargetsPosition(x: -1);
        if (GUILayout.Button("↓", GUILayout.Width(40))) ChangeTargetsPosition(y: -1);
        if (GUILayout.Button("→", GUILayout.Width(40))) ChangeTargetsPosition(x: 1);
        EditorGUILayout.Space();
        EditorGUILayout.EndHorizontal();

        GUILayout.Label("Z axis:");
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Space();
        if (GUILayout.Button("↑", GUILayout.Width(40))) ChangeTargetsPosition(z: 1);
        EditorGUILayout.Space();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Space();
        if (GUILayout.Button("↓", GUILayout.Width(40))) ChangeTargetsPosition(z: -1);
        EditorGUILayout.Space();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(30);

        if (GUILayout.Button("Reset position")) ResetPositions();
        if (GUILayout.Button("Ping invalid object")) Ping();
        if (GUILayout.Button("Delete all invalid objects")) DeleteAll();
    }

}
