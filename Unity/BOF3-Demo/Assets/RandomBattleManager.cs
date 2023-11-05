using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RandomBattleManager : MonoBehaviour
{

    public TurnBasedBattleEngine BattleEngine;
    public IsometricCharacterController isoController;
    public List<Transform> LocationParents = new List<Transform>();
    public float battleTimerMulitplier;
    public float battleTimerMax;
    public float battleTimer;
    public bool battleEnabled;
    public bool playerMoving;

    private float closestDist;
    public BattleLocations battleLocations;

    public void CheckDistanceTraveled()
    {
        if(!playerMoving)
        {
            return;
        }

        if(battleEnabled)
        {
            FindClosestBattleLocation();
            return;
        }

        var mag = isoController._input.normalized.magnitude;
        if (mag>0)
        {
            float value = mag * battleTimerMulitplier;
            battleTimer += value;
        }

        if(battleTimer>battleTimerMax)
        {
            battleEnabled = true;
        }
    }

    public void FindClosestBattleLocation()
    {
        PlayerInputManager.Instance.SwapActionMaps("");
        closestDist = 100;
        foreach(Transform t in LocationParents)
        {
            float distance = Vector3.Distance(isoController.transform.position, t.position);

            if(distance < closestDist)
            {
                closestDist = distance;
                battleLocations = t.GetComponent<BattleLocations>();
            }
        }

        isoController.SetEmoji();
        StartCoroutine(BeginBattleTransistion());
    }

    public IEnumerator BeginBattleTransistion()
    {
        yield return new WaitForSeconds(0.2f);
        var player = PlayerCharacterManager.instance.playerCharacterController;
        player._renderer.DOFade(0, 0.3f);
        yield return new WaitForSeconds(0.3f);
        player.transform.DOMove(battleLocations.Locations[7].position, 1f);
        BattleEngine.SetupBattleLocations(battleLocations.Locations);
        yield return new WaitForSeconds(1f);
        AreaManager.instance.HideGameObjects();
        yield return new WaitForSeconds(1f);
        BattleEngine.Init();
    }

    private void Update()
    {
        CheckDistanceTraveled();
    }

    public void ResetRandomBattle()
    {
        battleTimer = 0;
        battleEnabled = false;
    }
}
