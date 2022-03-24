using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour
{
    [SerializeField] private Transform generalMesh = null;
    [SerializeField] private MeshRenderer meshToChangeColor = null;
    [HideInInspector] public Transform _transform = null;

    private void Awake()
    {
        _transform = transform;
    }

    public void Setup(float length)
    {
        generalMesh.localScale = new Vector3(1, 1, length);
        meshToChangeColor.material.color = Random.ColorHSV(0, 1, 0.6f, 0.8f, 0.6f, 0.8f);
    }

    public float GetHeight()
    {
        //print(_transform.position.y);
        return _transform.position.y;
    }
}
