using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames;

public class FinalPart : MonoBehaviour
{
    [HideInInspector] public Transform _transform = null;
    [SerializeField] private MeshRenderer meshToChangeColor = null;

    void Awake()
    {
        _transform = transform;
    }

    public void Activate()
    {
        Color newColor = new Color(meshToChangeColor.material.color.r, meshToChangeColor.material.color.g, meshToChangeColor.material.color.b, 1);
        meshToChangeColor.material.color = newColor;
        ConfettiManager.CreateConfettiDirectional(new Vector3(0, _transform.position.y - 5f, _transform.position.z +  5f), Vector3.up, Vector3.one * 5);
    }
}
