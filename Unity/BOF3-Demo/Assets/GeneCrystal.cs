using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneCrystal : MonoBehaviour
{

    public Animator animator;
    public float MaxTime, MinTime;

    public PlayerBattleCharacter playerBattleCharacter;

    private void Start()
    {
        StartCoroutine(PlayShineRandom());
    }

    public IEnumerator PlayShineRandom()
    {
        animator.SetTrigger("Shine");
        yield return new WaitForSeconds(Random.Range(MinTime,MaxTime));
        StartCoroutine(PlayShineRandom());
    }

    public void AddGeneAbility()
    {
        var PCM  = PlayerCharacterManager.instance;
        PCM.playerBattleCharacters.Add(playerBattleCharacter);
        PCM.enemyMax = 3;
        PCM.enemyMin = 2;
        gameObject.SetActive(false);
    }
}
