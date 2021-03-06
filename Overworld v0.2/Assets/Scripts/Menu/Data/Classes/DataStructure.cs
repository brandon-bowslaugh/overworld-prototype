﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

#region Menu Data
[System.Serializable]
public class MenuData {

    public string version;
    public MenuAbilityData[] menuAbilityData;
    public MenuWeaponData[] menuWeaponData;
    public MenuTalentTreeData[] menuTalentTreeData;
    public bool changeLogSeen;

    public static MenuData CreateFromJSON( string jsonString ) {
        return JsonUtility.FromJson<MenuData>( jsonString );
    }

    public static MenuAbilityData[] GetMenuAbilityData(MenuData menuData) {
        return menuData.menuAbilityData;
    }

    /*
    public static MenuTalentData[] GetMenuTalentData( MenuData menuData ) {
        return menuData.menuTalentData;
    }
    */

    public static MenuWeaponData[] GetMenuWeaponData( MenuData menuData ) {
        return menuData.menuWeaponData;
    }
}

[System.Serializable]
public class MenuAbilityData {

    public int id;
    public string name;
    public int value;
    public bool physical;
    public int cost;
    public int cooldown;
    public int secondaryValue;
    public int target;
    public int range;
    public int type;
    public int xAxis;
    public int reticle;
    public string talentTree;
    public string description;
    public string shiftDescription;
    public int statusId;

}

[System.Serializable]
public class MenuTalentData {

    public int id;
    public string name;
    public int requiredTalentId;
    public int type; // { 0="Ability", 1="Passive", 2="Boon", 3="Ultimate" }
    public int subTree;
    public string description;
    public string shiftDescription;
    public int school; // 0 = physical, 1 = magical
    public int aoe; // 0 = targeted, 1 = aoe, 2 = cone
    public int range; // 0 = melee, 1 = ranged, 2 = self

}

[System.Serializable]
public class MenuWeaponData {

    public int id;
    public string name;
    public int damage;
    public int[] abilities;

}

[System.Serializable]
public class MenuTalentTreeData {
    public int id;
    public string name;
    public MenuSubTreeData[] subTrees; // 3 sub trees per talent tree
}

[System.Serializable]
public class MenuSubTreeData {
    public int id;
    public string name;
    public MenuTalentData[] talents; // 18 talents per sub tree
}

#endregion

#region Player Data

[System.Serializable]
public class PlayerData {

    public int previousCharacterId;
    public PlayerCompData selectedParty;
    public PlayerPartyData characterSwapParty;
    public PlayerPartyData[] playerPartyData;
    public PlayerCharacterData[] playerCharacterData;

    public static PlayerData CreateFromJSON( string jsonString ) {
        return JsonUtility.FromJson<PlayerData>( jsonString );
    }

    public static int GetPreviousCharacterId( PlayerData playerData ) {
        return playerData.previousCharacterId;
    }

    public static PlayerCompData GetPlayerCompData( PlayerData playerData ) {
        return playerData.selectedParty;
    }

    public static PlayerPartyData GetPlayerSwapParty ( PlayerData playerData ) {
        return playerData.characterSwapParty;
    }

    public static PlayerPartyData[] GetPlayerPartysData( PlayerData playerData ) {
        return playerData.playerPartyData;
    }

    public static PlayerCharacterData[] GetPlayerCharacterData( PlayerData playerData ) {
        return playerData.playerCharacterData;
    }

}

[System.Serializable]
public class PlayerPartyData {

    public int id;
    public string name;
    public int slotOneCharacterId;
    public int slotTwoCharacterId;
    public int slotThreeCharacterId;
    public int slotFourCharacterId;

}

[System.Serializable]
public class PlayerCharacterData {

    public int id;
    public string name;
    public int level;
    public int weapon;
    public int offHand;
    public int armor;
    public int quality;
    public List<MenuTalentData> talents;
    public CharacterSelectedData selectedTalents;

}

[System.Serializable]
public class CharacterSelectedData {

    public MenuTalentData[] abilities = new MenuTalentData[4];
    public MenuTalentData ultimate = new MenuTalentData();
    public MenuTalentData[] passives = new MenuTalentData[3];
    public List<MenuTalentData> boons = new List<MenuTalentData>();

}

[System.Serializable]
public class PlayerCompData {

    public int partyId;

}
#endregion

#region Battle Data
[System.Serializable] // For menu data editor to access
public class Status {

    public int id;
    public string statusName;
    public string description;
    public int sourceCharacter;
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

[System.Serializable]
public class BattleData {
    public Status[] statusData;

    public static BattleData CreateFromJSON( string jsonString ) {
        return JsonUtility.FromJson<BattleData>( jsonString );
    }

    public static Status[] GetBattleStatusData ( BattleData battleData ) {
        return battleData.statusData;
    }
}
#endregion
