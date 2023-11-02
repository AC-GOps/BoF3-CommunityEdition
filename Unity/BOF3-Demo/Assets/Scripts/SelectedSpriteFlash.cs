using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SelectedSpriteFlash : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Material material;
    public float flashSpeed;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        material = spriteRenderer.material;
    }

    public void StartFlash()
    {
        material.DOFloat(1,"_StrongTintFade", flashSpeed).SetLoops(-1, LoopType.Yoyo);
    }

    public void StopFlash()
    {
        material.DOKill();
        material.DOFloat(0, "_StrongTintFade", 0.1f);
    }
}
