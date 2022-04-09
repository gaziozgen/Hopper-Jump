using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private Transform levelParent = null;
    [SerializeField] private Transform environmentParent = null;
    [SerializeField] private float levelLength = 20f;
    private float currentZ = 0;
    private float currentY = 0;

    public void Generate()
    {
        GenerateStart();

        /*float length = Random.Range(10, 20);
        GenerateStickyFloor(length);
        GenerateBasicFloor(10);

        length = Random.Range(10, 20);
        GenerateStickyFloor(length);
        GenerateBasicFloor(10);

        length = Random.Range(10, 20);
        GenerateStickyFloor(length);
        GenerateBasicFloor(10);*/

        while (currentZ < levelLength)
        {
            GenerateBasicFloor(5);

            float randomValue = Random.Range(0f, 12f);
            if (10 < randomValue)
            {
                // slope set

                float length = Random.Range(5, 20);
                float slope = -Random.Range(0.3f, 0.5f);

                if (CheckMinHeight(length * slope - 1)) // offset eklenmiþ
                    continue;

                GenerateSlope(length * slope, length);

                int ballCount = Random.Range(0, (int)length / 4);
                float range = length / (ballCount + 1);
                for (int i = 0; i < ballCount; i++)
                {
                    GenerateCollectableBallOnSlope(-(i + 1) * range, slope);
                }
            }
            else if (8 < randomValue)
            {
                // single height

                float height = Random.Range(2, 5);
                GenerateSingleHeigth(1, height);
            }
            else if (6 < randomValue)
            {
                // stair set

                int stepCount = Random.Range(2, 5);
                float stepHeight = Random.Range(1f, 2f);
                float stepLength = Random.Range(stepHeight + 2, stepHeight + 5);
                for (int i = 0; i < stepCount; i++)
                {
                    GenerateMiniLevelUp(stepLength, stepHeight);
                    RandomBallOnFloor(0.2f, -stepLength * 2/3);
                }
            }
            else if (4 < randomValue)
            {
                // level heigth set

                float height = Random.Range(3, 5);
                GenerateLevelUp(10, height);
            }
            else if (2 < randomValue)
            {
                // pit set

                if (CheckMinHeight(-5))
                    continue;

                GenerateLevelDown(5, -5);
                GenerateLevelUp(5, 5);
            }
            else
            {
                // sticky floor set

                GenerateStickyFloor(10);
            }
            GenerateBasicFloor(10);
            RandomBallOnFloor(0.5f, -5);
        }

        GenerateFinal();
    }

    private bool CheckMinHeight(float newHeigthOffset)
    {
        return false;
        /*if (currentY + newHeigthOffset < 0)
        {
            currentZ -= 5;
            DestroyImmediate(levelParent.GetChild(levelParent.childCount - 1).gameObject);
            return true;
        }
        else
        {
            return false;
        }*/
    }

    private void RandomBallOnFloor(float probability, float offset)
    {
        if (Random.Range(0f, 1f) < probability)
        {
            GenerateCollectableBallOnFloor(offset);
        }
    }

    private void GenerateCollectableBallOnFloor(float offsetOnLastFloor)
    {
        InstantiatePrefab("CollectableBall", new Vector3(0, currentY, currentZ + offsetOnLastFloor), Quaternion.identity, levelParent);
    }

    private void GenerateCollectableBallOnSlope(float offsetOnLastFloor, float slope)
    {
        CollectableBall cb = InstantiatePrefab("CollectableBall", new Vector3(0, currentY + offsetOnLastFloor * slope, currentZ + offsetOnLastFloor), Quaternion.identity, levelParent).GetComponent<CollectableBall>();
        cb.HideShadow();
    }

    private void GenerateStart()
    {
        Floor floor = InstantiatePrefab("Floor", new Vector3(0, 0, -20), Quaternion.identity, levelParent).GetComponent<Floor>();
        floor.Setup(10);
        floor = InstantiatePrefab("Floor", new Vector3(0, 0, -10), Quaternion.identity, levelParent).GetComponent<Floor>();
        floor.Setup(10);

        GenerateBasicFloor(10);
    }

    private void GenerateFinal()
    {
        InstantiatePrefab("Final", new Vector3(0, currentY, currentZ), Quaternion.identity, levelParent);
    }

    private void GenerateBasicFloor(float length)
    {
        Floor floor = InstantiatePrefab("Floor", new Vector3(0, currentY, currentZ), Quaternion.identity, levelParent).GetComponent<Floor>();
        floor.Setup(length);
        currentZ += length;
    }

    private void GenerateLevelUp(float length, float heigth)
    {
        Floor floor = InstantiatePrefab("Floor", new Vector3(0, currentY + heigth, currentZ), Quaternion.identity, levelParent).GetComponent<Floor>();
        floor.SetupLevelUp(length, heigth);
        currentY += heigth;
        currentZ += length;
    }

    private void GenerateMiniLevelUp(float length, float heigth)
    {
        Floor floor = InstantiatePrefab("Floor", new Vector3(0, currentY + heigth, currentZ), Quaternion.identity, levelParent).GetComponent<Floor>();
        floor.SetupMiniLevelUp(length, heigth);
        currentY += heigth;
        currentZ += length;
    }

    private void GenerateLevelDown(float length, float heigth)
    {
        Floor floor = InstantiatePrefab("Floor", new Vector3(0, currentY + heigth, currentZ), Quaternion.identity, levelParent).GetComponent<Floor>();
        floor.SetupLevelDown(length);
        currentY += heigth;
        currentZ += length;
    }

    private void GenerateSingleHeigth(float length, float heigth)
    {
        Floor floor = InstantiatePrefab("Floor", new Vector3(0, currentY + heigth, currentZ), Quaternion.identity, levelParent).GetComponent<Floor>();
        floor.SetupSingleHeight(length, heigth);
        currentZ += length;
    }
    
    private void GenerateStickyFloor(float length)
    {
        Floor floor = InstantiatePrefab("Floor", new Vector3(0, currentY, currentZ), Quaternion.identity, levelParent).GetComponent<Floor>();
        floor.SetupStickyFloor(length);
        currentZ += length;
    }

    private void GenerateSlope(float depth, float length)
    {
        Slope slope = InstantiatePrefab("Slope", new Vector3(0, currentY, currentZ), Quaternion.identity, levelParent).GetComponent<Slope>();
        slope.Setup(depth, length);
        currentY += depth;
        currentZ += length;
    }

    private GameObject InstantiatePrefab(string name, Vector3 position, Quaternion rotation, Transform parent)
    {
        //return null;
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

    public void GenerateEnvironment()
    {
        float length = 1000;
        float x_range = -20;
        float x_offset = -20;
        int z_range = 20;
        int z_offset = 15;

        float y_range = 40;
        float y_offset = 100;

        for (int i = 0; i < length; i +=  z_offset + Random.Range(0, z_range))
        {
            GenerateBuilding(new Vector3(x_offset + Random.Range(x_range, 0), -100, i), new Vector3(10, y_offset + Random.Range(0, y_range), 10));
        }
    }

    public void GenerateBuilding(Vector3 position, Vector3 scale)
    {
        Transform t = InstantiatePrefab("Building", position, Quaternion.identity, environmentParent).transform;
        t.localScale = scale;
    }

    public void ResetEnvironment()
    {
        for (int i = environmentParent.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(environmentParent.GetChild(i).gameObject);
        }
    }
}
