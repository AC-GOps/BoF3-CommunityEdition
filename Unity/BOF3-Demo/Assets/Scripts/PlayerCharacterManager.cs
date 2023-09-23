using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacterManager : MonoBehaviour
{
    public static PlayerCharacterManager instance;

    public IsometricCharacterController playerCharacterController;
    public BattleCharacter battleCharacter;
    public InteractionManager interactionManager;

    public List<EnemyBattleCharacter> enemyBattleCharacters = new List<EnemyBattleCharacter>();
    public List<PlayerBattleCharacter> playerBattleCharacters = new List<PlayerBattleCharacter>();

    private void Awake()
    {
        instance = this;
    }

    public void UpdatePlayerInfo(List<PlayerBattleCharacter> battleCharacters)
    {
        playerBattleCharacters.Clear();
        playerBattleCharacters = new List<PlayerBattleCharacter>( battleCharacters );
    }

    public List<EnemyBattleCharacter> CreateNewEnemyList()
    {
        List<EnemyBattleCharacter> CreatedList = new List<EnemyBattleCharacter>();
        List<EnemyBattleCharacter> TempList = new List<EnemyBattleCharacter>(enemyBattleCharacters);

        int enemyamount = Random.Range( 3, 4 );
        print("There are " + enemyamount + " enemies");

        for (int i = 0; i < enemyamount; i++)
        {
            int enemyIndex = Random.Range(0, TempList.Count);
            EnemyBattleCharacter enemy = TempList[enemyIndex];
            TempList.Remove(enemy);
            CreatedList.Add(enemy);
            print("Enemy No." + i + " is " + enemy.nameCharacter);
        }
        return CreatedList;

    }
}
