using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CrystalManager : MonoBehaviour
{
    public ParticleSystem breaking;
    public ParticleSystem breakOpen;
    public GameObject crystalNormal;
    public GameObject crystalBroken;
    public Image whiteOut;
    public AudioClip breakopen;
    public AudioClip crack;

    public void BeginCrystalAnim()
    {
        StartCoroutine(BreakCrystal());
    }


    private IEnumerator BreakCrystal()
    {
        breaking.Play();
        yield return new WaitForSeconds(0.5f);
        crystalNormal.GetComponent<MeshRenderer>().material.DOColor(Color.white, "_EmissionColor", 5f).SetEase(Ease.InCubic).OnComplete(Break);
        yield return new WaitForSeconds(4);
        AudioManager.instance.PlaySFXFromClip(crack);
        yield return new WaitForSeconds(0.2f);
        AudioManager.instance.PlaySFXFromClip(crack);
        whiteOut.DOFade(1, 1.5f).SetEase(Ease.InCubic).OnComplete(CrystalSwap);
    }

    private void WhiteFlash()
    {
        whiteOut.DOFade(0, 0.3f).OnComplete(CutSceneManager.instance.OnCompleteEvent);
    }

    private void Break()
    {
        breakOpen.Play();
    }

    private void CrystalSwap()
    {
        AudioManager.instance.StopSFX();
        AudioManager.instance.PlaySFXFromClip(breakopen);
        crystalNormal.SetActive(false);
        crystalBroken.SetActive(true);
        WhiteFlash();
    }


}
