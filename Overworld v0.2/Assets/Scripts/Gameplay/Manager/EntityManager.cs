using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// Class responsible for managing Character entities
// Probably should use this more in the Controllers TODO
public class EntityManager : MonoBehaviour {
    #region Create Singleton
    private static EntityManager _instance;

    public static EntityManager Instance {
        get {
            if(_instance == null) {
                GameObject go = new GameObject( "EntityManager" );
                go.AddComponent<EntityManager>();
            }
            return _instance;
        }
    }

    void Awake() {
        _instance = this;
    }
    #endregion

    public static List<Character> Entities;

    // Initializes the entities list and assignes Characters their respective Ability Bars
    public void Setup() {
        Entities = new List<Character>();
        List<Character> characters = FindObjectsOfType<Character>().ToList();
        Entities = characters.OrderByDescending( o => o.initiative ).ToList();
        foreach (GameObject abilityBar in GameObject.FindGameObjectsWithTag( "AbilityBar" )) {
            for(int i=0; i < Entities.Count; i++) {
                if (Entities[i].entityIdentifier == abilityBar.GetComponent<SampleAbilityBar>().entityIdentifier) {
                    // logic for deciding ally or enemy here later TODO
                    Entities[i].tag = "Ally";
                    Entities[i].abilityBar = abilityBar;
                    break;
                }
            }
        }
        // After Entities are initialized begin the game
        StartCoroutine( TurnController.Instance.HandleTurnState() );
    }
}
