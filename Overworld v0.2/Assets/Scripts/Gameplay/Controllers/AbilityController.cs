using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// This class is responsible for controlling the currently selected ability. 
// Works in conjunction with ReticleController for casting abilities
public class AbilityController : MonoBehaviour {

    #region Create Singleton
    private static AbilityController _instance;

    public static AbilityController Instance {
        get {
            if (_instance == null) {
                GameObject go = new GameObject( "AbilityController" );
                go.AddComponent<AbilityController>();
            }
            return _instance;
        }
    }

    void Awake() {
        _instance = this;
    }
    #endregion

    // Public Variables
    public enum ReticleType { None, Diamond, Square, Cone };
    public enum AbilityType { Damage, Healing, Dispell, Dash, Teleport, StunBreak, SnareBreak, SlowBreak, Swap, Appearance, Tile, Ressurect, SP, Execute, MindControl, Consume }; 
    public enum TargetType { Any, Enemy, Ally, Self, Corpse, Ground };
    public static int CastRange { get; private set; }
    public static AbilityType Type { get; private set; } // 0 is damage, 1 is healing, 2 is mobility
    public static ReticleType Reticle { get; set; } // 0 is none, 1 is diamond, 2 is square
    public static int XAxis { get; private set; }
    public static int YAxis { get; private set; } // unused at this time, will be required for some abilities later
    public static int Cost { get; private set; }
    private SampleAbility button;

    // Local Variables
    float abilityValue; // damage or healing number
    float secondaryValue;
    TargetType targetType; // 0 all, 1 is enemy, 2 is ally, 3 is self
    private Status status;

    // For initializing the selected ability
    public void Init(BattleAbility ability, SampleAbility btn) {
        SetTargetType( ability.targetType );
        SetAbilityType( ability.abilityType );
        secondaryValue = ability.secondaryValue;
        CastRange = ability.range;
        SetAbilityDamage( ability.value );
        XAxis = ability.xAxis;
        SetReticleType( ability.reticleType );
        if (ability.status != null) {
            status = ability.status;
        } else {
            status = null;
        }
        button = btn;
        Cost = ability.cost;
        TurnController.State = TurnController.TurnState.Attack;

    }

    // Initializes enum ReticleType Reticle
    private void SetReticleType( int type ) {
        if(Cost <= TurnController.ActionPoints) {
            switch (type) {
                case 0:
                    Reticle = ReticleType.None;
                    break;
                case 1:
                    Reticle = ReticleType.Diamond;
                    break;
                case 2:
                    Reticle = ReticleType.Square;
                    break;
            }
        } else {
            Reticle = ReticleType.None;
        }
        
    }

    // Initializes enum TargetType targetType, used for deciding affected targets
    private void SetTargetType( int type ) {
        switch (type) {
            case 0:
                targetType = TargetType.Any;
                break;
            case 1:
                targetType = TargetType.Ally;
                break;
            case 2:
                targetType = TargetType.Enemy;
                break;
            case 3:
                targetType = TargetType.Self;
                break;
        }
    }

    // Initializes enum AbilityType Type, used for Ability Functionality
    private void SetAbilityType( int type ) {
        switch (type) {
            case 0:
                Type = AbilityType.Damage;
                break;
            case 1:
                Type = AbilityType.Healing;
                break;
            case 2:
                Type = AbilityType.Dispell;
                break;
            case 3:
                Type = AbilityType.Dash;
                break;
            case 4:
                Type = AbilityType.Teleport;
                break;
            case 5:
                Type = AbilityType.StunBreak;
                break;
            case 6:
                Type = AbilityType.SnareBreak;
                break;
            case 7:
                Type = AbilityType.SlowBreak;
                break;
            case 8:
                Type = AbilityType.Swap;
                break;
            case 9:
                Type = AbilityType.Appearance;
                break;
            case 10:
                Type = AbilityType.Tile;
                break;
            case 11:
                Type = AbilityType.Ressurect;
                break;
            case 12:
                Type = AbilityType.SP;
                break;
            case 13:
                Type = AbilityType.Execute;
                break;
            case 14:
                Type = AbilityType.MindControl;
                break;
            case 15:
                Type = AbilityType.Consume;
                break;
        }
    }

    // Initializes abilityValue, used for Healing or Damage calculations
    private void SetAbilityDamage( float damage ) {
        abilityValue = damage;
    }

    // Method responsible for the cast animation, calls Cast() after completion
    public IEnumerator Animation( Color32 color1, Color32 color2, Color32 color3 ) {

        // Initialize sequence for the animation order
        TileController.sequence = 0;

        // Animation sequence 0
        yield return new WaitForSeconds( 0.3f );
        TileController.Instance.ColorTiles( ReticleController.CastArea, TileController.ColorMethod.Cast );

        // Animation sequence 1
        yield return new WaitForSeconds( 0.1f );
        TileController.Instance.ColorTiles( ReticleController.CastArea, TileController.ColorMethod.Cast );

        // Animation sequence 2
        yield return new WaitForSeconds( 0.1f );
        TileController.Instance.ColorTiles( ReticleController.CastArea, TileController.ColorMethod.Cast );

        // Animation sequence 3
        yield return new WaitForSeconds( 0.1f );
        TileController.Instance.ColorTiles( ReticleController.CastArea, TileController.ColorMethod.Cast );

        // Animation sequence 4
        yield return new WaitForSeconds( 0.1f );
        TileController.Instance.ColorTiles( ReticleController.CastArea, TileController.ColorMethod.Cast );

        // Cast ability
        BeginCast();

        GameObject.Find( "Move Button" ).GetComponent<Button>().interactable = true;
        GameObject.Find( "Attack Button" ).GetComponent<Button>().interactable = true;
        GameObject.Find( "End Button" ).GetComponent<Button>().interactable = true;
    }
    
    // Casts the spell, and deals damage to affected targets using abilityValue. Healing is done with -abilityValue
    private void BeginCast() {

        TurnController.ActionPoints -= Cost;
        List<string> targets = new List<string>();

        switch (targetType) {
            case TargetType.Any:
                targets.Add( "Player" );
                targets.Add( "Ally" );
                targets.Add( "Enemy" );
                break;
            case TargetType.Enemy:
                targets.Add( "Enemy" );
                break;
            case TargetType.Ally:
                targets.Add( "Player" );
                targets.Add( "Ally" );
                break;
            case TargetType.Corpse:
                targets.Add( "Corpse" );
                break;
            default:
                Debug.LogError( "Invalid Target Type" );
                break;
        }

        switch (targetType) {
            case TargetType.Any:
            case TargetType.Enemy:
            case TargetType.Ally:
            case TargetType.Corpse:
                SelectTargetType( targets );
                break;
            case TargetType.Self:
                // disregard reticle position
                CastOnSelf();
                break;
            case TargetType.Ground:
                // cast on ground, change reticles for teleport and dash
                break;
            default:
                Debug.LogError( "Invalid Target Type" );
                break;
        }

        UIController.Instance.CheckAbilities();
        button.OnCooldown();
        TurnController.State = TurnController.TurnState.Standby;
    }

    // ensures that teleport or dash to ally/enemy only casts once
    private bool mobilityCompleted;

    private void SelectTargetType( List<string> targets  ) {
        mobilityCompleted = false;
        foreach(string target in targets) {
            switch (Type) {
                case AbilityType.Damage:
                    ApplyValToTargets( GameObject.FindGameObjectsWithTag( target ), abilityValue );
                    break;
                case AbilityType.Healing:
                    ApplyValToTargets( GameObject.FindGameObjectsWithTag( target ), -abilityValue );
                    break;
                case AbilityType.Dispell:
                    CleanseTargets( GameObject.FindGameObjectsWithTag( target ) );
                    break;
                case AbilityType.Dash: // change reticle and do standard dash code
                    if(!mobilityCompleted)
                        DashToLocation();
                    break;
                case AbilityType.Teleport: // disregard for now
                    if(!mobilityCompleted)
                        TeleportToLocation();
                    break;
                case AbilityType.StunBreak:
                    ClearStunFromTargets( GameObject.FindGameObjectsWithTag( target ) );
                    break;
                case AbilityType.SnareBreak:
                    ClearSnareFromTargets( GameObject.FindGameObjectsWithTag( target ) );
                    break;
                case AbilityType.SlowBreak:
                    ClearSlowFromTargets( GameObject.FindGameObjectsWithTag( target ) );
                    break;
                case AbilityType.Swap: // disregard for now
                    Debug.LogError( "No handlers for AbilityType.Swap" );
                    break;
                case AbilityType.Appearance: // disregard for now
                    Debug.LogError( "No handlers for AbilityType.Appearance" );
                    break;
                case AbilityType.Tile: // disregard for now
                    Debug.LogError( "No handlers for AbilityType.Tile" );
                    break;
                case AbilityType.Ressurect: // disregard for now
                    Debug.LogError( "No handlers for AbilityType.Ressurect" );
                    break;
                case AbilityType.SP:
                    DamageSpOfTargets( GameObject.FindGameObjectsWithTag( target ), abilityValue );
                    break;
                case AbilityType.Execute:
                    AttemptExecuteOfTargets( GameObject.FindGameObjectsWithTag( target ), abilityValue );
                    break;
                case AbilityType.MindControl: // disregard for now
                    Debug.LogError( "No handlers for AbilityType.MindControl" );
                    break;
                case AbilityType.Consume: // disregard for now
                    Debug.LogError( "No handlers for AbilityType.Consume" );
                    break;
            }
        }
    }

    #region Ability Functionality
    private void TeleportToLocation() {
        GameObject.FindWithTag( "Player" ).transform.position = new Vector3( ReticleController.CastArea[0].x + 0.5f, ReticleController.CastArea[0].y + 0.5f, ReticleController.CastArea[0].z );
        mobilityCompleted = true;
        NavigationController.Instance.ReInit();
    }

    private void DashToLocation() {
        // TODO change method of movement to support animation
        GameObject.FindWithTag( "Player" ).transform.position = new Vector3( ReticleController.CastArea[0].x + 0.5f, ReticleController.CastArea[0].y + 0.5f, ReticleController.CastArea[0].z );
        mobilityCompleted = true;
        NavigationController.Instance.ReInit();
    }

    private void ClearStunFromTargets( GameObject[] targets ) {
        foreach(GameObject target in targets)
            target.GetComponent<Character>().RemoveStatus( "Stunned" );
    }

    private void ClearSnareFromTargets( GameObject[] targets ) {
        foreach(GameObject target in targets)
            target.GetComponent<Character>().RemoveStatus( "Snared" );
    }

    private void ClearSlowFromTargets( GameObject[] targets ) {
        foreach (GameObject target in targets)
            target.GetComponent<Character>().RemoveStatus( "Slowed" );
    }

    private void DamageSpOfTargets( GameObject[] targets, float val ) {
        // remove val % of targets SP, conditionally lifesteal the SP to yourself with modifier
        foreach (GameObject target in targets)
            DamageSpOfTarget( target, val );
        
    }

    private void DamageSpOfTarget ( GameObject target, float val ) {
        float spDamage = target.GetComponent<Character>().TakeSpDamage( val );
        if(secondaryValue != 0) {
            EntityManager.Entities[TurnController.turn].GetComponent<Character>().GenerateSp( spDamage * (secondaryValue / 100) * 4 );
            // may need to make this based on max sp rather than current sp, also consider increasing SP generation by 33% TODO
        }
        ApplyStatusToTarget( target );
    }

    private void AttemptExecuteOfTargets( GameObject[] targets, float val ) {
        foreach(GameObject target in targets)
            AttemptExecuteOfTarget( target, val );
    }

    private void AttemptExecuteOfTarget( GameObject target, float val ) {
        if((target.GetComponent<Character>().currentHp / target.GetComponent<Character>().maxHp * 100) <= val)
            target.GetComponent<Character>().KillCharacter();        
    }

    // ValidateTargets methods ensure that only Characters inside the cast area will be affected
    private void ApplyValToTargets( GameObject[] targets, float val ) {
        foreach (GameObject target in targets) 
            ApplyValToTarget( target, val );
    }

    private void CleanseTargets( GameObject[] targets ) {
        foreach (GameObject target in targets) 
            CleanseTarget( target );
    }
    private void CleanseTarget( GameObject target ) {
        if (ReticleController.CastArea.Contains( TileController.GridLayout.WorldToCell( target.transform.position ) ) && target.tag == "Enemy") { // need to rework this logic later with teams implemented TODO
            target.GetComponent<Character>().ClearBuffs(0); // 0 is placeholder TODO
            target.GetComponent<Character>().ClearDebuffs(0); // Temporary until teams TODO
            target.GetComponent<Character>().RevealThisCharacter();
        }
        else if (ReticleController.CastArea.Contains( TileController.GridLayout.WorldToCell( target.transform.position ) ) && target.tag == "Ally") {
            target.GetComponent<Character>().ClearBuffs(0); // Temporary until teams TODO
            target.GetComponent<Character>().ClearDebuffs(0); // 0 is placeholder TODO
            target.GetComponent<Character>().RevealThisCharacter();
        }
        else if (ReticleController.CastArea.Contains( TileController.GridLayout.WorldToCell( target.transform.position ) ) && target.tag == "Player") {
            target.GetComponent<Character>().ClearDebuffs(0);
        }
        ApplyStatusToTarget( target );
    }

    private void ApplyValToTarget( GameObject target, float val ) {
        if (ReticleController.CastArea.Contains( TileController.GridLayout.WorldToCell( target.transform.position ) )) {
            target.GetComponent<Character>().TakeDamage( val, TurnController.turn );
            ApplyStatusToTarget( target );
        }
    }

    private void ApplyStatusToTarget( GameObject target ) {
        if (status != null) {
            Status curStatus = new Status();
            curStatus = status;
            curStatus.sourceCharacter = TurnController.turn;
            target.GetComponent<Character>().ApplyStatusEffect( curStatus );
        }
    }

    #endregion

    #region Self Cast Functionality
    private void CastOnSelf() {
        switch (Type) {
            case AbilityType.Damage:
                ValueToSelf( abilityValue );
                break;
            case AbilityType.Healing:
                ValueToSelf( -abilityValue );
                break;
            case AbilityType.Dispell:
                CleanseSelf();
                break;
            case AbilityType.StunBreak:
                StunBreakSelf();
                break;
            case AbilityType.SnareBreak:
                SnareBreakSelf();
                break;
            case AbilityType.SlowBreak:
                break;
        }
    }
    private void ValueToSelf( float val ) {
        EntityManager.Entities[TurnController.turn].GetComponent<Character>().TakeDamage( val, -1 );
    }
    private void CleanseSelf() {
        EntityManager.Entities[TurnController.turn].GetComponent<Character>().ClearDebuffs( 0 ); // 0 is placeholder TODO
    }
    private void StunBreakSelf() {
        // nTODO must make this castable while stunned somewhere else
        EntityManager.Entities[TurnController.turn].GetComponent<Character>().RemoveStatus( "Stunned" );
    }
    private void SnareBreakSelf() {
        EntityManager.Entities[TurnController.turn].GetComponent<Character>().RemoveStatus( "Snared" );
    }
    private void SlowBreakSelf() {
        EntityManager.Entities[TurnController.turn].GetComponent<Character>().RemoveStatus( "Slowed" );
    }
    #endregion
}
