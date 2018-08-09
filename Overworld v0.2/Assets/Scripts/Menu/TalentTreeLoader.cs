using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

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
    private List<GameObject> coloredTalents;

	// Use this for initialization
	void Start () {
        
        coloredTalents = new List<GameObject>();
        dataLoader = new DataLoader();
        data = dataLoader.LoadData();
        // TODO Fetch available talent trees and populate dropdown menu
        foreach(MenuTalentTreeData tree in data.GetTalentTreeData()) {
            GameObject.Find( "TalentTreeDropdown" ).GetComponent<TMP_Dropdown>().options.Add(new TMP_Dropdown.OptionData(tree.name));
        }
        // Dropdown bug is solved by this mess
        GameObject.Find( "TalentTreeDropdown" ).GetComponent<TMP_Dropdown>().value = 1;
        GameObject.Find( "TalentTreeDropdown" ).GetComponent<TMP_Dropdown>().value = 0;
        GameObject.Find( "TalentTreeDropdown" ).GetComponent<TMP_Dropdown>().onValueChanged.AddListener( delegate { SwapTree(); } );
        // Temporary
        characterId = 2; // TODO replace with { data.GetEditCharacter() }
        
        character = data.GetCharacter( characterId );
        characterTalents = character.talents;

        // Temporary
        LoadNewTree();

	}

    // Repsonsible for loading page data
    private void LoadNewTree() {

        talentTreeId = GameObject.Find("TalentTreeDropdown").GetComponent<TMP_Dropdown>().value;
        selectedTreeTalents = 0;
        talentTree = data.GetTalentTree( talentTreeId );
        
        for (int i=0; i<talentTree.subTrees.Length; i++){
            PopulateSubTree(talentTree.subTrees[i], i);
        }

        GameObject.Find( "ThisTreeTalents" ).GetComponent<TextMeshProUGUI>().text = talentTree.name + " Talents: " + selectedTreeTalents;

    }

    // Adds talents to the respective sub trees
    private void PopulateSubTree(MenuSubTreeData subTree, int subTreeIndex) {

        switch (subTreeIndex) {
            case 0:
                GameObject.Find("Subtree1Name").GetComponent<TextMeshProUGUI>().text = subTree.name;
                break;
            case 1:
                GameObject.Find( "Subtree2Name" ).GetComponent<TextMeshProUGUI>().text = subTree.name;
                break;
            case 2:
                GameObject.Find( "Subtree3Name" ).GetComponent<TextMeshProUGUI>().text = subTree.name;
                break;
        }

        for(int i=0; i<subTree.talents.Length; i++){ // (MenuTalentData talent in subTree.talents) {
            UITalent currentTalent = new UITalent {
                talent = subTree.talents[i]
            };
            
            if (characterTalents.Exists( x => x.name == subTree.talents[i].name )) {
                currentTalent.selected = true;                
                selectedTreeTalents += 1;
            } else {
                currentTalent.selected = false;
            }

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
            coloredTalents.Add( talentButton );
        }
        
    }

    public void ClickTalent() {
        bool selected = EventSystem.current.currentSelectedGameObject.GetComponent<TalentButtonData>().selected;
        MenuTalentData talent = EventSystem.current.currentSelectedGameObject.GetComponent<TalentButtonData>().talentData;
        // After all above shit { remove or add talent to characterTalents }

        if (!selected) {
            character.talents.Add( talent );
            coloredTalents.Add( EventSystem.current.currentSelectedGameObject );
            EventSystem.current.currentSelectedGameObject.GetComponent<Outline>().effectColor = new Color32( 169, 87, 202, 255 );
        } else {
            character.talents.Remove( talent );
            EventSystem.current.currentSelectedGameObject.GetComponent<Outline>().effectColor = new Color32( 222, 222, 222, 255 );
        }

        data.SaveCharacterTalents( character );
    }

    public void SwapTree() {

        // Clear selected colors
        foreach(GameObject button in coloredTalents) {
            button.GetComponent<Outline>().effectColor = new Color32( 222, 222, 222, 255 );
        }
        coloredTalents.Clear();

        // Load new tree
        LoadNewTree();

    }

    private GameObject GetTalentButton(int subTreeIndex, int talentIndex) {
        // math to find the desired talent object to display on
        string talentObject = "Talent " + ((talentIndex + 1) + (subTreeIndex * 8)).ToString();
        return GameObject.Find(talentObject);
    }

}
