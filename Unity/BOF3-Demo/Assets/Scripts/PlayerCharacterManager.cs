using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacterManager : MonoBehaviour
{
    public static PlayerCharacterManager instance;

    public IsometricCharacterController playerCharacterController;
    public BattleCharacter battleCharacter;
    public InteractionManager interactionManager;

    public List<BattleCharacter> enemyBattleCharacters = new List<BattleCharacter>();
    public List<BattleCharacter> playerBattleCharacters = new List<BattleCharacter>();

    private void Awake()
    {
        instance = this;
    }

    public void UpdatePlayerInfo(List<BattleCharacter> battleCharacters)
    {
        playerBattleCharacters.Clear();
        playerBattleCharacters = new List<BattleCharacter>( battleCharacters );
    }
}
