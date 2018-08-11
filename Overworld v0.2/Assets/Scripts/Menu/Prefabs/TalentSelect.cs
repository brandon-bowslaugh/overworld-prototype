using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TalentSelect : MonoBehaviour {

    public TextMeshProUGUI talentName;
    public TextMeshProUGUI type;
    public TextMeshProUGUI school;
    public TextMeshProUGUI method;
    public TextMeshProUGUI range;
    public TextMeshProUGUI subtree;

    private MenuTalentData talent;

    // Use this for initialization
    public void Setup ( MenuTalentData currentTalent, TalentSelectLoader loader, string talentTreeName, string subTreeName ) {
        
        talent = currentTalent;

        talentName.text = currentTalent.name;
        switch (currentTalent.type) {
            case 0:
                type.text = "Ability";
                /* Get values for this from GetAbilityData
                school.text = "Physical";
                method.text = "Targeted";
                range.text ;
                 */
                break;
            case 1:
                type.text = "Passive";
                /* Get values for this from GetPassiveData
                school.text = "Physical";
                method.text = "Targeted";
                range.text ;
                 */
                break;
            case 3:
                type.text = "Ultimate";
                /* Get values for this from GetAbilityData
                school.text = "Physical";
                method.text = "Targeted";
                range.text ;
                 */
                break;
        }
        
        // Temporary
        school.text = "Physical";
        method.text = "Targeted";
        range.text = "Melee";
        subtree.text = subTreeName + " (" + talentTreeName + ")";

        gameObject.GetComponent<Button>().onClick.RemoveAllListeners();
        gameObject.GetComponent<Button>().onClick.AddListener(SelectTalent);

    }

    public void SelectTalent() {

        GameObject.Find( "TalentSelectLoader" ).GetComponent<TalentSelectLoader>().SelectTalent( talent.type, talent );
        
    }

}
