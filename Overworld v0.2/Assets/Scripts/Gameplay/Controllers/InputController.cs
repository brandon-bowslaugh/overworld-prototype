using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class is responsible for determining cursor position
// Functionality may be added further into the project
public class InputController : MonoBehaviour {

    #region Create Singleton
    private static InputController _instance;

    public static InputController Instance {
        get {
            if (_instance == null) {
                GameObject go = new GameObject( "InputController" );
                go.AddComponent<InputController>();
            }
            return _instance;
        }
    }

    void Awake() {
        _instance = this;
    }
    #endregion


    private Vector3 _tilePosition;
    private GameObject hoverCursor;
    public static Vector3 CursorPosition { get; private set; }
    public static Vector3Int CursorPositionInt { get; private set; }
    
    public void Setup() {
        hoverCursor = GameObject.FindWithTag( "HoverCursor" );
    }

    private void FixedUpdate() {
        _tilePosition = Camera.main.ScreenToWorldPoint( Input.mousePosition );
        CursorPositionInt = TileController.GridLayout.WorldToCell(_tilePosition);
        // re-orient the cursor to the center of the square
        _tilePosition.x = CursorPositionInt.x  + 0.5f;
        _tilePosition.y = CursorPositionInt.y + 0.5f;
        _tilePosition.z = 0;
        CursorPosition = _tilePosition;
        hoverCursor.transform.position = _tilePosition;
    }
}
