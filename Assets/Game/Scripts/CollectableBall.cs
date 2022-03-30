using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableBall : MonoBehaviour
{
    [SerializeField] private MeshRenderer meshToChangeColor = null;
    [HideInInspector] public Transform _transform = null;
    private BoxCollider boxCollider = null;
    private Color color;
    private bool owned = false;

    void Awake()
    {
        _transform = transform;
        boxCollider = GetComponent<BoxCollider>();
        color = Ball.CreateRandomColor();
        meshToChangeColor.material.color = color;
    }

    public bool GetBall()
    {
        if (!owned)
        {
            owned = true;
            Dissapear();
            return true;
        }
        else
        {
            return false;
        }
    }

    private void Dissapear()
    {
        boxCollider.enabled = false;
        _transform.LeanScale(Vector3.zero, 0.1f);
    }

    public Color GetColor()
    {
        return color;
    }

}
