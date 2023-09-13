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
        if(auto)
        {
            Play();
        }
    }

    public void Play()
    {
        image = GetComponent<Image>();
        image.DOFade(EndValue, Time).SetEase(Ease.InCubic);
    }

}
