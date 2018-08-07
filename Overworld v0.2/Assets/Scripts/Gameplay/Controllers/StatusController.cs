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

    private StatusType Type;
    private EffectType Effect;
    private string statusName;
    private string description;
    private int priority;
    private float value;
    private int duration;

    /* 
     * For additional status effects, because some Status Effects will do multiple things.
     * I feel like hiding an additional Status Effect inside of a status effect will be the
     * best way to deal with this. 
     * (Example: Stun for 2 rounds {Status Effect #1}, and 50 Damage per round {Status Effect #2 Hidden})
     */
    private bool hidden; 

    public void Init(Status status, bool hide) {

        if (!hide) {
            statusName = status.statusName;
            description = status.description;
        }
        priority = status.priority;
        value = status.value;
        duration = status.duration;
        SetStatusType( status.statusType );
        SetEffectType( status.effectType );
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
    }

    // StatusType.VoT, uses EffectType for { Buff: value stays positive }, { Debuff: value becomes negative }
    private void ValueOverTime() {

    }
    
    // StatusType.Incapacitate, Not used at this time
    private void Incapacitate() { 

    }

    // StatusType.Stun
    private void Stun() {

    }

    // StatusType.Stat, uses EffectType to determine positive or negative { int value }
    private void Stat() {

    }

    // StatusType.Invisibility, uses EffectType for { Buff = Make invisible }, { Debuff = reveal invisible / prevent invisible }
    private void Invisibility() {

    }

}
