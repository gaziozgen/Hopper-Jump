using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slope : MonoBehaviour
{
    [SerializeField] private Transform generalMesh = null;
    [SerializeField] private Transform restMesh = null;
    [SerializeField] private Transform slopeMesh = null;
    [HideInInspector] public Transform _transform = null;

    private void Awake()
    {
        _transform = transform;
    }

    public void Setup(float depth, float length)
    {
        restMesh.localPosition = -Vector3.up * depth;
        slopeMesh.localScale = new Vector3(1, depth, 1);
        generalMesh.localScale = new Vector3(1, 1, length);
    }

    public float GetSlope()
    {
        //print(slopeMesh.localScale.y / generalMesh.localScale.z);
        return slopeMesh.localScale.y / generalMesh.localScale.z;
    }
}
