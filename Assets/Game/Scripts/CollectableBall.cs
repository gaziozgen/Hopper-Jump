using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableBall : MonoBehaviour
{
    [HideInInspector] public Transform _transform = null;
    private BoxCollider boxCollider = null;
    private bool owned = false;

    void Awake()
    {
        _transform = transform;
        boxCollider = GetComponent<BoxCollider>();
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

}
