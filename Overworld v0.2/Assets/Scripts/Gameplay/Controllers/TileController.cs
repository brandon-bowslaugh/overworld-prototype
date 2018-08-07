using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

// This class is responsible for coloring tiles, and handles tile positions
// Later will swap tile coloring to sprite changes TODO
public class TileController : MonoBehaviour {

    #region Create Singleton
    private static TileController _instance;

    public static TileController Instance {
        get {
            if (_instance == null) {
                GameObject go = new GameObject( "TileController" );
                go.AddComponent<TileController>();
            }
            return _instance;
        }
    }

    void Awake() {
        _instance = this;
    }

    public void Setup() {
        tilemap = GameObject.FindWithTag( "Disp" ).GetComponent<Tilemap>();
        watermap = GameObject.FindWithTag( "Blocked" ).GetComponent<Tilemap>();
    }
    #endregion

    #region Gridlayout
    private static GridLayout _gridLayout;

    // Initializes the GridLayout, used by many classes
    public static GridLayout GridLayout {
        get {
            if(_gridLayout == null) {
                GameObject map = GameObject.FindWithTag( "Disp" );
                _gridLayout = map.GetComponent<GridLayout>();
            }
            return _gridLayout;
        }
    }
    #endregion

    #region Tile Coloring
    // Public Variables
    public enum ColorMethod { Ability, Movement, Cast };
    public static int sequence { get; set; }
    // Local Variables
    private Tilemap tilemap;
    private Tilemap watermap;
    private Color32 moveColor = new Color32( 123, 173, 252, 255 );

    #region Single Tile
    // Colors a single tile if the area is clear, and returns false if there is an obstruction in the way
    // Used for Movement Range, Collision with Players, Collision with Water
    public bool ColorTile( Vector3Int tile, ColorMethod method ) {

        if (method == ColorMethod.Movement) {
            return ColorMovementTile( tile );
        } else {
            Debug.LogError( "enum ColorMethod not set" );
            return false;
        }
    }

    // Colors a single tile regardless of obstructions
    private void ColorTile( Vector3Int tile, Color32 color ) {
        watermap.SetColor( tile, color );
        tilemap.SetColor( tile, color );
    }

    // Method for ability animation cast sequence
    private void CastTile( Vector3Int tile ) {
        switch (sequence) {
            case 0:
                ColorTile( tile, ReticleController.Color1 );
                break;
            case 1:
                ColorTile( tile, ReticleController.Color2 );
                break;
            case 2:
                ColorTile( tile, ReticleController.Color3 );
                break;
            case 3:
                ColorTile( tile, ReticleController.Color2 );
                break;
            case 4:
                ClearTile( tile );
                break;
        }
    }

    // Handles movement tile coloring and deciding if the tile is inaccessible
    // There might be a better way to handle this later TODO
    private bool ColorMovementTile(Vector3Int tile) {

        // Check for other players on the tile
        for (int i = 0; i < GameObject.FindGameObjectsWithTag( "Enemy" ).Count(); i++) {
            if (GridLayout.WorldToCell( GameObject.FindGameObjectsWithTag( "Enemy" )[i].transform.position ) == tile) {
                return false;
            }
        }
        for (int i = 0; i < GameObject.FindGameObjectsWithTag( "Ally" ).Count(); i++) {
            if (GridLayout.WorldToCell( GameObject.FindGameObjectsWithTag( "Ally" )[i].transform.position ) == tile) {
                return false;
            }
        }

        if (watermap.GetTile( tile )) {
            Debug.Log( "Water Tile exists" );
            return false;
        } else if (tilemap.GetTile( tile )) {
            Debug.Log( watermap.gameObject.name );
            tilemap.SetColor( tile, moveColor ); // Add the tile if no obstructions
            return true;
        } else {
            return false;
        }
    }


    // Method responsible for displaying the cast range of an ability
    public void DisplayRange() {
        Vector3Int cellPosition = GridLayout.WorldToCell( GameObject.FindWithTag( "Player" ).transform.position );
        ReticleController.RangeArea.Clear();

        for (int x = (AbilityController.CastRange * -1); x <= AbilityController.CastRange; x++) {
            for (int y = (AbilityController.CastRange * -1); y <= AbilityController.CastRange; y++) {
                if (Mathf.Abs( x ) + Mathf.Abs( y ) <= AbilityController.CastRange) {
                    Vector3Int tile = new Vector3Int( cellPosition.x + x, cellPosition.y + y, cellPosition.z );
                    ReticleController.RangeArea.Add( tile );
                    ColorTile( tile, ReticleController.RangeColor );
                }
            }
        }
    }

    // Method responsible for displaying the cast reticle of an ability
    public void RenderReticle() {

        Vector3Int cellPosition = GridLayout.WorldToCell( InputController.CursorPosition );

        foreach (Vector3Int tile in ReticleController.CastArea) {
            ColorTile( tile, Color.white );
        }
        ReticleController.CastArea.Clear();
        if (ReticleController.RangeArea.Contains( cellPosition )) {
            for (int x = (AbilityController.XAxis * -1); x <= AbilityController.XAxis; x++) {
                for (int y = (AbilityController.XAxis * -1); y <= AbilityController.XAxis; y++) {
                    if ( Mathf.Abs( x ) + Mathf.Abs( y ) <= AbilityController.XAxis || AbilityController.Reticle == AbilityController.ReticleType.Square ) {
                        Vector3Int tile = new Vector3Int( cellPosition.x + x, cellPosition.y + y, cellPosition.z );
                        ReticleController.CastArea.Add( tile );
                        ColorTile( tile, ReticleController.ReticleColor );
                    }
                }
            }
        }
    }

    
    // Clears tile of color
    public void ClearTile( Vector3Int tile ) {
        watermap.SetColor( tile, Color.white );
        tilemap.SetColor( tile, Color.white );
    }
    #endregion

    // Clears a List of tiles of color
    public void ClearTiles( List<Vector3Int> tiles ) {
        foreach(Vector3Int tile in tiles) {
            ColorTile( tile, Color.white );
        }
    }
    
    // Colors ability animation tiles
    public void ColorTiles( List<Vector3Int> tiles, ColorMethod method ) {
        if (method == ColorMethod.Cast) {
            sequence += 1;
            foreach(Vector3Int tile in tiles) {
                CastTile( tile );
            }
        }
    }

    #endregion
}
