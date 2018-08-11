using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddNewParty : MonoBehaviour {
    // This is placeholder to get to the testing phase. Complete rewrite
    public void CreateNewTestParty() {

        DataLoader dataLoader = new DataLoader();
        DataController data = dataLoader.LoadData();
        PlayerPartyData testParty = new PlayerPartyData();

        testParty.name = "Temporary Party " + data.GetAllPlayerParties().Length;
        for(int i=0; i<4; i++) {
            data.AddNewCharacter();
        }
        testParty.slotOneCharacterId = data.GetAllPlayerCharacterData().Length - 4;
        testParty.slotTwoCharacterId = data.GetAllPlayerCharacterData().Length - 3; 
        testParty.slotThreeCharacterId = data.GetAllPlayerCharacterData().Length - 2;
        testParty.slotFourCharacterId = data.GetAllPlayerCharacterData().Length - 1;
        data.CreateTestParty(testParty);

        SceneLoader sceneLoader = new SceneLoader();
        sceneLoader.ReloadScene();

    }


}
