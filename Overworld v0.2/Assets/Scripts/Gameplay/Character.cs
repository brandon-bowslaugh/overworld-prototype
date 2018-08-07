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
public class Status {

    public string statusName;
    public string description;
    public float value;
    public int duration;
    public int priority; // To Determine order of applications
    public int statusType;
    public int effectType;
    public Status additionalStatus = null;

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
    [SerializeField] TextMeshProUGUI combatText;
    [SerializeField] TextMeshProUGUI displayName;
    // For multiplayer later
    [SerializeField] int playerNumber;

    public List<Status> statusEffects;

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
        statusEffects = new List<Status>();
    }

    // Method responsible for dealing damage to THIS Character
    public void TakeDamage(float damage) {
        if (damage != 0) {
            currentHp -= damage;
            if(currentHp > maxHp) {
                currentHp = maxHp;
            }
            if(damage > 0) {
                combatText.text = damage.ToString();
                combatText.color = new Color32(255, 30, 30, 255);
            } else {
                combatText.text = (damage * -1).ToString();
                combatText.color = new Color32( 3, 204, 0, 255 );
            }
            combatText.canvasRenderer.SetAlpha( 1.0f );
            combatText.CrossFadeAlpha( 0.0f, 2.5f, false );
            if (currentHp <= 0) {
                // die TODO properly
                Debug.Log( entityName + " Died" );
                EntityManager.Entities.Remove( this );
                gameObject.SetActive( false );
            }
            Debug.Log( "Took " + damage + " damage, remaining HP: " + currentHp );
            DisplayHealth();
        }
    }

    // Method responsible for refreshing health bar display
    public void DisplayHealth() {
        healthDisplay.fillAmount = currentHp / maxHp;
    }

    public void ApplyStatusEffect(Status status) {
        statusEffects.Add( status );
    }

    public void StatusEffectsActivate() {
        foreach (Status status in statusEffects) {
            StatusController.Instance.Init( status, false );
            if (status.additionalStatus != null) {
                StatusController.Instance.Init( status.additionalStatus, true );
            }
        }
    }
}