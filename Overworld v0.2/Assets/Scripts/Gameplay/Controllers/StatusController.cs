using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusController : MonoBehaviour {

    #region Create Singleton
    private static StatusController _instance;

    public static StatusController Instance {
        get {
            if (_instance == null) {
                GameObject go = new GameObject( "StatusController" );
                go.AddComponent<StatusController>();
            }
            return _instance;
        }
    }

    void Awake() {
        _instance = this;
    }
    #endregion

                         
    public enum StatusType { VoT/*Value over Time*/, Incapacitate, Stun, Stat, Invisibility } // { 0, 1, 2, 3, 4 } 
    public enum EffectType { Debuff, Buff } // { 0, 1 }
    public enum TurnPosition { Start, End }
    public enum SchoolType { Physical, Magical }

    public static TurnPosition Turn { get; set; }

    private StatusType Type;
    private EffectType Effect;
    private string statusName;
    private string description;
    private int priority;
    private float value;
    private int source;
    private Status currentStatus;

    delegate void StealthDelegate();
    StealthDelegate handleStealth;

    /* 
     * For additional status effects, because some Status Effects will do multiple things.
     * I feel like hiding an additional Status Effect inside of a status effect will be the
     * best way to deal with this. 
     * (Example: Stun for 2 rounds {Status Effect #1}, and 50 Damage per round {Status Effect #2 Hidden})
     */
    private bool hidden = false; 

    public void Init(Status status) {

        if (status.hidden) {
            statusName = status.statusName;
            description = status.description;
            hidden = true;
        }
        source = status.sourceCharacter;
        priority = status.priority;
        value = status.value;
        SetStatusType( status.statusType );
        SetEffectType( status.effectType );
        currentStatus = status;
        Activate();
        
    }

    private void SetStatusType( int type ) {
        switch (type) {
            case 0:
                Type = StatusType.VoT;
                break;
            case 1:
                Type = StatusType.Incapacitate;
                break;
            case 2:
                Type = StatusType.Stun;
                break;
            case 3:
                Type = StatusType.Stat;
                break;
            case 4:
                Type = StatusType.Invisibility;
                break;
        }
    }

    private void SetEffectType( int type ) {
        switch (type) {
            case 0:
                Effect = EffectType.Debuff;
                break;
            case 1:
                Effect = EffectType.Buff;
                break;
        }
    }


    // Activate() calls one of the below methods depending on the Init() variables above  
    private void Activate() {
        switch (Type) {
            case StatusType.VoT:
                ValueOverTime();
                break;
            case StatusType.Incapacitate:
                Incapacitate();
                break;
            case StatusType.Stun:
                Stun();
                break;
            case StatusType.Stat:
                Stat();
                break;
            case StatusType.Invisibility:
                Invisibility();
                break;
        }
    }

    // StatusType.VoT, uses EffectType for { Buff: value stays positive }, { Debuff: value becomes negative }
    private void ValueOverTime() {
        if (Effect == EffectType.Debuff) {
            Finish( statusName, false );
            EntityManager.Entities[TurnController.turn].TakeDamage( value, source );
        } else {
            Finish( statusName, true );
            EntityManager.Entities[TurnController.turn].TakeDamage( -value, source );
        }            
    }
    
    // StatusType.Incapacitate
    private void Incapacitate() { 
        // Not used at this time
    }

    // StatusType.Stun
    private void Stun() {
        if (Effect == EffectType.Debuff) {
            TurnController.ActionPoints = 0;
        }
    }

    // StatusType.Stat, uses EffectType to determine positive or negative { int value }
    private void Stat() {
        // Hardcoding as movement speed for now TODO
        if (Effect == EffectType.Debuff) {
            NavigationController.MovementRemaining = 0;
        }
    }

    #region Invisibility
    // StatusType.Invisibility, uses EffectType for { Buff = Make invisible }, { Debuff = reveal invisible / prevent invisible }
    private void Invisibility() {
        bool positiveEffect;
        if(Effect == EffectType.Buff) {

            if (Turn == TurnPosition.Start) { // reveal the character on their turn
                handleStealth = RevealCharacter;
                positiveEffect = false;
            } else if ( Turn == TurnPosition.End && EntityManager.Entities[TurnController.turn].revealed == false ) { // hide the character after their turn
                handleStealth = StealthCharacter;
                positiveEffect = true;
            } else {
                // They are revealed and can not become invisible
                EntityManager.Entities[TurnController.turn].RemoveStatus( currentStatus );
                EntityManager.Entities[TurnController.turn].RemoveEndOfTurnStatus( currentStatus );
                handleStealth = RevealCharacter;
                positiveEffect = false;
            }

        } else {

            EntityManager.Entities[TurnController.turn].revealed = true;
            handleStealth = RevealCharacter;
            positiveEffect = false;

        }
        Finish( "Invisibility", positiveEffect );
        
    }

    public void StealthCharacter() { // TODO animation
        foreach (SpriteRenderer renderer in EntityManager.Entities[TurnController.turn].GetComponentsInChildren<SpriteRenderer>()) {
            renderer.enabled = false;
        }
        EntityManager.Entities[TurnController.turn].GetComponentInChildren<Canvas>().enabled = false;
    }

    public void RevealCharacter() { // TODO animation
        foreach (SpriteRenderer renderer in EntityManager.Entities[TurnController.turn].GetComponentsInChildren<SpriteRenderer>()) {
            renderer.enabled = true;
        }
        EntityManager.Entities[TurnController.turn].GetComponentInChildren<Canvas>().enabled = true;
    }

    public void Finish( string text, bool positiveEffect ) {
        if (positiveEffect) {
            EntityManager.Entities[TurnController.turn].combatText.color = new Color32( 3, 204, 0, 255 ); // Green
            EntityManager.Entities[TurnController.turn].combatText.text = text;
        }
        else {
            EntityManager.Entities[TurnController.turn].combatText.color = new Color32( 255, 30, 30, 255 ); // Red
            EntityManager.Entities[TurnController.turn].combatText.text = text;
            if (Type == StatusType.Invisibility) // Must reveal before displaying text if invisibility
                handleStealth();
        }
        EntityManager.Entities[TurnController.turn].combatText.canvasRenderer.SetAlpha( 1.0f );
        EntityManager.Entities[TurnController.turn].combatText.CrossFadeAlpha( 0.0f, 2.0f, false );

        if (positiveEffect && Type == StatusType.Invisibility) // Only hide after displaying text if invisibility
            handleStealth();
    }
    #endregion
}
