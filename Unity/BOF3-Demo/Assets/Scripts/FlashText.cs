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
    public void EndFlash()
    {
        text.DOKill();
        text.DOColor(Color.white, 0.1f);
        // transform.DOShakeScale(flashSpeed, 1, 0, 0, true);
        transform.DOPunchScale(new Vector3(0.2f, 0.2f, 0.2f), flashSpeed, 1, 0);
    }
    public void FadeOut()
    {
        text.DOColor(Color.clear, flashSpeed);

    }
}
