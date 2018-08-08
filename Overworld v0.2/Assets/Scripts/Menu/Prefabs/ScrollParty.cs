using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class UICharacter {
    public int id;
    public string name;
    public string level;
    public string weapon;
    public string armor;
    public int partyId;
    public Color quality;
}

public class ScrollParty : MonoBehaviour {

    // Vars from Parent
    public int partyId;
    public string partyName;
    public bool selected;
    private List<UICharacter> characters;
    private PlayerPartyData party;
    public PartiesMenu partiesMenu;
    public PlayerPartyData[] playerPartyData;

    // Vars for children
    public Transform contentPanel;
    public ScrollParty otherParty;
    public SimpleObjectPool characterObjectPool;
    

    private void RefreshDisplay( List<UICharacter> characters, PlayerPartyData playerPartyData ) {
        AddCharacters( characters, playerPartyData );
    }
    private void AddCharacters( List<UICharacter> characters, PlayerPartyData playerPartyData ) {
        // will need to add empty slots if there are empty characters
        for (int i = 0; i < characters.Count; i++) {
            UICharacter character = characters[i];
            GameObject newCharacter = GameObject.Find("CharacterObjectPool").GetComponent<SimpleObjectPool>().GetObject();
            newCharacter.transform.SetParent( contentPanel );

            SampleCharacter sampleCharacter = newCharacter.GetComponent<SampleCharacter>();
            sampleCharacter.Setup( character, this, playerPartyData );
        }
    }


    public void Setup( UIParty currentParty, PartiesMenu currentPartiesMenu, List<UICharacter> characters, PlayerPartyData[] playerParties ) {
        playerPartyData = playerParties;
        foreach(UICharacter character in characters) {
            character.partyId = currentParty.partyId;
        }
        foreach(PlayerPartyData playerParty in playerPartyData) {
            if(currentParty.partyId == playerParty.id) {
                party = playerParty;
            }
        }
        RefreshDisplay( characters, party );
        AppendDataToButtons( currentParty.partyId );

        // we have the party ID at this point

        partiesMenu = currentPartiesMenu;
    }

    private void AppendDataToButtons(int partyId) {
        foreach (PartyButtonHandler obj in gameObject.GetComponentsInChildren<PartyButtonHandler>()) {
            obj.partyId = partyId;
            string stringId = partyId.ToString();
            PlayerPartyData playerParty;
            foreach(PlayerPartyData p in playerPartyData) {
                if(p.id == partyId) {
                    playerParty = p;
                    gameObject.GetComponentInChildren<PartyButtonHandler>().party = playerParty;
                }
            }
        }
    }
}