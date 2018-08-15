﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// This class is responsible for all turn management, and switches between Characters
public class TurnController : MonoBehaviour {

    #region Create Singleton
    private static TurnController _instance;

    public static TurnController Instance {
        get {
            if (_instance == null) {
                GameObject go = new GameObject( "TurnController" );
                go.AddComponent<TurnController>();
            }
            return _instance;
        }
    }

    void Awake() {
        _instance = this;
    }
    #endregion

    #region Turn Management

    public enum TurnState { BeginningOfTurn, Movement, Attack, EndOfTurn, Standby };
    public static TurnState State { get; set; }
    public static int turn = 0;
    public static int ActionPoints { get; set; }

    // Inifinitely loop through Character turns
    public IEnumerator HandleTurnState() {
        State = TurnState.BeginningOfTurn;
        while (true) {
            if (State == TurnState.BeginningOfTurn) {
                BeginTurn();
            }
            else if (State == TurnState.Standby) {
                Standby();
            }
            else if (State == TurnState.Movement) {
                Movement();
            }
            else if (State == TurnState.Attack) {
                Attack();
            }
            else if (State == TurnState.EndOfTurn) {
                EndTurn();
            }
            yield return null;
        }
    }

    // Called when a player clicks an 'Ability' Button
    private void Attack() {
        // Displays required ability reticle
        switch (AbilityController.Reticle) {
            case AbilityController.ReticleType.Diamond:
                TileController.Instance.DisplayRange();
                TileController.Instance.RenderReticle();
                ReticleController.Instance.ConfirmLocation();
                break;
            case AbilityController.ReticleType.Square:
                TileController.Instance.DisplayRange();
                TileController.Instance.RenderReticle();
                ReticleController.Instance.ConfirmLocation();
                break;
        }
    }

    // Called when the Character is not performing an action
    private void Standby() {
        if(NavigationController.MovementRemaining == 0) {
            GameObject.Find( "Move Button" ).GetComponent<Button>().interactable = false;
        }
        Debug.Log( "ActionPoints Remaining: " + ActionPoints );
        Debug.Log( "SP Gage: " + EntityManager.Entities[TurnController.turn].GetComponent<Character>().SpGage() );
    }

    // Called when the user presses the 'Move' Button, awaits input and ensures that the selected tile is in the movement area
    private void Movement() {
        if (Input.GetMouseButtonDown( 0 ) && NavigationController.navTiles.Contains(InputController.CursorPositionInt) && !EventSystem.current.IsPointerOverGameObject()) {
            NavigationController.Instance.Move( InputController.CursorPosition );
        } else if (Input.GetMouseButtonDown( 1 ) || Input.GetMouseButtonDown( 0 ) && EventSystem.current.IsPointerOverGameObject()) {
            NavigationController.Instance.ReInit();
            State = TurnState.Standby;
        }
    }

    // Re-Initialize Character variables at the start of a new Character turn
    private void BeginTurn() {
        if (EntityManager.Entities[turn].dead) {
            EndTurn();
        }
        UIController.Instance.ResetAbilities();

        foreach(SampleAbility ab in EntityManager.Entities[turn].abilityBar.GetComponentsInChildren<SampleAbility>()) {
            ab.currentCooldown -= 1;
        }
        
        ActionPoints = 8;
        GameObject.Find( "Move Button" ).GetComponent<Button>().interactable = true;
        EntityManager.Entities[turn].tag = "Player";
        NavigationController.Instance.Init(EntityManager.Entities[turn].gameObject);
        EntityManager.Entities[turn].GetComponent<Character>().StatusEffectsActivate();
        // Init other controllers
        EntityManager.Entities[turn].GetComponent<Character>().healthDisplay.GetComponent<Outline>().effectColor = new Color32( 97, 41, 152, 255 );

        UIController.Show( EntityManager.Entities[turn].abilityBar.GetComponent<CanvasGroup>() );
        UIController.Show( GameObject.FindWithTag( "Standbybar" ).GetComponent<CanvasGroup>() );
        State = TurnState.Standby;
    }

    // Clear all tiles for the next Character turn
    private void EndTurn() {
        // Clear focus on entity
        ActionPoints = 0;
        EntityManager.Entities[turn].GetComponent<Character>().EndOfTurnStatusEffects();
        EntityManager.Entities[turn].GetComponent<Character>().healthDisplay.GetComponent<Outline>().effectColor = new Color32( 97, 41, 152, 0 );
        // Clear all tile colors

        // Check if these are needed
        AbilityController.Reticle = AbilityController.ReticleType.None;
        NavigationController.Instance.ReInit();
        NavigationController.Instance.Init( EntityManager.Entities[turn].gameObject );

        UIController.Hide( EntityManager.Entities[turn].abilityBar.GetComponent<CanvasGroup>() );


        // Increment turn counter
        EntityManager.Entities[turn].tag = "Ally";
        if (turn != EntityManager.Entities.Count - 1) {
            turn += 1;
        }
        else {
            turn = 0;
        }
        State = TurnState.BeginningOfTurn;
    }
    #endregion
}
