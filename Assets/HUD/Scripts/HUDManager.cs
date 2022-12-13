using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HUDManager : MonoBehaviour
{

    [SerializeField] private TMP_Text speedValueText, eatingSpeedValueText, mouthSizeValueText, dashCDValueText, stealCDValueText, spitCDValueText;

    [SerializeField] private TMP_Text breadPointsValueText, digestedBreadPointsValueText, goalValueText;
    // Start is called before the first frame update

    public void ChangeText(float value, textFields field){
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
            case textFields.dashCD:
                dashCDValueText.text = value.ToString();
                break;
            case textFields.stealCD:
                stealCDValueText.text = value.ToString();
                break;
            case textFields.spitCD:
                spitCDValueText.text = value.ToString();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(field), field, null);
        }
    }

    public void ChangeBreadPointsCollectedText(int value){
        breadPointsValueText.text = "BP: " + value;
    }

    public void ChangeDigestedBreadPointsCollectedText(int value){
        digestedBreadPointsValueText.text = "DBP: " + value;
    }

    public void UpdateSkillCooldown(){
        
    }

    public void ChangeGoalText(int value){
        goalValueText.text = "Goal: " + value;
    }

    public enum textFields
    {
        speed,
        eatingSpeed,
        mouthSize,
        dashCD,
        stealCD,
        spitCD
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
