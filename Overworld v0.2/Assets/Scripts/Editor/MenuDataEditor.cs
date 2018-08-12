using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

[CanEditMultipleObjects]
public class DataEditorWindow : EditorWindow {

    public MenuData menuData;
    public PlayerData playerData;
    public BattleData battleData;

    private string menuDataFilePath = "/StreamingAssets/menu-data.json";
    private string playerDataFilePath = "/StreamingAssets/player-data.json";
    private string battleDataFilePath = "/StreamingAssets/battle-data.json";
    private Vector2 scrollPos;

    [MenuItem ("Window/Menu Data Editor")]
    static void Init() {

        DataEditorWindow window = (DataEditorWindow)EditorWindow.GetWindow( typeof( DataEditorWindow ) );
        window.Show();

    }

    void OnGUI() {
        if (menuData != null && battleData != null && playerData != null) {

            EditorGUILayout.BeginVertical();
            scrollPos = EditorGUILayout.BeginScrollView( scrollPos );
            SerializedObject serializedObject = new SerializedObject( this );
            SerializedProperty serializedPropertyMenu = serializedObject.FindProperty("menuData");
            EditorGUILayout.PropertyField( serializedPropertyMenu, true );
            SerializedProperty serializedPropertyBattle = serializedObject.FindProperty( "battleData" );
            EditorGUILayout.PropertyField( serializedPropertyBattle, true );
            SerializedProperty serializedPropertyPlayer = serializedObject.FindProperty( "playerData" );
            EditorGUILayout.PropertyField( serializedPropertyPlayer, true );
            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();



            if (GUILayout.Button("Save")) {
                SaveMenuData();
                SavePlayerData();
                SaveBattleData();
            }
        }

        if (GUILayout.Button( "Load All" )) {
            LoadMenuData();
            LoadPlayerData();
            LoadBattleData();
        }
    }

    private void LoadMenuData() {

        string filePath = Application.dataPath + menuDataFilePath;

        if (File.Exists( filePath )) {
            string dataAsJson = File.ReadAllText( filePath );
            menuData = JsonUtility.FromJson<MenuData>( dataAsJson );
        } else {
            menuData = new MenuData();
        }

    }
	
    private void SaveMenuData() {

        string dataAsJson = JsonUtility.ToJson( menuData );
        string filePath = Application.dataPath + menuDataFilePath;
        File.WriteAllText( filePath, dataAsJson );

    }

    private void LoadPlayerData() {

        string filePath = Application.dataPath + playerDataFilePath;

        if (File.Exists( filePath )) {
            string dataAsJson = File.ReadAllText( filePath );
            playerData = JsonUtility.FromJson<PlayerData>( dataAsJson );

        }
        else {
            playerData = new PlayerData();
        }

    }

    private void SavePlayerData() {

        string dataAsJson = JsonUtility.ToJson( playerData );
        string filePath = Application.dataPath + playerDataFilePath;
        File.WriteAllText( filePath, dataAsJson );

    }

    private void LoadBattleData() {

        string filePath = Application.dataPath + battleDataFilePath;

        if (File.Exists( filePath )) {
            string dataAsJson = File.ReadAllText( filePath );
            battleData = JsonUtility.FromJson<BattleData>( dataAsJson );
        }
        else {
            battleData = new BattleData();
        }

    }

    private void SaveBattleData() {

        string dataAsJson = JsonUtility.ToJson( battleData );
        string filePath = Application.dataPath + battleDataFilePath;
        File.WriteAllText( filePath, dataAsJson );

    }

}
