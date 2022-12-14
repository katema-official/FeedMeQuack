using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HUDManager : MonoBehaviour
{

    [SerializeField] private TMP_Text speedValueText, eatingSpeedValueText, mouthSizeValueText, chewingRate,
        dashCDValueText, stealCDValueText, spitCDValueText;

    [SerializeField] private TMP_Text breadPointsValueText, digestedBreadPointsValueText, goalValueText;
    // Start is called before the first frame update

    public void ChangeText(textFields field, float value){
        //per cd: grigio se è in cd, bianco se è pronta
        switch (field){
            case textFields.speed:
                speedValueText.text = value.ToString();
                break;
            case textFields.eatingSpeed:
                eatingSpeedValueText.text = value.ToString();
                break;
            case textFields.mouthSize:
                mouthSizeValueText.text = value.ToString();
                break;
            case textFields.chewingRate:
                chewingRate.text = value.ToString();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(field), field, null);
        }
    }

    public void ChangeBreadPointsCollectedText(int value){
        breadPointsValueText.text = value.ToString();
    }

    public void ChangeDigestedBreadPointsCollectedText(int value){
        digestedBreadPointsValueText.text = value.ToString();
    }

    public void UpdateSkillCooldown(textFields skillType, float newValue){
        switch (skillType){
            case textFields.dashCD:
                dashCDValueText.text = newValue.ToString();
                break;
            case textFields.stealCD:
                stealCDValueText.text = newValue.ToString();
                break;
            case textFields.spitCD:
                spitCDValueText.text = newValue.ToString();
                break;
        }
    }

    public void ChangeGoalText(int value){
        goalValueText.text = value.ToString();
    }

    public enum textFields
    {
        speed,
        eatingSpeed,
        mouthSize,
        chewingRate,
        dashCD,
        stealCD,
        spitCD
    }
}
