using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class LevelGenerator : MonoBehaviour
{

    [SerializeField] private Transform levelParent = null;
    [SerializeField] private float levelLength = 20f;
    private float currentZ = 0;
    private float currentY = 0;

    public void Generate()
    {

        GenerateStart();

        while (currentZ < levelLength)
        {
            float randomValue = Random.Range(0f, 6f);

            if (5 < randomValue)
            {
                GenerateRandomSlopeSet();
            }
            else if (3 < randomValue)
            {
                GenerateRandomSingleHeightSet();
            }
            else if (1 < randomValue)
            {
                GenerateRandomStairSet();
            }
            else
            {
                GenerateBallGroupSet();
            }
        }

        GenerateFinal();
    }

    private void GenerateRandomSlopeSet()
    {
        Debug.Log("SlopeSet()");
        GenerateFlatSurface(0, 5);

        float length = Random.Range(10, 20);
        float slope = Random.Range(0.3f, 0.5f);
        GenerateSlope(length * slope, length);

        int ballCount = Random.Range(0, (int)length / 4);
        float range = length / (ballCount + 1);
        for (int i = 0; i < ballCount; i++)
        {
            GenerateCollectableBallOnSlope(-(i+1) * range, slope);
        }


        GenerateFlatSurface(0, 10);
        if (Random.Range(0f, 1f) < 0.4f)
        {
            GenerateCollectableBallOnFloor(-5);
        }
    }

    private void GenerateRandomSingleHeightSet()
    {
        Debug.Log("SingleHeightSet()");
        GenerateFlatSurface(0, 5);

        float height = Random.Range(2f, 5f);
        GenerateFlatSurface(height, 2);


        GenerateFlatSurface(-height, 10);
        if (Random.Range(0f, 1f) < 0.4f)
        {
            GenerateCollectableBallOnFloor(-5);
        }
    }

    private void GenerateRandomStairSet()
    {
        Debug.Log("StairSet()");
        GenerateFlatSurface(0, 5);

        int stepCount = Random.Range(2, 6);
        float stepHeight = Random.Range(1f, 2f);
        float stepLength = Random.Range(stepHeight + 1, stepHeight + 5);
        for (int i = 0; i < stepCount; i++)
        {
            GenerateFlatSurface(stepHeight, stepLength);

            if (Random.Range(0f, 1f) < 0.3f)
            {
                GenerateCollectableBallOnFloor(-stepLength /2);
            }
        }


        GenerateFlatSurface(0, 10);
        if (Random.Range(0f, 1f) < 0.4f)
        {
            GenerateCollectableBallOnFloor(-5);
        }
    }

    private void GenerateBallGroupSet()
    {
        Debug.Log("GroupSet()");
        GenerateFlatSurface(0, 5);

        float length = Random.Range(10, 20);
        int ballCount = Random.Range(0, (int)length / 6);
        float range = length / (ballCount + 1);

        GenerateFlatSurface(0, length);
        for (int i = 0; i < ballCount; i++)
        {
            GenerateCollectableBallOnFloor(-(i + 1) * range);
        }

        GenerateFlatSurface(0, 10);
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

    private void GenerateCollectableBallOnFloor(float offsetOnLastFloor)
    {
        InstantiatePrefab("CollectableBall", new Vector3(0, currentY, currentZ + offsetOnLastFloor), Quaternion.identity, levelParent);
    }

    private void GenerateCollectableBallOnSlope(float offsetOnLastFloor, float slope)
    {
        InstantiatePrefab("CollectableBall", new Vector3(0, currentY + -offsetOnLastFloor * slope, currentZ + offsetOnLastFloor), Quaternion.identity, levelParent);
    }

    private void GenerateStart()
    {
        InstantiatePrefab("Floor", new Vector3(0, 0, -20), Quaternion.identity, levelParent);
        InstantiatePrefab("Floor", new Vector3(0, 0, -10), Quaternion.identity, levelParent);

        GenerateFlatSurface(0, 10);
    }

    private void GenerateFinal()
    {
        InstantiatePrefab("Final", new Vector3(0, currentY, currentZ), Quaternion.identity, levelParent);
    }

    private GameObject InstantiatePrefab(string name, Vector3 position, Quaternion rotation, Transform parent)
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
