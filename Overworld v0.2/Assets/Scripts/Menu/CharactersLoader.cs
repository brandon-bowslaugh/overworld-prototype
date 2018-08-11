using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharactersLoader : MonoBehaviour {

    // Use this for initialization
    DataFormat dataFormat;
    DataController data;

    void Start () {
        DataLoader dataLoader = new DataLoader(); 
        dataFormat = new DataFormat();
        data = dataLoader.LoadData();
        AddCharacters( data.GetAllPlayerCharacterData() );
    }

    private void AddCharacters( PlayerCharacterData[] characters ) {
        // will need to add empty slots if there are empty characters
        for (int i = 0; i < characters.Length; i++) {
            int charId = characters[i].id;
            int slot1 = -1;
            int slot2 = -1;
            int slot3 = -1;
            int slot4 = -1;

            if (data.GetPreviousPage() == 3) {
                PlayerPartyData party = data.GetPartyToEdit();
                slot1 = party.slotOneCharacterId;
                slot2 = party.slotTwoCharacterId;
                slot3 = party.slotThreeCharacterId;
                slot4 = party.slotFourCharacterId;
                GameObject.Find( "Backbutton" ).GetComponent<Button>().onClick.RemoveAllListeners();
                GameObject.Find( "Backbutton" ).GetComponent<Button>().onClick.AddListener( delegate { new SceneLoader().PartiesMenu(); } );
            }
            if (charId != slot1 && charId != slot2 && charId != slot3 && charId != slot4) { 
                UICharacter character = dataFormat.CreateUICharacter( characters[i], data );
                GameObject newCharacter = GameObject.Find( "CharacterObjectPool" ).GetComponent<SimpleObjectPool>().GetObject();
                newCharacter.transform.SetParent( GameObject.Find( "CharacterContainer" ).transform );
                newCharacter.GetComponent<Button>().interactable = true;
                newCharacter.GetComponent<Button>().onClick.RemoveAllListeners();
                if (data.GetPreviousPage() == 3) {
                    newCharacter.GetComponent<Button>().onClick.AddListener( delegate { SelectPartyCharacter( charId ); } );
                } else {
                    newCharacter.GetComponent<Button>().onClick.AddListener( delegate { EditPartyCharacter( charId ); } );
                }

                SampleCharacter sampleCharacter = newCharacter.GetComponent<SampleCharacter>();
                sampleCharacter.Setup( character, this );
            }
        }
    }

    private void SelectPartyCharacter(int characterId) {
        data.SwapCharacterInParty(data.GetPartyToEdit(), data.GetCharacterToSwap(), characterId);
        SceneLoader sceneLoader = new SceneLoader();
        sceneLoader.PartiesMenu();
    }

    private void EditPartyCharacter(int id) {
        data.SetEditCharacter( id );
        SceneLoader sceneLoader = new SceneLoader();
        sceneLoader.CharacterMenu();
    }

    public void AddNewCharacter() {
        if(data.GetPreviousPage() == 3) {
            data.AddNewCharacterToParty();
        } else {
            data.AddNewCharacter();
        }
        SceneLoader sceneLoader = new SceneLoader();
        sceneLoader.CharacterMenu();
    }

}
