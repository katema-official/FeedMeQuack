using System;
using System.Collections;
using System.Collections.Generic;
using BreadNamespace;
using CodeMonkey.Utils;
using DuckEnemies;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

namespace HUDNamespace
{

    public class HUDManager : MonoBehaviour
    {
        private Windows_QuestPointer _windowQuestPointer;
        private bool isTabPressed;
        public List<GameObject> breadsInWater, breadsOutsideScreen;
        private int goal=0, currentPoints=0;
        public Camera camera;
        [SerializeField] private GameObject ToggableHUD;
        [SerializeField]
        private TMP_Text speedValueText, eatingSpeedValueText, mouthSizeValueText, chewingRate,
            dashCDValueText, stealCDValueText, spitCDValueText, dashCDValueTextCopy, stealCDValueTextCopy, spitCDValueTextCopy;

        [SerializeField] private Texture dashOn, dashOff, stealOn, stealOff, spitOn, spitOff;
        [SerializeField] private RawImage dash1, dash2, steal1, steal2, spit1, spit2;

        [SerializeField] private TMP_Text breadPointsValueText, digestedBreadPointsValueText, PressTabText, levelValueText, stageValueText;

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
                    value = 1f / value;
                    string s = String.Format("{0:0.00}", value);
                    chewingRate.text = s + " BP/s";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(field), field, null);
            }
        }

        public void ChangeBreadPointsCollectedText(int value){
            Color orange = new Color(255, 169, 106);
            breadPointsValueText.text = value.ToString()+"/"+ "<color=orange>"+goal.ToString() + " BP"+"</color>";
        }

        public void ChangeDigestedBreadPointsCollectedText(int value)
        {
            digestedBreadPointsValueText.text = value.ToString() + " DBP";
        }

        public void UpdateSkillCooldown(textFields skillType, float newValue, float maxDuration = 0f)
        {
            string s = String.Format("{0:0.0}", newValue);
            switch (skillType)
            {
                case textFields.dashCD:
                    dashCDValueText.text = s + " sec.";
                    dashCDValueTextCopy.text = s + " sec.";
                    if (newValue == 0){
                        dashCDValueText.text = "Ready (" + maxDuration + "s)";
                        dashCDValueTextCopy.text = "Ready";
                        dashCDValueText.color = Color.green;
                        dashCDValueTextCopy.color = Color.green;
                        dash1.texture = dashOn;
                        dash2.texture = dashOn;

                    }
                    else{
                        dashCDValueText.color= Color.white;
                        dashCDValueTextCopy.color= Color.white;
                        dash1.texture = dashOff;
                        dash2.texture = dashOff;
                    }
                    break;
                case textFields.stealCD:
                    stealCDValueText.text = s + " sec.";
                    stealCDValueTextCopy.text = s + " sec.";
                    if (newValue == 0){
                        stealCDValueText.text = "Ready (" + maxDuration + "s)";
                        stealCDValueTextCopy.text = "Ready";
                        stealCDValueText.color = Color.green;
                        stealCDValueTextCopy.color = Color.green;
                        steal1.texture = stealOn;
                        steal2.texture = stealOn;
                    }
                    else{
                        stealCDValueText.color= Color.white;
                        stealCDValueTextCopy.color= Color.white;
                        steal1.texture = stealOff;
                        steal2.texture = stealOff;
                    }
                    break;
                case textFields.spitCD:
                    spitCDValueText.text = s + " sec.";
                    spitCDValueTextCopy.text = s + " sec.";
                    if (newValue == 0){
                        spitCDValueText.text = "Ready (" + maxDuration + "s)";
                        spitCDValueTextCopy.text = "Ready";
                        spitCDValueText.color = Color.green;
                        spitCDValueTextCopy.color = Color.green;
                        spit1.texture = spitOn;
                        spit2.texture = spitOn;
                    }
                    else{
                        spitCDValueText.color= Color.white;
                        spitCDValueTextCopy.color= Color.white;
                        spit1.texture = spitOff;
                        spit2.texture = spitOff;
                    }
                    break;
            }
        }

        public void ChangeGoalText(int value){
            goal = value;
            ChangeBreadPointsCollectedText(0);
            //goalValueText.text = value.ToString();

        }

        public void ChangeLevelText(String value){
            string[] parts = value.Split('.');
            int level = int.Parse(parts[0]);
            int stage = int.Parse(parts[1]);
            levelValueText.text = level.ToString();
            stageValueText.text = stage.ToString();
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
        
        void Update()
        {
            var gamepad = Gamepad.current;
            if (Input.GetKeyDown(KeyCode.Tab) || (gamepad != null && gamepad.selectButton.wasPressedThisFrame))
            {
                isTabPressed = !isTabPressed;
            }

            if (isTabPressed){
                ToggableHUD.SetActive(true);
                PressTabText.gameObject.SetActive(false);
            }
            else{
                ToggableHUD.SetActive(false);
                PressTabText.gameObject.SetActive(true);
            }

//            if (breadsInWater.Count > 0)
//                DisplayArrows();
            UpdateBreadList();
        }

        private void UpdateBreadList(){
            BreadInWaterComponent[] breads = FindObjectsOfType<BreadInWaterComponent>();
            breadsInWater = new List<GameObject>();
            for (int i = 0; i < breads.Length; i++){
                GameObject bread = breads[i].gameObject;
                if (!breadsInWater.Contains(bread)){
                    breadsInWater.Add(bread);
                }
            }
            DisplayArrows();
        }

        private void DisplayArrows(){
            foreach (var bread in breadsInWater){
                if (CheckIfBreadOutsideCamera(bread)){ //okay if bread outside camera
                    if (!breadsOutsideScreen.Contains(bread)){
                        breadsOutsideScreen.Add(bread);
                        var foo=_windowQuestPointer.CreatePointer(bread.transform.position);
                        FunctionUpdater.Create(() => {
                            if (bread==null || !CheckIfBreadOutsideCamera(bread)) {
                                _windowQuestPointer.DestroyPointer(foo);
                                breadsOutsideScreen.Remove(bread);
                                return true;
                            } else {
                                return false;
                            }
                        });
                    }
                    
                }
            }
        }

        private void DrawArrowToBread(GameObject bread){
            _windowQuestPointer.CreatePointer(bread.transform.position);
        }

        public void NotifyHUDManagerOfNewBread(GameObject newBread){
            if (breadsInWater == null){
                breadsInWater = new List<GameObject>();
            }
            breadsInWater.Add(newBread);
        }

        public void BreadDeleted(GameObject breadDeleted){
            breadsInWater.Remove(breadDeleted);
        }

        private bool CheckIfBreadOutsideCamera(GameObject gameObject){
            Vector3 viewportPoint = camera.WorldToViewportPoint(gameObject.transform.position);
            return !(viewportPoint.x >= 0 && viewportPoint.x <= 1 && viewportPoint.y >= 0 && viewportPoint.y <= 1);
        }

        void Start(){
            breadsOutsideScreen = new List<GameObject>();
            _windowQuestPointer = FindObjectOfType<Windows_QuestPointer>();
            camera = FindObjectOfType<Player.PlayerController>().GetCamera();
        }
    }
}