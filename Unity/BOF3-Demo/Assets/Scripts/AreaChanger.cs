using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaChanger : MonoBehaviour
{
    public FadeImage fade;
    public Transform newLocation;
    public ParticleSystem teleportEffect;

    public IEnumerator ChangeAreaWithFade()
    {
        Vector3 pos = PlayerCharacterManager.instance.playerCharacterController.transform.position;
        teleportEffect.transform.position = new Vector3(pos.x,pos.y -0.5f, pos.z);
        teleportEffect.Play();
        yield return new WaitForSeconds(1);
        fade.FadeOut();
        yield return new WaitForSeconds(fade.Time + 1);
        PlayerCharacterManager.instance.playerCharacterController.transform.position = newLocation.position;
        fade.FadeIn();
    }

    public void LoadArea()
    {
        StartCoroutine(ChangeAreaWithFade());
    }
}
