using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TalentTreeLoader : MonoBehaviour {

    private class UITalent {
        public bool selected;
        public MenuTalentData talent;
    }

    private DataLoader dataLoader;
    private DataController data;

    private int talentTreeId;
    private int characterId;
    private int selectedTreeTalents;
    private int selectedTalents = 0;
    private MenuTalentTreeData talentTree;
    private PlayerCharacterData character;
    private List<MenuTalentData> characterTalents;

	// Use this for initialization
	void Start () {

        dataLoader = new DataLoader();
        data = dataLoader.LoadData();
        // TODO Fetch available talent trees and populate dropdown menu
        foreach(MenuTalentTreeData tree in data.GetTalentTreeData()) {
            GameObject.Find( "TalentTreeDropdown" ).GetComponent<TMP_Dropdown>().options.Add(new TMP_Dropdown.OptionData(tree.name));
        }
        GameObject.Find( "TalentTreeDropdown" ).GetComponent<TMP_Dropdown>().value = 0  ;

        // Temporary
        talentTreeId = 0; // TODO replace with current dropdown value
        characterId = 2; // TODO replace with { data.GetEditCharacter() }
        
        character = data.GetCharacter( characterId );
        talentTree = data.GetTalentTree( talentTreeId );
        characterTalents = character.talents;

        // Temporary
        LoadNewTree();

	}

    // Repsonsible for loading page data
    private void LoadNewTree() {
        talentTreeId = 0; // TODO replace with current dropdown value
        selectedTreeTalents = 0;

        // TODO count character talents in this talent tree


        for(int i=0; i<talentTree.subTrees.Length; i++){
            PopulateSubTree(talentTree.subTrees[i], i);
        }
    }

    // Adds talents to the respective sub trees
    private void PopulateSubTree(MenuSubTreeData subTree, int subTreeIndex) {
        for(int i=0; i<subTree.talents.Length; i++){ // (MenuTalentData talent in subTree.talents) {
            UITalent currentTalent = new UITalent {
                talent = subTree.talents[i]
            };

            Debug.Log( "Character Talents Count: " + characterTalents.Count );
            // This is the only spot that didnt work when i blind coded this whole thing
            if (characterTalents.Exists( x => x.name == subTree.talents[i].name )) {
                Debug.Log( "Selected: " + subTree.talents[i].name );
                currentTalent.selected = true;
                selectedTreeTalents += 1;
            } else {
                currentTalent.selected = false;
            }
            // end of the spot

            DisplayTalents( currentTalent, subTreeIndex, i, subTree.talents[i] );

        }
    }

    // Visually updates which talents are unlocked by this character
    private void DisplayTalents( UITalent currentTalent, int subTreeIndex, int talentIndex, MenuTalentData talentData ) {
        // Do something with currentTalent to add it to the page, color it differently if its selected, ect
        GameObject talentButton = GetTalentButton( subTreeIndex, talentIndex );
        talentButton.AddComponent<TalentButtonData>(); // TODO for 'locking' talents behind their prerequisites
        talentButton.GetComponentInChildren<TextMeshProUGUI>().text = currentTalent.talent.name;
        talentButton.GetComponent<Button>().onClick.AddListener( ClickTalent );
        talentButton.GetComponent<TalentButtonData>().talentData = talentData;

        if (currentTalent.selected) {
            talentButton.GetComponent<Outline>().effectColor = new Color32(169, 87, 202, 255);
            talentButton.GetComponent<TalentButtonData>().selected = true;
        }
        
    }

    public void ClickTalent() {
        bool selected = EventSystem.current.currentSelectedGameObject.GetComponent<TalentButtonData>().selected;
        MenuTalentData talent = EventSystem.current.currentSelectedGameObject.GetComponent<TalentButtonData>().talentData;
        // After all above shit { remove or add talent to characterTalents }

        if (!selected) {
            character.talents.Add( talent );
            EventSystem.current.currentSelectedGameObject.GetComponent<Outline>().effectColor = new Color32( 169, 87, 202, 255 );
        } else {
            character.talents.Remove( talent );
            EventSystem.current.currentSelectedGameObject.GetComponent<Outline>().effectColor = new Color32( 222, 222, 222, 255 );
        }

        data.SaveCharacterTalents( character );
    }

    public void SwapTree() {
        Debug.Log( "SwapTree()" );
    }

    private GameObject GetTalentButton(int subTreeIndex, int talentIndex) {
        // math to find the desired talent object to display on
        string talentObject = "Talent " + ((talentIndex + 1) + (subTreeIndex * 8)).ToString();
        return GameObject.Find(talentObject);
    }

}
