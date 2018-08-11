using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class CharacterLoader : MonoBehaviour {

    private PlayerCharacterData character;
    private DataController data;

    public TextMeshProUGUI characterName;
    public TextMeshProUGUI characterLevel;
    // Green Toggle
    // Blue Toggle
    // Purple Toggle
    // Armor Light Toggle
    // Armor Medium Toggle
    // Armor Heavy Toggle
    // Main Hand Weapon
    // Off Hand Weapon
    // Weapon Ability 1
    // Weapon Ability 2
    // Weapon Ability 3
    public GameObject abilitySlot1;
    public GameObject abilitySlot2;
    public GameObject abilitySlot3;
    public GameObject abilitySlot4;
    public GameObject ultimateSlot;
    public GameObject passiveSlot1;
    public GameObject passiveSlot2;
    public GameObject passiveSlot3;
    // Boon bar

    void Start () {
        
        DataLoader dataLoader = new DataLoader();
        data = dataLoader.LoadData();
        
        character = data.GetCharacter(data.GetEditCharacter());

        PopulateAbilities();
        PopulateUltimate();
        PopulatePassives();

	}

    private void PopulateAbilities() {
        for (int i = 0; i < character.selectedTalents.abilities.Length; i++) {
            if (character.selectedTalents.abilities[i].name != "") {
                switch (i) {
                    case 0:
                        abilitySlot1.GetComponentInChildren<TextMeshProUGUI>().text = character.selectedTalents.abilities[i].name;
                        break;
                    case 1:
                        abilitySlot2.GetComponentInChildren<TextMeshProUGUI>().text = character.selectedTalents.abilities[i].name;
                        break;
                    case 2:
                        abilitySlot3.GetComponentInChildren<TextMeshProUGUI>().text = character.selectedTalents.abilities[i].name;
                        break;
                    case 3:
                        abilitySlot4.GetComponentInChildren<TextMeshProUGUI>().text = character.selectedTalents.abilities[i].name;
                        break;
                }
            }
        }
    }

    private void PopulateUltimate() {
        if(character.selectedTalents.ultimate.name != "") {
            ultimateSlot.GetComponentInChildren<TextMeshProUGUI>().text = character.selectedTalents.ultimate.name;
        }
    }

    private void PopulatePassives() {
        for(int i=0; i<character.selectedTalents.passives.Length; i++) {
            if (character.selectedTalents.passives[i].name != "") {
                switch (i) {
                    case 0:
                        passiveSlot1.GetComponentInChildren<TextMeshProUGUI>().text = character.selectedTalents.passives[i].name;
                        break;
                    case 1:
                        passiveSlot2.GetComponentInChildren<TextMeshProUGUI>().text = character.selectedTalents.passives[i].name;
                        break;
                    case 2:
                        passiveSlot3.GetComponentInChildren<TextMeshProUGUI>().text = character.selectedTalents.passives[i].name;
                        break;
                }
            }
        }
    }
	
    public void TalentTree() {

        SceneLoader sceneLoader = new SceneLoader();
        sceneLoader.TalentTree();

    }

    public void SwapTalentSlot() {

        data.SetEditType( EventSystem.current.currentSelectedGameObject.GetComponent<TalentSlotData>().type );
        data.SetEditSlot( EventSystem.current.currentSelectedGameObject.GetComponent<TalentSlotData>().slot );

        SceneLoader sceneLoader = new SceneLoader();
        sceneLoader.Abilities();

    }

}
