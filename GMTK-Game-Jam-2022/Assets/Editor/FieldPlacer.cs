using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Sprites;

public class FieldPlacer : EditorWindow
{
    private GameObject [] prefabs;
    private Texture [] sprites;

    private int currentPrefabIdx;

    private List<SerializedObject> lastFields;

    private bool placing = true;

    [MenuItem("Tools/Field Placer")]
    public static void Open() => GetWindow<FieldPlacer>();

    private void OnEnable()
    {
        currentPrefabIdx = 0;

        lastFields = new List<SerializedObject>();

        // Alle Prefabs aus dem Ordner Assets/Prefabs/Fields als GameObjects laden
        string [] assetGUIDs = AssetDatabase.FindAssets("t:prefab", new string[] { "Assets/Prefabs/Fields" });
        prefabs = Array.ConvertAll(
            assetGUIDs, 
            guid => AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guid))
        );

        // Zu jedem Prefab den Sprite/die Texture speichern
        sprites = Array.ConvertAll(
            prefabs,
            go => {
                SpriteRenderer renderer = go.GetComponent<SpriteRenderer>();
                Rect spriteRectInTexture = renderer.sprite.textureRect;
                Color[] pixels = renderer.sprite.texture.GetPixels
                    ((int)spriteRectInTexture.x, (int)spriteRectInTexture.y,
                     (int)spriteRectInTexture.width, (int)spriteRectInTexture.height);
                Texture2D spriteTexture = new Texture2D((int)spriteRectInTexture.width, (int)spriteRectInTexture.height);
                spriteTexture.SetPixels(pixels, 0);
                spriteTexture.Apply();
                return spriteTexture;
            }
        );

        SceneView.duringSceneGui += DuringSceneGUI;
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= DuringSceneGUI;
    }

    private void OnGUI()
    {
        for (int i = 0; i < prefabs.Length; i++)
        {
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            GUILayout.Label(prefabs[i].name);
            if (GUILayout.Button(sprites[i], GUILayout.Width(180), GUILayout.Height(60)))
            {
                int localI = i;
                currentPrefabIdx = localI;
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }
    }

    private void DuringSceneGUI(SceneView sceneView)
    {
        // Rechte Maustaste
        if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
        {
            lastFields.Clear();
            placing = !placing;
            Event.current.Use();
        }

        // Wenn man gerade nicht im placing mode ist, kann man mit C zwei ausgewählte Felder verbinden
        if (!placing)
        {
            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.C)
            {
                if (Selection.count == 2 && Selection.objects[0] is GameObject && Selection.objects[1] is GameObject)
                {
                    Field field1 = ((GameObject)Selection.objects[0]).GetComponent<Field>();
                    Field field2 = ((GameObject)Selection.objects[1]).GetComponent<Field>();
                    if (field1 == null || field2 == null)
                        return;

                    SerializedObject field1SO = new SerializedObject(field1);
                    SerializedObject field2SO = new SerializedObject(field2);

                    SerializedProperty field1NeighboursProp = field1SO.FindProperty("neighbours");
                    SerializedProperty field2NeighboursProp = field2SO.FindProperty("neighbours");

                    field1NeighboursProp.arraySize++;
                    field1NeighboursProp.GetArrayElementAtIndex(field1NeighboursProp.arraySize - 1)
                                       .objectReferenceValue = field2SO.targetObject;

                    field2NeighboursProp.arraySize++;
                    field2NeighboursProp.GetArrayElementAtIndex(field2NeighboursProp.arraySize - 1)
                                       .objectReferenceValue = field1SO.targetObject;

                    // Mit Undo
                    field1SO.ApplyModifiedPropertiesWithoutUndo();
                    field2SO.ApplyModifiedPropertiesWithoutUndo();
                }

                Event.current.Use();
            }

            return;
        }
            
        // Mit jeder Mausbewegung wird der SceneView refresht
        if (Event.current.type == EventType.MouseMove)
            SceneView.RepaintAll();

        // Position der Maus in der Scene bestimmen
        Vector3 mousePosition = Event.current.mousePosition;
        Ray ray = HandleUtility.GUIPointToWorldRay(mousePosition);
        Vector2 realMousePosition = (Vector2) ray.origin;

        // Preview-Sprite an der Mausposition anzeigen
        Graphics.DrawTexture(
            new Rect(realMousePosition.x - 0.5f, realMousePosition.y - 0.5f, 1, 1),
            sprites[currentPrefabIdx]
        );

        // Wenn man die linke Maustaste drückt, wird das ausgewählte Prefab platziert
        if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
        {
            GameObject instantiatedObject = (GameObject)PrefabUtility.InstantiatePrefab(prefabs[currentPrefabIdx]);
            instantiatedObject.transform.position = realMousePosition;

            Field field = instantiatedObject.GetComponent<Field>();
            SerializedObject fieldSO = new SerializedObject(field);

            // Wenn man beim Platzieren Shift gedrückt hält, werden die Felder miteinander verbunden
            if (Event.current.shift)
            {
                SerializedProperty fieldNeighboursProp = fieldSO.FindProperty("neighbours");

                if (lastFields.Count > 0) // Standardfall: Neues Feld wird mit vorherigem verbunden
                {
                    SerializedObject lastFieldSO = lastFields[lastFields.Count - 1];
                    /*field.neighbours.Add(lastField);
                    lastField.neighbours.Add(instantiatedObject.GetComponent<Field>());*/

                    fieldNeighboursProp.arraySize++;
                    fieldNeighboursProp.GetArrayElementAtIndex(fieldNeighboursProp.arraySize - 1)
                                       .objectReferenceValue = lastFieldSO.targetObject;

                    SerializedProperty lastFieldNeighboursProp = lastFieldSO.FindProperty("neighbours");
                    lastFieldNeighboursProp.arraySize++;
                    lastFieldNeighboursProp.GetArrayElementAtIndex(lastFieldNeighboursProp.arraySize - 1)
                                           .objectReferenceValue = fieldSO.targetObject;

                    lastFieldSO.ApplyModifiedPropertiesWithoutUndo();
                }
                else if (lastFields.Count == 0 // Ein Feld wurde ausgewählt, das nächste soll mit ihm verbunden werden
                      && Selection.count == 1 && Selection.objects[0] is GameObject)
                {
                    Field selectedField = ((GameObject)Selection.objects[0]).GetComponent<Field>();
                    SerializedObject selectedFieldSO = new SerializedObject(field);

                    if (selectedField != null)
                        lastFields.Add(selectedFieldSO);

                    /*field.neighbours.Add(selectedField);
                    selectedField.neighbours.Add(field);*/

                    fieldNeighboursProp.arraySize++;
                    fieldNeighboursProp.GetArrayElementAtIndex(fieldNeighboursProp.arraySize - 1)
                                       .objectReferenceValue = selectedField;


                    SerializedProperty selectedFieldNeighboursProp = selectedFieldSO.FindProperty("neighbours");
                    selectedFieldNeighboursProp.arraySize++;
                    selectedFieldNeighboursProp.GetArrayElementAtIndex(selectedFieldNeighboursProp.arraySize - 1)
                                               .objectReferenceValue = field;

                    selectedFieldSO.ApplyModifiedPropertiesWithoutUndo();
                }

                fieldSO.ApplyModifiedPropertiesWithoutUndo();
            }
            else
                lastFields.Clear();

            lastFields.Add(fieldSO);

            Event.current.Use();
        }
    }
}
