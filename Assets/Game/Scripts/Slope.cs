using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slope : MonoBehaviour
{
    [SerializeField] private Transform generalMesh = null;
    [SerializeField] private Transform floorMesh = null;
    [SerializeField] private Transform stickMesh = null;
    [SerializeField] private Transform foots = null;
    [SerializeField] private Transform foot2 = null;
    [SerializeField] private Transform footStick1 = null;
    [SerializeField] private Transform footStick2 = null;
    [HideInInspector] public Transform _transform = null;
    public float depth = 0;
    public float length = 0;

    private void Awake()
    {
        _transform = transform;
    }

    public void Setup(float depth, float length)
    {
        this.depth = depth;
        this.length = length;

        floorMesh.localPosition = Vector3.up * (depth -1) ;
        floorMesh.localScale = new Vector3(1, 1, length);

        stickMesh.localRotation = Quaternion.Euler(Mathf.Atan(-GetSlope()) * Mathf.Rad2Deg, 0, 0);
        stickMesh.localScale = new Vector3(1, 1, Mathf.Sqrt(depth * depth + length * length));

        foots.localPosition = Vector3.up * (depth - 1);
        foot2.localPosition = Vector3.forward * length;

        footStick1.transform.localPosition = Vector3.up * (-GetSlope() * (length-1) + 1);
        footStick2.transform.localPosition = Vector3.up * (1-GetSlope());
    }

    public float GetSlope()
    {
        return depth / length;
    }
}
