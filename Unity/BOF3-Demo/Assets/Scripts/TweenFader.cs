using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class TweenFader : MonoBehaviour 
{
    private Image image;
    public float speed;
    public bool loop;
    public bool auto;
    public float value;
    public Ease ease;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

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
        //image.DOColor(Color.clear,speed).SetLoops(loop?-1:0,LoopType.Yoyo).SetEase(Ease.InCubic);
        image.DOFade(value,speed).SetLoops(loop ? -1 : 0, LoopType.Yoyo).SetEase(ease);
    }

    public void FadeOut()
    {
        image.DOFade(0, speed);
    }

    public void FadeIn()
    {
        image.DOFade(1, speed);
    }
}
