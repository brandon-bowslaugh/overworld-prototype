using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class DataController : MonoBehaviour {
    #region Variables

    // Filepath where data is stored
    // His: SameName
    private readonly string menuDataFileName = "menu-data.json";
    private readonly string playerDataFileName = "player-data.json";
    private readonly string battleDataFileName = "battle-data.json";

    #endregion

    // TODO fix this to swap arrays to lists
    // Use this for initialization
    void Awake() {
        DataLoader dataLoader = new DataLoader();
        if (!dataLoader.DataExists()) {
            DontDestroyOnLoad( gameObject );
            gameObject.tag = "Loaded";
        }         
        LoadMenuData();
        LoadPlayerComp();
        LoadPlayerData();
        LoadBattleData();// need to move this so that it loads right before entering a battle TODO
        InitializeBattle();
    }

    #region Load Menu Data

    private MenuData loadedMenuData;
    private MenuTalentTreeData[] allMenuTalentTreeData;
    private MenuWeaponData[] allMenuWeaponData;
    private MenuAbilityData[] allMenuAbilityData;
    private int navData = 0;

    private void LoadMenuData() {

        string filePath = Path.Combine( Application.streamingAssetsPath, menuDataFileName );

        if (File.Exists( filePath )) {

            string dataAsJson = File.ReadAllText( filePath );
            loadedMenuData = MenuData.CreateFromJSON( dataAsJson );
            allMenuWeaponData = loadedMenuData.menuWeaponData;
            allMenuAbilityData = loadedMenuData.menuAbilityData;
            allMenuTalentTreeData = loadedMenuData.menuTalentTreeData;

        }
        else {
            Debug.LogError( "Cannot load game data" );
        }

    }

    #endregion

    #region Load Player Data

    private PlayerCompData playerComp;
    private PlayerData loadedPlayerData;
    private PlayerPartyData characterSwapParty;
    private PlayerPartyData[] allPlayerPartyData;
    private PlayerCharacterData[] allPlayerCharacterData;
    private int previousCharacterId;
    private int editCharacterid;

    private void LoadPlayerData() {

        string filePath = Path.Combine( Application.streamingAssetsPath, playerDataFileName );

        if (File.Exists( filePath )) {

            string dataAsJson = File.ReadAllText( filePath );
            loadedPlayerData = PlayerData.CreateFromJSON( dataAsJson );
            allPlayerCharacterData = loadedPlayerData.playerCharacterData;
            allPlayerPartyData = loadedPlayerData.playerPartyData;
            previousCharacterId = loadedPlayerData.previousCharacterId;
            characterSwapParty = loadedPlayerData.characterSwapParty;

        }
        else {
            Debug.LogError( "Cannot load game data" );
        }

    }

    #endregion

    #region Load Battle Data

    private BattleData loadedBattleData;
    private Status[] statusEffects;

    private void LoadBattleData() {

        string filePath = Path.Combine( Application.streamingAssetsPath, battleDataFileName );

        if (File.Exists( filePath )) {

            string dataAsJson = File.ReadAllText( filePath );
            loadedBattleData = BattleData.CreateFromJSON( dataAsJson );
            statusEffects = loadedBattleData.statusData;

        }
        else {
            Debug.LogError( "Cannot load game data" );
        }

    }

    #endregion

    #region Initialize Battle Mode

    // Variables for InitializeBattle()
    private List<BattleCharacter> battleCharacters;

    private void InitializeBattle() {
        #region Character Creation Variables
        List<BattleCharacter> tempCharacters = new List<BattleCharacter>();
        // temporary value
        int baseMovement = 1;
        // base this on rarity/level/armortype later TODO
        int baseHealth = 1824;
        // temporary value
        int baseInitiative = 6;
        #endregion
        PlayerPartyData party = GetPlayerPartyData();
        int[] partyMemberIds = { party.slotOneCharacterId, party.slotTwoCharacterId, party.slotThreeCharacterId, party.slotFourCharacterId };
        foreach (int id in partyMemberIds) {
            BattleCharacter character = new BattleCharacter();
            PlayerCharacterData characterData = GetCharacter( id );
            if (characterData.name != null && characterData.name != "") {
                // set Name
                character.name = characterData.name;
                // set Movement range
                // set Max HP
                character.maxHp = (int)Mathf.Floor( baseHealth * (characterData.armor / 10.0f) ) + baseHealth;
                Debug.Log( character.maxHp );
                // set Initiative
                if (characterData.armor == 0) {
                    character.initiative = baseInitiative + (int)Mathf.Floor( Random.value * 20 );
                    character.movementCost = 1;
                }
                else if (characterData.armor == 1) {
                    character.initiative = (baseInitiative + 5) + (int)Mathf.Floor( Random.value * 20 );
                    character.movementCost = 1;
                }
                else if (characterData.armor == 2) {
                    character.initiative = (baseInitiative - 5) + (int)Mathf.Floor( Random.value * 20 );
                    character.movementCost = 2;
                }
                else {
                    Debug.LogError( "Can not find out what armor character is wearing" );
                }
                // set Damage Mod
                character.damageMod = 1;
                // Temporary
                character.abilities = LoadAbilities( characterData.weapon );
                tempCharacters.Add( character );
            }
            else {
                Debug.LogError( "character does not exist in party" );
            }

        }
        battleCharacters = tempCharacters;
    }

    #endregion

    #region Menu Functionality

    public string GetVersionNumber() {
        return loadedMenuData.version;
    }

    private void SaveMenuData() {
        string dataAsJson = JsonUtility.ToJson( loadedMenuData );
        string filePath = Application.streamingAssetsPath + "/" + menuDataFileName;
        File.WriteAllText( filePath, dataAsJson );
        LoadPlayerData();
    }

    public void SawChangeLog() {
        loadedMenuData.changeLogSeen = true;
        SaveMenuData();
    }

    public bool ChangeLogSeen() {
        return loadedMenuData.changeLogSeen;
    }

    public void AddNewCharacter() {
        List<PlayerCharacterData> tempList = loadedPlayerData.playerCharacterData.ToList();
        int count = tempList.Count;
        SetEditCharacter( count );
        PlayerCharacterData newPlayer = GenerateNewCharacter(count);
        tempList.Add( newPlayer );
        loadedPlayerData.playerCharacterData = tempList.ToArray();
        SavePlayerData();
    }

    public void AddNewCharacterToParty() {
        List<PlayerCharacterData> tempList = loadedPlayerData.playerCharacterData.ToList();
        int count = tempList.Count;
        SetEditCharacter( count );
        PlayerCharacterData newPlayer = GenerateNewCharacter( count );
        tempList.Add( newPlayer );
        loadedPlayerData.playerCharacterData = tempList.ToArray();
        SwapCharacterInParty(GetPartyToEdit(), GetCharacterToSwap(), count);
    }

    private PlayerCharacterData GenerateNewCharacter(int count) {
        PlayerCharacterData newPlayer = new PlayerCharacterData();
        newPlayer.id = count;
        newPlayer.name = "New Character " + (count + 1).ToString();
        newPlayer.weapon = 14;
        newPlayer.offHand = -1;
        newPlayer.armor = 0;
        newPlayer.quality = 0;
        newPlayer.level = 30;
        return newPlayer;
    }

    private int editType;
    private int editSlot;

    public void SetEditType(int val) {
        editType = val;
    }

    public void SetEditSlot(int val) {
        editSlot = val;
    }

    public int GetEditType() {
        return editType;
    }

    public int GetEditSlot() {
        return editSlot;
    }

    public MenuTalentTreeData[] GetTalentTreeData() {
        return allMenuTalentTreeData;
    }

    public void SaveCharacter( PlayerCharacterData character ) {
        // edit the selected character based on the editcharacter id
        if(character.talents.Count != 0) {
            character.level = character.talents.Count;
        } else {
            character.level = 30;
        }
        for(int i=0; i<allPlayerCharacterData.Length; i++) {
            if (allPlayerCharacterData[i].id == character.id) {
                allPlayerCharacterData[i] = character;
            }
        }
        SavePlayerData();
    }

    public MenuTalentTreeData GetTalentTree(int id) {
        foreach(MenuTalentTreeData talentTree in allMenuTalentTreeData) {
            if(talentTree.id == id) {
                return talentTree;
            }
        }
        Debug.LogError( "Talent Tree Does Not Exist!" );
        return new MenuTalentTreeData();
    }

    public void SetPreviousPage(int prev) {
        navData = prev;
    }
    public int GetPreviousPage() {
        return navData;
    }
    public void SetCharacterSwap(PlayerPartyData party, int characterId ) {
        previousCharacterId = characterId;
        characterSwapParty = party;
    }
    public int GetCharacterToSwap() {
        return previousCharacterId;
    }
    public PlayerPartyData GetPartyToEdit() {
        return characterSwapParty;
    }

    public void SwapCharacterInParty(PlayerPartyData party, int oldCharacterId, int newCharacterId ) {
        int[] partyMembers = { party.slotOneCharacterId, party.slotTwoCharacterId, party.slotThreeCharacterId, party.slotFourCharacterId };
        for(int i=0; i<partyMembers.Length; i++) {
            if(partyMembers[i] == oldCharacterId) {
                partyMembers[i] = newCharacterId;
                break;
            }
        }
        int[] newPartyMembers = { partyMembers[0], partyMembers[1], partyMembers[2], partyMembers[3] };
        for (int i=0; i<allPlayerPartyData.Length; i++) {
            if(allPlayerPartyData[i] == party) {
                party.slotOneCharacterId = newPartyMembers[0];
                party.slotTwoCharacterId = newPartyMembers[1];
                party.slotThreeCharacterId = newPartyMembers[2];
                party.slotFourCharacterId = newPartyMembers[3];
                allPlayerPartyData[i].Equals(party);
                break;
            }
        }
        loadedPlayerData.playerPartyData = allPlayerPartyData;
        SavePlayerData();
    }

    public MenuWeaponData[] GetAllMenuWeaponData() {
        return allMenuWeaponData;
    }

    private MenuWeaponData GetWeapon( int id ) {
        foreach (MenuWeaponData weapon in allMenuWeaponData) {
            if (weapon.id == id) {
                return weapon;
            }
        }
        return new MenuWeaponData();
    }
    private MenuAbilityData GetAbility( int id ) {
        foreach (MenuAbilityData ability in allMenuAbilityData) {
            if (ability.id == id) {
                return ability;
            }
        }
        return new MenuAbilityData();
    }
    #endregion

    #region Player Functionality - Data Manipulation

    private void SavePlayerData() {
        string dataAsJson = JsonUtility.ToJson( loadedPlayerData );
        string filePath = Application.streamingAssetsPath + "/" + playerDataFileName;
        File.WriteAllText( filePath, dataAsJson );
        LoadPlayerData();
    }

    public void SetEditCharacter(int characterId) {

        editCharacterid = characterId;

    }

    private void SavePlayerComp() { // his SavePlayerProgress
        PlayerPrefs.SetInt( "currentComp", playerComp.partyId );
        loadedPlayerData.selectedParty = playerComp;
        SavePlayerData();
    }

    // Temporary to get to testing, complete rewrite later
    public void CreateTestParty(PlayerPartyData testParty) {
        List<PlayerPartyData> listPlayerPartyData = allPlayerPartyData.ToList();
        testParty.id = listPlayerPartyData.Count;
        listPlayerPartyData.Add( testParty );
        loadedPlayerData.playerPartyData = listPlayerPartyData.ToArray();
        SavePlayerData();
    }

    public void EditPartyName(PlayerPartyData party) {
        List<PlayerPartyData> listPlayerPartyData = allPlayerPartyData.ToList();
        listPlayerPartyData[party.id].name = party.name;
        loadedPlayerData.playerPartyData = listPlayerPartyData.ToArray();
        SavePlayerData();
    }
    
    public void DuplicateParty(PlayerPartyData party) {
        // TODO fix this its probably too much resources for what its doing
        List<PlayerPartyData> listPlayerPartyData = allPlayerPartyData.ToList();
        PlayerPartyData newParty = new PlayerPartyData();
        newParty.id = listPlayerPartyData.Count();
        newParty.name = party.name;
        newParty.slotOneCharacterId = party.slotOneCharacterId;
        newParty.slotTwoCharacterId = party.slotTwoCharacterId;
        newParty.slotThreeCharacterId = party.slotThreeCharacterId;
        newParty.slotFourCharacterId = party.slotFourCharacterId;

        listPlayerPartyData.Add( newParty );
        loadedPlayerData.playerPartyData = listPlayerPartyData.ToArray();
        SavePlayerData();
    }

    public void DeleteParty(int partyId) {
        // TODO fix this after changes to List<PlayerPartyData>
        PlayerPartyData[] newPartyData = new PlayerPartyData[allPlayerPartyData.Length - 1];
        for (int i=0; i<allPlayerPartyData.Length; i++) {
            if (allPlayerPartyData[i].id < partyId) {
                newPartyData[i] = allPlayerPartyData[i];
            } else if(allPlayerPartyData[i].id > partyId) {
                allPlayerPartyData[i].id = allPlayerPartyData[i].id-1;
                newPartyData[i - 1] = allPlayerPartyData[i];
            }
        }
        loadedPlayerData.playerPartyData = newPartyData;
        if (partyId <= GetCurrentPlayerParty()) {
            SetPlayerComp( GetCurrentPlayerParty()-1 );
        } else {
            SavePlayerData();
        }
    }

    public void SetPlayerComp( int newPartyId ) { // his SubmitNewPlayerScore
        playerComp.partyId = newPartyId;
        SavePlayerComp();
    }

    #endregion

    #region Player Functionality - Data Accessing

    public int GetEditCharacter() {
        return editCharacterid;
    }

    public PlayerPartyData GetPlayerPartyData() { // his GetCurrentRoundData()
        return allPlayerPartyData[loadedPlayerData.selectedParty.partyId];
    }
    public PlayerPartyData GetPlayerPartyData(int partyId) { // his GetCurrentRoundData()
        return allPlayerPartyData[partyId];
    }

    public PlayerPartyData[] GetAllPlayerParties() {
        return allPlayerPartyData;
    }

    public PlayerCharacterData[] GetAllPlayerCharacterData() {
        return allPlayerCharacterData;
    }

    private void LoadPlayerComp() { // his LoadPlayerProgress
        playerComp = new PlayerCompData();
        if (PlayerPrefs.HasKey( "currentComp" )) {
            playerComp.partyId = PlayerPrefs.GetInt("currentComp");
        }
    }

    public int GetCurrentPlayerParty() { // his GetHighestPlayerScore
        return playerComp.partyId;
    }

    
    public PlayerCharacterData GetCharacter(int id) {
        foreach(PlayerCharacterData character in allPlayerCharacterData) {
            if (character.id == id)
                return character;
        }
        return new PlayerCharacterData();
    }

    #endregion

    #region Battle Functionality - Data Accessing
    
    private List<BattleAbility> LoadAbilities(int id) {


        List<BattleAbility> abilities = new List<BattleAbility>();
        MenuWeaponData weapon = GetWeapon(id);
        if(weapon.name == "" || weapon.name == null) {
            Debug.LogError("Weapon does not exist");
        }
        int[] abilityIds = { weapon.abilities[0], weapon.abilities[1], weapon.abilities[2] };
        foreach(int abilityId in abilityIds) {
            MenuAbilityData abilityData = GetAbility(abilityId);
            BattleAbility ability = new BattleAbility();
            ability.abilityId = abilityId;
            ability.name = abilityData.name;
            ability.value = abilityData.value;
            ability.xAxis = abilityData.xAxis;
            ability.range = abilityData.range;
            ability.cost = abilityData.cost;
            ability.abilityType = abilityData.type;
            ability.reticleType = abilityData.reticle;
            ability.targetType = abilityData.target;
            ability.cooldown = abilityData.cooldown;
            if(abilityData.statusId != -1) {
                for (int i = 0; i < statusEffects.Count(); i++) {
                    if (abilityData.statusId == i) {
                        ability.status = statusEffects[i];
                    }
                }
            }
            abilities.Add( ability );
        }
        return abilities;
    }
    public List<BattleCharacter> GetBattleData() {
        return battleCharacters;
    }
    
    #endregion
}
