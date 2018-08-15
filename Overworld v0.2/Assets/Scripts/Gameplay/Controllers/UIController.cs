using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// This class is responsible for managing all UI elements
public class UIController : MonoBehaviour {

    #region Create Singleton
    private static UIController _instance;

    public static UIController Instance {
        get {
            if (_instance == null) {
                GameObject go = new GameObject( "UIController" );
                go.AddComponent<UIController>();
            }
            return _instance;
        }
    }

    void Awake() {
        _instance = this;
    }
    #endregion

    private CanvasGroup actionbar;
    private CanvasGroup standybar;

    public void Setup() {
        actionbar = GameObject.FindWithTag( "Actionbar" ).GetComponent<CanvasGroup>();
        standybar = GameObject.FindWithTag( "Standbybar" ).GetComponent<CanvasGroup>();
        GameObject.FindWithTag( "MoveButton" ).GetComponent<Button>().onClick.AddListener( MoveButton );
        GameObject.FindWithTag( "AttackButton" ).GetComponent<Button>().onClick.AddListener( AttackButton );
        GameObject.FindWithTag( "EndButton" ).GetComponent<Button>().onClick.AddListener( EndButton );
        GameObject.FindWithTag( "ReturnButton" ).GetComponent<Button>().onClick.AddListener( ReturnButton );
    }

    // Hides a CanvasGroup
    public static void Hide( CanvasGroup canvasGroup ) {
        canvasGroup.alpha = 0f; //this makes everything transparent
        canvasGroup.blocksRaycasts = false; //this prevents the UI element to receive input events
    }
    // Shows a CanvasGroup
    public static void Show( CanvasGroup canvasGroup ) {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
    }

    // Method called when 'Move' Button is pressed
    public void MoveButton() {
        NavigationController.Instance.ReInit();
        if ( NavigationController.MovementRemaining > 0 ) {
            NavigationController.Instance.CalcMoveArea();
            TurnController.State = TurnController.TurnState.Movement;
        }
    }

    // Method called when 'Attack' Button is pressed
    public void AttackButton() {
        CheckAbilities();
        Hide( standybar );
        Show( actionbar );    
    }

    // Method called when the user presses the 'Return' button on the ability bar 
    public void ReturnButton() {
        TileController.Instance.ClearTiles( ReticleController.CastArea );
        Hide( actionbar );
        Show( standybar );
    }

    // Method called when the user presses the 'End' button, switches to the next players turn
    public void EndButton() {
        TurnController.State = TurnController.TurnState.EndOfTurn;
    }

    public void CheckAbilities() {
        GameObject abilityBar = EntityManager.Entities[TurnController.turn].abilityBar;
        foreach (Button ability in abilityBar.GetComponentsInChildren<Button>()) {
            int cost = ability.GetComponent<SampleAbility>().cost;
            if (cost > TurnController.ActionPoints) {
                ability.interactable = false;
            }
        }
    }

    public void ResetAbilities() {
        GameObject abilityBar = EntityManager.Entities[TurnController.turn].abilityBar;
        foreach (Button ability in abilityBar.GetComponentsInChildren<Button>()) {
            ability.interactable = true;
        }
    }
}
