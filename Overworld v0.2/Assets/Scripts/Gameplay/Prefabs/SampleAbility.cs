using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SampleAbility : MonoBehaviour {

    public int castRange;
    public int xAxis;
    //[SerializeField] int yAxis; dont need this yet
    public float abilityValue; // damage or healing
    public int reticleType; // 0 is none, 1 is diamond, 2 is square
    public int targetType; // 0 is enemy, 1 is ally, 2 is self
    public int abilityType; // 0 is damage, 1 is healing, 2 is mobility
    public int cost;
    public int cooldown;
    public int currentCooldown = 0;
    [SerializeField] TextMeshProUGUI abilityName;
    [SerializeField] TextMeshProUGUI cooldownDisp;

    private SampleAbilityBar sampleAbilityBar;
    private BattleAbility ability;

    public void Setup( BattleAbility currentAbility, SampleAbilityBar currentAbilityBar ) {

        castRange = currentAbility.range;
        cost = currentAbility.cost;
        xAxis = currentAbility.xAxis;
        abilityValue = currentAbility.value;
        reticleType = currentAbility.reticleType;
        targetType = currentAbility.targetType;
        abilityType = currentAbility.abilityType;
        string img = currentAbility.abilityId.ToString() + ".jpg";
        Image abilityIcon = gameObject.GetComponentsInChildren<Image>()[1];
        Sprite abilitySprite = IMG2Sprite.instance.LoadNewSprite( Application.streamingAssetsPath + "/Abilities/" + img );
        abilityIcon.sprite = abilitySprite;
        abilityName.text = currentAbility.name;
        cooldown = currentAbility.cooldown;
        ability = currentAbility;
        sampleAbilityBar = currentAbilityBar;

    }

    private void OnGUI() {
        if(currentCooldown > 0) {
            cooldownDisp.text = currentCooldown.ToString();
            gameObject.GetComponent<Button>().interactable = false;
        } else {
            gameObject.GetComponent<Button>().interactable = true;
            cooldownDisp.text = "";
        }
    }


    public void AbilityButton() {
        TileController.Instance.ClearTiles( ReticleController.CastArea );
        AbilityController.Instance.Init( ability, this );
    }

    public void OnCooldown() {
        currentCooldown = cooldown;
    }
}
