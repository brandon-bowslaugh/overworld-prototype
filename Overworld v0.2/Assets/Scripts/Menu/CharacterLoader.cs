using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterLoader : MonoBehaviour {

    private int characterId;

	// Use this for initialization
	void Start () {
        
        DataLoader dataLoader = new DataLoader();
        DataController data = dataLoader.LoadData();

        data.SetEditCharacter( 3 ); // TEMP TODO

        characterId = data.GetEditCharacter();

	}
	
    public void TalentTree() {

        SceneLoader sceneLoader = new SceneLoader();
        sceneLoader.TalentTree();

    }

}
