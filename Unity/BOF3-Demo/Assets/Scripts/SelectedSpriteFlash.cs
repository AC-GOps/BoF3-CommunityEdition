using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SelectedSpriteFlash : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public float flashSpeed;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void StartFlash()
    {
        spriteRenderer.DOColor(Color.white,1f/ flashSpeed).SetLoops(-1,LoopType.Yoyo);
    }

    public void StopFlash()
    {
        spriteRenderer.DOKill();    
        spriteRenderer.DOColor(Color.clear, 0);
    }
}
