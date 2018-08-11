using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class CharacterLoader : MonoBehaviour {

    private PlayerCharacterData character;
    private DataController data;

    public TMP_InputField characterName;
    public TextMeshProUGUI characterLevel;
    public Button greenToggle;
    public Button blueToggle;
    public Button purpleToggle;
    public Button lightToggle;
    public Button mediumToggle;
    public Button heavyToggle;
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

    private Color32 unsetColor = new Color32( 245, 245, 245, 255 );
    private Color32 setColor = new Color32( 220, 220, 220, 255 );

    void Start () {
        
        // Get Character Data
        DataLoader dataLoader = new DataLoader();
        data = dataLoader.LoadData();        
        character = data.GetCharacter(data.GetEditCharacter());

        CheckPreviousPage();
        SetOnClickEvents();
        PopulateTopBar();
        PopulateAbilities();
        PopulateUltimate();
        PopulatePassives();

	}

    private void SaveCharacter() {
        character.name = characterName.text;
        data.SaveCharacter( character );
    }

    private void CheckPreviousPage() {
        if (data.GetPreviousPage() == 3) {
            GameObject.Find( "Backbutton" ).GetComponent<Button>().onClick.RemoveAllListeners();
            GameObject.Find( "Backbutton" ).GetComponent<Button>().onClick.AddListener( delegate { new SceneLoader().PartiesMenu(); } );
        }
    }

    private void SetOnClickEvents() {

        // Back button
        GameObject.Find( "Backbutton" ).GetComponent<Button>().onClick.AddListener( delegate { SaveCharacter(); } );

        // Character rarity
        greenToggle.GetComponent<Button>().onClick.AddListener(delegate { ClickQualityToggleButton( 1 ); } );
        blueToggle.GetComponent<Button>().onClick.AddListener( delegate { ClickQualityToggleButton( 2 ); } );
        purpleToggle.GetComponent<Button>().onClick.AddListener( delegate { ClickQualityToggleButton( 3 ); } );

        //
        lightToggle.GetComponent<Button>().onClick.AddListener( delegate{ ClickArmorToggleButton( 0 ); } );
        mediumToggle.GetComponent<Button>().onClick.AddListener( delegate { ClickArmorToggleButton( 1 ); } );
        heavyToggle.GetComponent<Button>().onClick.AddListener( delegate { ClickArmorToggleButton( 2 ); } );

    }

    private void PopulateTopBar() {
        characterName.text = character.name;
        characterLevel.text = "Lv. " + character.level.ToString();

        switch (character.quality) {
            case 1:
                greenToggle.GetComponent<Button>().image.color = setColor;
                break;
            case 2:
                blueToggle.GetComponent<Button>().image.color = setColor;
                break;
            case 3:
                purpleToggle.GetComponent<Button>().image.color = setColor;
                break;
        }

        switch (character.armor) {
            case 0:
                lightToggle.GetComponent<Button>().image.color = setColor;
                break;
            case 1:
                mediumToggle.GetComponent<Button>().image.color = setColor;
                break;
            case 2:
                heavyToggle.GetComponent<Button>().image.color = setColor;
                break;
        }
    }

    public void ClickQualityToggleButton(int quality) {

        Color32 curColor = EventSystem.current.currentSelectedGameObject.GetComponent<Button>().image.color;

        if ( curColor.r == 220 ) { // unset all of the buttons and make quality common
            EventSystem.current.currentSelectedGameObject.GetComponent<Button>().image.color = unsetColor;
            character.quality = 0;
        } else {

            UnsetPreviousQualityButton();
            EventSystem.current.currentSelectedGameObject.GetComponent<Button>().image.color = setColor;
            character.quality = quality;

        }

    }
    
    public void ClickArmorToggleButton( int armor ) {
        UnsetPreviousArmorButton();
        EventSystem.current.currentSelectedGameObject.GetComponent<Button>().image.color = setColor;
        character.armor = armor;
    }

    private void UnsetPreviousQualityButton() {
        switch (character.quality) {
            case 1:
                greenToggle.GetComponent<Button>().image.color = unsetColor;
                break;
            case 2:
                blueToggle.GetComponent<Button>().image.color = unsetColor;
                break;
            case 3:
                purpleToggle.GetComponent<Button>().image.color = unsetColor;
                break;
        }
    }

    private void UnsetPreviousArmorButton() {
        switch (character.armor) {
            case 0:
                lightToggle.GetComponent<Button>().image.color = unsetColor;
                break;
            case 1:
                mediumToggle.GetComponent<Button>().image.color = unsetColor;
                break;
            case 2:
                heavyToggle.GetComponent<Button>().image.color = unsetColor;
                break;
        }
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
