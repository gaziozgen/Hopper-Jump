using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] private Transform characterMesh = null;

    public void CharacterDown(float squishedOffset)
    {
        LeanTween.cancel(characterMesh.gameObject);
        characterMesh.LeanMoveLocalY(squishedOffset, 0.1f).setOnComplete(() =>
        {
            Player.Instance.Jump();
        });
    }

    public void CharacterUp()
    {
        LeanTween.cancel(characterMesh.gameObject);
        characterMesh.LeanMoveLocalY(1f, 0.1f);
    }
}
