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

    private List<Transform> lastFields;

    private bool placing = true;

    [MenuItem("Tools/Field Placer")]
    public static void Open() => GetWindow<FieldPlacer>();

    private void OnEnable()
    {
        currentPrefabIdx = 0;

        lastFields = new List<Transform>();

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
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(sprites[i]))
            {
                int localI = i;
                currentPrefabIdx = localI;
            }
            EditorGUILayout.EndHorizontal();
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
                    Field testField1 = ((GameObject)Selection.objects[0]).GetComponent<Field>();
                    Field testField2 = ((GameObject)Selection.objects[1]).GetComponent<Field>();
                    if (testField1 == null || testField2 == null)
                        return;

                    testField1.neighbours.Add(testField2);
                    testField2.neighbours.Add(testField1);
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

            // Wenn man beim Platzieren Shift gedrückt hält, werden die Felder miteinander verbunden
            if (Event.current.shift)
            {
                if (lastFields.Count > 0) // Standardfall: Neues Feld wird mit vorherigem verbunden
                {
                    Field lastField = lastFields[lastFields.Count - 1].GetComponent<Field>();
                    field.neighbours.Add(lastField);
                    lastField.neighbours.Add(instantiatedObject.GetComponent<Field>());
                }
                else if (lastFields.Count == 0 // Ein Feld wurde ausgewählt, das nächste soll mit ihm verbunden werden
                      && Selection.count == 1 && Selection.objects[0] is GameObject)
                {
                    Field selectedField = ((GameObject)Selection.objects[0]).GetComponent<Field>();
                    if (selectedField != null)
                        lastFields.Add(selectedField.transform);

                    field.neighbours.Add(selectedField);
                    selectedField.neighbours.Add(field);
                }
            }
            else
                lastFields.Clear();

            lastFields.Add(instantiatedObject.transform);

            Event.current.Use();
        }
    }
}
