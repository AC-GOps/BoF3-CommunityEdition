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
}
