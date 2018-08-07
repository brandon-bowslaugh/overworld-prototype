using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleCharacter {

    public string name;
    public int maxHp;
    public int movementRange;
    public int initiative;
    public float damageMod;
    public List<BattleAbility> abilities;

}

[System.Serializable] // For menu data editor to access
public class Status {

    public int id;
    public string statusName;
    public string description;
    public float value;
    public int duration;
    public int priority; // To Determine order of applications
    public int statusType;
    public int effectType;
    public int applicationTime; // Start or End of round
    public int schoolType; // Physical or Magical
    public bool hidden;
    public Status additionalStatus = null;

}

public struct StatusStruct {
    public int id;
    public string statusName;
    public string description;
    public float value;
    public int duration;
    public int priority; // To Determine order of applications
    public int statusType;
    public int effectType;
    public int applicationTime; // Start or End of round
    public int schoolType; // Physical or Magical
    public bool hidden;
    public Status additionalStatus;
}
// This class is responsible for storing all data about individual Characters and handling their Health
public class Character : MonoBehaviour {

    // Setup() variables
    public string entityName;
    public int movementStat;
    public float maxHp;
    public int initiative;
    public float damageMod;
    public int entityIdentifier;
    public float currentHp;
    public GameObject abilityBar;

    [SerializeField] public Image healthDisplay;
    [SerializeField] public TextMeshProUGUI combatText;
    [SerializeField] public TextMeshProUGUI displayName;
    // For multiplayer later
    [SerializeField] int playerNumber;

    private List<StatusStruct> statusEffects = new List<StatusStruct>();
    private List<StatusStruct> endOfTurnEffects = new List<StatusStruct>();
    public bool revealed = false;

    // Initialize Character variables on spawn
    public void Setup( int id, BattleCharacter currentCharacter, Vector3 spawnLocation ) {
        entityName = currentCharacter.name;
        maxHp = currentCharacter.maxHp;
        currentHp = maxHp;
        movementStat = currentCharacter.movementRange;
        initiative = currentCharacter.initiative;
        damageMod = currentCharacter.damageMod;
        entityIdentifier = id;
        displayName.text = entityName;
        gameObject.transform.position = spawnLocation;
    }

    // Method responsible for refreshing health bar display
    public void DisplayHealth() {
        healthDisplay.fillAmount = currentHp / maxHp;
    }

    // Method responsible for dealing damage to THIS Character
    public void TakeDamage(float damage) {
        if (damage != 0) {
            currentHp -= damage;
            if(currentHp > maxHp) {
                currentHp = maxHp;
            }
            if(damage > 0) {
                RemoveStatus( 4 ); // Remove any invisibility
                RevealThisCharacter();
                StartCoroutine(FloatingCombatText( damage.ToString(), false )); // Display damage number, Color Red
            } else {
                StartCoroutine(FloatingCombatText( (damage * -1).ToString(), true )); // Display positive healing number, Color Green
            }
            if (currentHp <= 0) {
                EntityManager.Entities.Remove( this );
                gameObject.SetActive( false );
            }
            DisplayHealth();
        }
    }

    // Display of floating combat text animation
    public IEnumerator FloatingCombatText(string text, bool positiveEffect) {
        if (positiveEffect) {
            combatText.color = new Color32( 3, 204, 0, 255 ); // Green
            combatText.text = text;
        } else {
            combatText.color = new Color32( 255, 30, 30, 255 ); // Red
            combatText.text = text;
        }
        combatText.canvasRenderer.SetAlpha( 1.0f );
        yield return new WaitForSeconds( 0.5f );
        combatText.CrossFadeAlpha( 0.0f, 2.0f, false );
        yield return new WaitForSeconds( 0.1f );
    }

    #region Status Handling, Needs much cleanup
    
    public void RemoveStatus( Status status ) {
        statusEffects.Remove( StatusClassToStruct( status ) );
    }

    public void RemoveEndOfTurnStatus( Status status ) {
        endOfTurnEffects.Remove( StatusClassToStruct( status ) );
    }

    public void ApplyStatusEffect(Status status) {
        statusEffects.Add( StatusClassToStruct(status) );
        if(status.applicationTime == 1) {
            endOfTurnEffects.Add( StatusClassToStruct( status ) );
        }
    }

    public void StatusEffectsActivate() {
        // Tell StatusController that this is the beginning of a round
        revealed = false;
        StatusController.Turn = StatusController.TurnPosition.Start;
        
        // Cycle through each start of round effect
        int counter = statusEffects.Count; // Need to do this rather than foreach because of bug
        for (int i=0; i<counter; i++) {
            Debug.Log( "StartDuration: " + statusEffects[i].duration );
            StatusController.Instance.Init( StatusStructToClass(statusEffects[i]) );
            // Subtract 1 from duration
            Status editStatus = StatusStructToClass( statusEffects[i] );
            editStatus.duration -= 1;
            statusEffects[i] = StatusClassToStruct( editStatus );
            
            if (statusEffects[i].additionalStatus != null) {
                StatusController.Instance.Init( statusEffects[i].additionalStatus );
            }
            if (statusEffects[i].duration < 1) {
                Debug.Log( "Removed From Start" );
                statusEffects.Remove( statusEffects[i] );
                i -= 1;
                counter = statusEffects.Count;
            }
        }
    }

    public void EndOfTurnStatusEffects() {
        // Tell StatusController that this is the end of a round
        StatusController.Turn = StatusController.TurnPosition.End;
        // Cycle through each end of round status effect
        int counter = endOfTurnEffects.Count;
        for (int i=0; i<counter; i++) {
            Debug.Log( "EndDuration: " + endOfTurnEffects[i].duration );
            StatusController.Instance.Init( StatusStructToClass( endOfTurnEffects[i] ) );
            Status editStatus = StatusStructToClass( endOfTurnEffects[i] );
            editStatus.duration -= 1;
            endOfTurnEffects[i] = StatusClassToStruct( editStatus );
            if (endOfTurnEffects[i].additionalStatus != null) {
                StatusController.Instance.Init( endOfTurnEffects[i].additionalStatus );
            }
            if(endOfTurnEffects[i].duration < 1) {                
                Debug.Log( "Removed From End" );
                endOfTurnEffects.Remove( endOfTurnEffects[i] );
                i -= 1;
                counter = endOfTurnEffects.Count;
            }
        }
    }

    // Remove Status of Either { Physical or Magical } and { Buff or Debuff } TODO edit to use enums instead of ints
    public void RemoveStatus( int schoolType, int effectType ) {
        foreach(StatusStruct status in statusEffects) {
            if(status.schoolType == schoolType && status.effectType == effectType) {
                statusEffects.Remove( status );
            }
        }
        foreach (StatusStruct status in endOfTurnEffects) {
            if (status.schoolType == schoolType && status.effectType == effectType) {
                statusEffects.Remove( status );
            }
        }
    }

    public void ClearAllDebuffs() {
        int counter = statusEffects.Count;
        for (int i=0; i<counter; i++) {
            if (statusEffects[i].effectType == 0) {
                statusEffects.Remove( statusEffects[i] );
                i -= 1;
                counter = statusEffects.Count;
            }
        }
        counter = endOfTurnEffects.Count;
        for (int i = 0; i < counter; i++) {
            if (endOfTurnEffects[i].effectType == 0) {
                endOfTurnEffects.Remove( endOfTurnEffects[i] );
                i -= 1;
                counter = endOfTurnEffects.Count;
            }
        }
    }

    public void ClearAllBuffs() {
        int counter = statusEffects.Count;
        for (int i = 0; i < counter; i++) {
            if (statusEffects[i].effectType == 1) {
                statusEffects.Remove( statusEffects[i] );
                i -= 1;
                counter = statusEffects.Count;
            }
        }
        counter = endOfTurnEffects.Count;
        for (int i = 0; i < counter; i++) {
            if (endOfTurnEffects[i].effectType == 1) {
                endOfTurnEffects.Remove( endOfTurnEffects[i] );
                i -= 1;
                counter = endOfTurnEffects.Count;
            }
        }
    }

    // Remove all Buff Status of { VoT, Incapacitate, Stun, Stat, Invisibility } TODO edit to use enums instead of ints
    public void RemoveStatus( int statusType ) {
        int counter = statusEffects.Count;
        for (int i=0; i<counter; i++) {
            if (statusEffects[i].statusType == statusType && statusEffects[i].effectType == 1) {
                statusEffects.Remove( statusEffects[i] );
                i -= 1;
                counter = statusEffects.Count;
            }
        }
        counter = endOfTurnEffects.Count;
        for (int i = 0; i < counter; i++) {
            if (endOfTurnEffects[i].statusType == statusType && endOfTurnEffects[i].effectType == 1) {
                endOfTurnEffects.Remove( endOfTurnEffects[i] );
                i -= 1;
                counter = endOfTurnEffects.Count;
            }
        }
    }

    public void RevealThisCharacter() {
        foreach (SpriteRenderer renderer in gameObject.GetComponentsInChildren<SpriteRenderer>()) {
            renderer.enabled = true;
        }
        gameObject.GetComponentInChildren<Canvas>().enabled = true;
    }

    private StatusStruct StatusClassToStruct(Status status) {
        StatusStruct curStatus = new StatusStruct();
        curStatus.id = status.id;
        curStatus.statusName = status.statusName;
        curStatus.description = status.description;
        curStatus.value = status.value;
        curStatus.duration = status.duration;
        curStatus.priority = status.priority;
        curStatus.statusType = status.statusType;
        curStatus.effectType = status.effectType;
        curStatus.applicationTime = status.applicationTime;
        curStatus.schoolType = status.schoolType;
        curStatus.hidden = status.hidden;
        curStatus.additionalStatus = status.additionalStatus;
        return curStatus;
    }

    private Status StatusStructToClass(StatusStruct curStatus) {
        Status status = new Status();
        status.id = curStatus.id;
        status.statusName = curStatus.statusName;
        status.description = curStatus.description;
        status.value = curStatus.value;
        status.duration = curStatus.duration;
        status.priority = curStatus.priority;
        status.statusType = curStatus.statusType;
        status.effectType = curStatus.effectType;
        status.applicationTime = curStatus.applicationTime;
        status.schoolType = curStatus.schoolType;
        status.hidden = curStatus.hidden;
        status.additionalStatus = curStatus.additionalStatus;
        return status;
    }
    #endregion
}