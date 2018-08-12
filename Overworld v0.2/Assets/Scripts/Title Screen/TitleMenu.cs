using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class TitleMenu : MonoBehaviour {

    // Update is called once per frame
    [TextArea(15,30)]
    public string changelog;
    DataController data;

    private void Start() {
        data = new DataLoader().LoadData();
        if (!data.ChangeLogSeen()) {
            GameObject.Find( "ChangesTitle" ).GetComponent<TextMeshProUGUI>().text = "Changes for v" + data.GetVersionNumber();
            GameObject.Find("Changes").GetComponent<TextMeshProUGUI>().text = changelog;
        } else {
            GameObject.Find( "ChangesPanel" ).SetActive(false);
        }
        
    }

    void Update () {
        if (Input.GetMouseButtonDown( 0 ) && data.ChangeLogSeen()) {
            SceneManager.LoadScene( "Main" );
        }
	}

    public void ChangeLogOk() {
        GameObject.Find( "ChangesPanel" ).SetActive( false );
        data.SawChangeLog();
    }
}
