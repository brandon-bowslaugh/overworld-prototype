using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ReticleController : MonoBehaviour {

    #region Create Singleton
    private static ReticleController _instance;

    public static ReticleController Instance {
        get {
            if (_instance == null) {
                GameObject go = new GameObject( "ReticleController" );
                go.AddComponent<ReticleController>();
            }
            return _instance;
        }
    }

    void Awake() {
        _instance = this;
    }
    #endregion
    
    public static List<Vector3Int> CastArea { get; set; }
    public static List<Vector3Int> RangeArea { get; set; }

    public static Color32 ReticleColor = new Color32( 201, 165, 255, 255 ); // cast area color
    public static Color32 RangeColor = new Color32( 224, 204, 255, 255 ); // range color

    // TODO add more colors
    private Color32 _damageColor1 = new Color32( 255, 99, 104, 255 );
    private Color32 _damageColor2 = new Color32( 255, 61, 67, 255 );
    private Color32 _damageColor3 = new Color32( 255, 0, 0, 255 );

    public static Color32 Color1 { get; private set; }
    public static Color32 Color2 { get; private set; }
    public static Color32 Color3 { get; private set; }

    // Initializes the variables
    public void Setup() {
        CastArea = new List<Vector3Int>();
        RangeArea = new List<Vector3Int>();
    }

    // Handles casting a spell at a desired location
    public void ConfirmLocation() {
        // Ensures that the selected area is in the cast range
        if (Input.GetMouseButtonDown( 0 ) && RangeArea.Contains( TileController.GridLayout.WorldToCell( InputController.CursorPosition ) )) {
            // Ensures the user was not trying to click a UI element
            if (!EventSystem.current.IsPointerOverGameObject()) {
                // Sets the ability animation colors
                GameObject.Find( "Move Button" ).GetComponent<Button>().interactable = false;
                GameObject.Find( "Attack Button" ).GetComponent<Button>().interactable = false;
                GameObject.Find( "End Button" ).GetComponent<Button>().interactable = false;
                if (AbilityController.Type == AbilityController.AbilityType.Damage) {
                    SetColors( _damageColor1, _damageColor2, _damageColor3 );
                } else {
                    // This is where additional ability colors will go later TODO
                    SetColors( _damageColor1, _damageColor2, _damageColor3 );
                }
                // Removes the tiles colored for displaying range
                TileController.Instance.ClearTiles( RangeArea );
                // Begins cast animation
                StartCoroutine( AbilityController.Instance.Animation(_damageColor1, _damageColor2, _damageColor3 ) );
                // Clear the reticle type
                AbilityController.Reticle = AbilityController.ReticleType.None;

            } 
        } else if (Input.GetMouseButtonDown( 0 )) {
            AbilityController.Reticle = AbilityController.ReticleType.None;
            TileController.Instance.ClearTiles( RangeArea );
        } else if (Input.GetMouseButtonDown( 1 )) {
            AbilityController.Reticle = AbilityController.ReticleType.None;
            TileController.Instance.ClearTiles( RangeArea );
        }
    }

    // This method is for setting animation colors
    private void SetColors(Color32 one, Color32 two, Color32 three) {
        Color1 = one;
        Color2 = two;
        Color3 = three;
    }

}
