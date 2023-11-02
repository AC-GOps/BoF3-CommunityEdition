using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum AbilityElement
{
    Fire,
    Ice,
    Electric,
    Wind,
    Earth,
    Psyonic,
    Status,
    Death
}

public enum AbilityType
{
    ATTACK,
    ASSIST,
    HEAL,
}
public class AbilityMenu : MonoBehaviour
{

    public List<Ability> abilities;
    public List<AbilityUI> Activeabilities;
    public List<AbilityUI> abilityPrefabs;
    private int abilityCount;
    private int selectedCount;
    public int selectedSavedCount;
    public List<Sprite> ElementImages;
    public RectTransform finger;
    public AbilityUI currentUISelected;

    private bool hasInput;

    private void Update()
    {
        UIInput();
    }

    private void UIInput()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {

        }

        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (input == Vector2.zero)
        {
            hasInput = false;
            return;
        }

        if (hasInput)
        {
            return;
        }

        AudioManager.instance.PlaySFX(SFX.Select);
        hasInput = true;
        selectedCount += input.y > 0 ? -1 : 1;

        if(selectedCount < 0)
        {
            selectedCount = 0;
        }
        if(selectedCount>Activeabilities.Count-1)
        {
            selectedCount = Activeabilities.Count-1;
        }
        selectedSavedCount = selectedCount;
        currentUISelected = Activeabilities[selectedCount];
        finger.localPosition = currentUISelected.transform.localPosition;
        BattleUI.instance.UpdateBattleInfo(currentUISelected.ability.abilityDes);   
    }

    public void PopulateAbilityList(List<Ability> playerAbilites, AbilityType type, bool sameCharacter = false)
    {
        abilityCount =0;

        if(!sameCharacter)
        {
            selectedSavedCount = 0;
        }

        foreach (AbilityUI prefab in abilityPrefabs)
        {
            prefab.gameObject.SetActive(false);
        }

        abilities.Clear();
        abilities = new List<Ability> ( playerAbilites );
        Activeabilities.Clear();

        for(int i = 0; i < abilities.Count; i++)
        {
            Ability ability = abilities[i];

            if (ability.type == type)
            {
                var prefab = abilityPrefabs[abilityCount];
                prefab.gameObject.SetActive(true);
                prefab.skillName.text = ability.abilityName;
                prefab.apCost.text = ability.apCost.ToString();
                prefab.element.sprite = FindElementImage(ability.element);
                prefab.ability = ability;
                abilityCount++;
                Activeabilities.Add(prefab);         
            }
        }
        currentUISelected = Activeabilities[selectedSavedCount];
        BattleUI.instance.UpdateBattleInfo(currentUISelected.ability.abilityDes);
    }


    private Sprite FindElementImage(AbilityElement element)
    {
        return ElementImages[(int)element];
    }

}
