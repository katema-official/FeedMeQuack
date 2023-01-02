using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace HUDNamespace
{

    public class HUDManager : MonoBehaviour
    {

        [SerializeField]
        private TMP_Text speedValueText, eatingSpeedValueText, mouthSizeValueText, chewingRate,
            dashCDValueText, stealCDValueText, spitCDValueText;

        [SerializeField] private TMP_Text breadPointsValueText, digestedBreadPointsValueText, goalValueText, levelValueText;
        // Start is called before the first frame update

        public void ChangeText(textFields field, float value)
        {
            //per cd: grigio se è in cd, bianco se è pronta
            switch (field)
            {
                case textFields.speed:
                    speedValueText.text = value.ToString() + " m/s";
                    break;
                case textFields.eatingSpeed:
                    eatingSpeedValueText.text = value.ToString()+ " m/s";
                    break;
                case textFields.mouthSize:
                    mouthSizeValueText.text = value.ToString()+ " BP/Bite";
                    break;
                case textFields.chewingRate:
                    chewingRate.text = value.ToString()+ " BP/s";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(field), field, null);
            }
        }

        public void ChangeBreadPointsCollectedText(int value)
        {
            breadPointsValueText.text = value.ToString();
        }

        public void ChangeDigestedBreadPointsCollectedText(int value)
        {
            digestedBreadPointsValueText.text = value.ToString();
        }

        public void UpdateSkillCooldown(textFields skillType, float newValue)
        {
            switch (skillType)
            {
                case textFields.dashCD:
                    dashCDValueText.text = newValue.ToString()+" sec.";
                    if (newValue == 0){
                        dashCDValueText.text = "Available";
                        dashCDValueText.color= Color.green;
                    }
                    else{
                        dashCDValueText.color= Color.white;
                    }
                    break;
                case textFields.stealCD:
                    stealCDValueText.text = newValue.ToString()+" sec.";
                    if (newValue == 0){
                        stealCDValueText.text = "Available";
                        stealCDValueText.color= Color.green;
                    }
                    else{
                        stealCDValueText.color= Color.white;
                    }
                    break;
                case textFields.spitCD:
                    spitCDValueText.text = newValue.ToString()+" sec.";
                    if (newValue == 0){
                        spitCDValueText.text = "Available";
                        spitCDValueText.color= Color.green;
                    }
                    else{
                        spitCDValueText.color= Color.white;
                    }
                    break;
            }
        }

        public void ChangeGoalText(int value)
        {
            goalValueText.text = value.ToString();
        }

        public void ChangeLevelText(String value){
            levelValueText.text = value;
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
}