using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class FadeImage : MonoBehaviour
{
    Image image;
    public float Time;
    public float EndValue;
    public bool auto;

    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
        if (auto)
        {
            FadeIn();
        }
    }

    public void FadeIn()
    {
        image.DOFade(0, Time).SetEase(Ease.InCubic);
    }

    public void FadeOut()
    {
        image.DOFade(1, Time).SetEase(Ease.InCubic);
    }
}
