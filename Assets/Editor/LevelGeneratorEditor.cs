using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(LevelGenerator))]
public class LevelGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        LevelGenerator generator = (LevelGenerator)target;

        if (GUILayout.Button("Generate Level"))
        {
            generator.Generate();
        }

        if (GUILayout.Button("Reset"))
        {
            generator.ResetLevel();
        }

        if (GUILayout.Button("Generate Environment"))
        {
            generator.GenerateEnvironment();
        }

        if (GUILayout.Button("Reset Environment"))
        {
            generator.ResetEnvironment();
        }

        EditorSceneManager.MarkSceneDirty(generator.gameObject.scene);
    }
}