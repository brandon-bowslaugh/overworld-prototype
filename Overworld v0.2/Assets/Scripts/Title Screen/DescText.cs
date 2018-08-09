using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DescText : MonoBehaviour {

    [SerializeField] TextMeshProUGUI descText;
    [SerializeField] string description;

	// Update is called once per frame
	void Start () {

        StartCoroutine( TextAnimation() );

    }

    private IEnumerator TextAnimation() {
        StartCoroutine( ShowText() );
        yield return new WaitForSeconds( 0.25f );
        StartCoroutine( HideText() );
        yield return null;
    }

    private IEnumerator ShowText() {
        while (true) {
            yield return new WaitForSeconds( 0.75f );
            descText.text = description;
            Debug.Log( "SHOW TEXT" );
        }
    }
    private IEnumerator HideText() {
        while (true) {
            yield return new WaitForSeconds( 1.5f );
            Debug.Log( "HIDE TEXT" );
            descText.text = "";
        }
    }


}
