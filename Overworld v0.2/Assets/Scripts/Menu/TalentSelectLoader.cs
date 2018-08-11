using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalentSelectLoader : MonoBehaviour {

    private DataLoader dataLoader;
    private DataController data;

    private int swapType;
    private int slotId;
    private PlayerCharacterData character;

    private string subTree;
    private string talentTree;

    public SimpleObjectPool talentSelectObjectPool = new SimpleObjectPool();

    // Use this for initialization
    void Start () {

        dataLoader = new DataLoader();
        data = dataLoader.LoadData();


        swapType = data.GetEditType(); 
        slotId = data.GetEditSlot();
        character = data.GetCharacter( data.GetEditCharacter() );
        Debug.Log( data.GetEditCharacter() );
        PopulateTalentSelect();

    }

    private void PopulateTalentSelect() {

        foreach (MenuTalentData talent in character.talents) {
            if(talent.type == swapType) {
                FindTreeNames( talent.subTree );
                GameObject newTalentSelect = talentSelectObjectPool.GetObject();
                newTalentSelect.transform.SetParent( GameObject.Find( "AbilityContainer" ).transform );
                TalentSelect talentSelect = newTalentSelect.GetComponent<TalentSelect>();
                talentSelect.Setup( talent, this, talentTree, subTree );
            }
            
        }

    }

    private void FindTreeNames(int id) {

        foreach(MenuTalentTreeData tree in data.GetTalentTreeData()) {
            if (FindSubTreeName(tree, id)) {
                talentTree = tree.name;
            }
        }

    }

    private bool FindSubTreeName(MenuTalentTreeData tree, int id) {

        foreach(MenuSubTreeData sub in tree.subTrees) {

            if(sub.id == id) {
                subTree = sub.name;
                return true;
            }

        }

        return false;

    }

    public void SelectTalent(int type, MenuTalentData saveTalent) {
        switch (type) {
            case 0:
                character.selectedTalents.abilities[slotId] = saveTalent;
                break;
            case 1:
                character.selectedTalents.passives[slotId] = saveTalent;
                break;
            case 3:
                character.selectedTalents.ultimate = saveTalent;
                break;
        }

        data.SaveCharacter( character );
        SceneLoader sceneLoader = new SceneLoader();
        sceneLoader.CharacterMenu();
    }

}
