using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class FlashText : MonoBehaviour
{
    private TMP_Text text;
    public float flashSpeed;

    private void Awake()
    {
        text = GetComponent<TMP_Text>();
    }

    private void Start()
    {
        StartFlash();
    }

    public void StartFlash()
    {
        text.DOColor(Color.clear,flashSpeed).SetLoops(-1,LoopType.Yoyo).SetEase(Ease.InCubic);
    }
}
