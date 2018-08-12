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
    [SerializeField] TextMeshProUGUI abilityName;

    private SampleAbilityBar sampleAbilityBar;
    private BattleAbility ability;

    public void Setup( BattleAbility currentAbility, SampleAbilityBar currentAbilityBar ) {

        castRange = currentAbility.range;
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
        ability = currentAbility;
        sampleAbilityBar = currentAbilityBar;

    }


    public void AbilityButton() {
        TileController.Instance.ClearTiles( ReticleController.CastArea );
        AbilityController.Instance.Init( ability );
    }
}
