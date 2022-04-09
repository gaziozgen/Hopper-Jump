using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour
{
    public FloorType floorType = FloorType.BASIC;
    [SerializeField] private Transform generalMesh = null;
    [SerializeField] private Transform floorMesh = null;
    [SerializeField] private Transform stairOnBack = null;
    [SerializeField] private Transform singleHeigth = null;
    [SerializeField] private Transform stickyFloor = null;
    [SerializeField] private Transform pillow = null;
    [SerializeField] private Transform miniLevelPillow = null;


    [HideInInspector] public Transform _transform = null;

    private void Awake()
    {
        _transform = transform;
    }
    // setuplarda yapýlan tüm iþlemler tasarým amaçlýdýr, floor height generator da belirlenir, deðiþmez
    // ünitelerin öncesinde veya sonrasýnda floora ihtiyacý varsa yine genmerator da atanýr
    
    public void Setup(float length)
    {
        floorMesh.localScale = new Vector3(1, 1, length);
    }

    public void SetupLevelUp(float length, float height)
    {
        floorType = FloorType.LEVEL_UP;
        floorMesh.localScale = new Vector3(1, 1, length);

        stairOnBack.gameObject.SetActive(true);
        stairOnBack.localScale = new Vector3(1, height * 0.25f, 1);
    }

    public void SetupLevelDown(float length)
    {
        floorType = FloorType.LEVEL_DOWN;
        floorMesh.localScale = new Vector3(1, 1, length);

        pillow.gameObject.SetActive(true);
    }

    public void SetupSingleHeight(float length, float height)
    {
        floorType = FloorType.SINGLE_HEIGHT;
        floorMesh.localScale = new Vector3(1, 1, length);
        floorMesh.localPosition = new Vector3(0, -height, 0);

        singleHeigth.gameObject.SetActive(true);
        //singleHeigth.localScale = new Vector3(1, height, 1);
    }

    public void SetupMiniLevelUp(float length, float height)
    {
        floorType = FloorType.MINI_LEVEL_UP;
        floorMesh.localScale = new Vector3(1, 1, length);

        miniLevelPillow.gameObject.SetActive(true);
        miniLevelPillow.localScale = new Vector3(1, height, 1);
    }

    public void SetupStickyFloor(float length)
    {
        floorType = FloorType.STICKY_FLOOR;
        floorMesh.localScale = new Vector3(1, 1, length);

        stickyFloor.gameObject.SetActive(true);
        stickyFloor.localPosition = new Vector3(0, 0, -0.5f);
    }

    public float GetHeight()
    {
        return _transform.position.y;
    }

    public FloorType GetFloorType()
    {
        return floorType;
    }

    public enum FloorType { BASIC, LEVEL_UP, LEVEL_DOWN, SINGLE_HEIGHT, MINI_LEVEL_UP, STICKY_FLOOR }
}
