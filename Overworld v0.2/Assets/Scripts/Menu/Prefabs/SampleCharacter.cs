using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SampleCharacter : MonoBehaviour {

    public TextMeshProUGUI name;
    public TextMeshProUGUI level;
    public TextMeshProUGUI weapon;
    public TextMeshProUGUI armor;
    public int partyId;
    public Color quality;
    public PlayerPartyData party;
    public int characterId;


    private UICharacter character;
    private ScrollParty scrollParty;
    private CharactersLoader charactersLoader;

    public void Setup(UICharacter currentCharacter, ScrollParty currentScrollParty, PlayerPartyData currentParty) {
        character = currentCharacter;
        name.text = character.name;
        level.text = character.level;
        weapon.text = character.weapon;
        armor.text = character.armor;
        quality = character.quality;
        weapon.color = quality;
        armor.color = quality;
        partyId = character.partyId;
        party = currentParty;
        characterId = character.id;
        scrollParty = currentScrollParty;
        gameObject.GetComponent<Button>().onClick.RemoveAllListeners();
        gameObject.GetComponent<Button>().onClick.AddListener( SwapPartyCharacter );

    }
    public void Setup( UICharacter currentCharacter, CharactersLoader currentCharactersLoader ) {
        character = currentCharacter;
        name.text = character.name;
        level.text = character.level;
        weapon.text = character.weapon;
        armor.text = character.armor;
        quality = character.quality;
        weapon.color = quality;
        armor.color = quality;
        charactersLoader = currentCharactersLoader;

    }
    // todo add the fetch data stuff, get the weapons and format the character data in here
    public void SwapPartyCharacter() {
        PlayerPartyData partyToChange = party;
        DataLoader dataLoader = new DataLoader();
        DataController data = dataLoader.LoadData();
        data.SetCharacterSwap( partyToChange, characterId );
        data.SetPreviousPage( 3 );
        SceneLoader sceneLoader = new SceneLoader();
        sceneLoader.CharactersMenu();
    }
}
