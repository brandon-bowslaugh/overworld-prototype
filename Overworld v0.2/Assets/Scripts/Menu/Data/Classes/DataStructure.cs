using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

#region Menu Data
[System.Serializable]
public class MenuData {

    public int navData;
    public MenuAbilityData[] menuAbilityData;
    public MenuWeaponData[] menuWeaponData;
    public MenuTalentTreeData[] menuTalentTreeData;

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

    public static int GetMenuNavData( MenuData menuData ) {
        return menuData.navData;
    }
}

[System.Serializable]
public class MenuAbilityData {

    public int id;
    public string name;
    public int value;
    public bool physical;
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

}

[System.Serializable]
public class PlayerCompData {

    public int partyId;

}
#endregion

#region Battle Data
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
