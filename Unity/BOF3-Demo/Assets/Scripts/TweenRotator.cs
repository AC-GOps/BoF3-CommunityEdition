using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TweenRotator : MonoBehaviour
{
    public float speed;
    public bool loop;
    public bool auto;
    public Vector3 rotateAngle;
    public Ease ease;

    private void Start()
    {
        if (!auto)
        {
            return;
        }
        StartTween();
    }

    public void StartTween()
    {
        transform.DORotate(rotateAngle, speed).SetLoops(loop ? -1 : 0, LoopType.Restart).SetEase(ease);
    }
}
