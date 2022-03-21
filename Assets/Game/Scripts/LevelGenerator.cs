using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class LevelGenerator : MonoBehaviour
{

    [SerializeField] private Transform levelParent = null;
    private float currentZ = 0;
    private float currentY = 0;

    public void Generate()
    {
        Debug.Log("Generate");

        GenerateFlatSurface(0, 10);

        GenerateFlatSurface(0, 10);
        GenerateFlatSurface(0, 10);
        GenerateFlatSurface(0, 10);
        GenerateFlatSurface(0, 10);
        /*GenerateFlatSurface(-5, 10);
        GenerateFlatSurface(4, 1);
        GenerateFlatSurface(-4, 10);
        GenerateSlope(5, 10);
        GenerateFlatSurface(0, 10);*/
    }

    private void GenerateFlatSurface(float heigth, float length)
    {
        Floor floor = InstantiatePrefab("Floor", new Vector3(0, currentY + heigth, currentZ), Quaternion.identity, levelParent).GetComponent<Floor>();
        floor.Setup(length);
        currentY += heigth;
        currentZ += length;
    }

    private void GenerateSlope(float depth, float length)
    {
        Slope slope = InstantiatePrefab("Slope", new Vector3(0, currentY, currentZ), Quaternion.identity, levelParent).GetComponent<Slope>();
        slope.Setup(depth, length);
        currentY -= depth;
        currentZ += length;
    }

    public GameObject InstantiatePrefab(string name, Vector3 position, Quaternion rotation, Transform parent)
    {
        GameObject variableForPrefab = Resources.Load("Fate Games/Prefabs/" + name, typeof(GameObject)) as GameObject;
        GameObject go = PrefabUtility.InstantiatePrefab(variableForPrefab) as GameObject;
        go.transform.SetPositionAndRotation(position, rotation);
        go.name = name;
        go.transform.parent = parent;
        return go;
    }

    public void ResetLevel()
    {
        Debug.Log("reset");
        currentY = 0;
        currentZ = 0;
        for (int i = levelParent.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(levelParent.GetChild(i).gameObject);
        }
    }
}
