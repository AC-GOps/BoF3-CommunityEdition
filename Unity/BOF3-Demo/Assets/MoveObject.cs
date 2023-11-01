using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MoveObject : MonoBehaviour
{
    public Transform startLoc;
    public Transform endLoc;
    public float durration;
    public bool looping;

    public void BeginMovement()
    {
        transform.DOMove(endLoc.position, durration).OnComplete(CutSceneManager.instance.OnCompleteEvent);
    }
}
