using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelStageNamespace
{

    public class EnterPlayerInLakeComponent : MonoBehaviour
    {

        private LakeDescriptionComponent _lakeDescriptionComponent;
        private LevelStageManagerComponent _levelStageManagerComponent;
        private List<GameObject> _enterCollidersList;

        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (_levelStageManagerComponent.GetLakeDescriptionSO().IsLakeCleared == false && collider.gameObject.tag == "Player")
            {
                //when this triggers, the player has entered the lake.
                //at this point, there are different things to do:

                //first:, to avoid that this happens again, we have to disable all TriggerEnteredCollider objects
                for(int i = 0; i < _enterCollidersList.Count; i++)
                {
                    _enterCollidersList[i].SetActive(false);
                }

                //second: close all the lakes
                _lakeDescriptionComponent.CloseLakesWithAnimation();

                //then, we have to start generating the bread for this lake. The component that will do so is
                //in the WholeLake object, and the specific component will be the LakeDescriptionComponent
                _lakeDescriptionComponent.StartThrowingAllTheBread();

                //MUSIC: since this is a new "combat" room, reproduce combat music
                
                string musicName = "Combat" + _levelStageManagerComponent.GetCurrentLevelIndex().ToString();

                if (!(_levelStageManagerComponent.GetCurrentLevelIndex() == 3 && _levelStageManagerComponent.GetCurrentStageIndex() == 3)){
                    Music.UniversalAudio.PlayMusic(musicName, false);
                }
                else
                {
                    Music.UniversalAudio.PlayMusic("CombatFinal", false);
                }
                
            }
        }




        // Start is called before the first frame update
        void Start()
        {
            _enterCollidersList = new List<GameObject>();
            _lakeDescriptionComponent = GameObject.Find("WholeLake").GetComponent<LakeDescriptionComponent>();//transform.parent.parent.parent.parent.gameObject.GetComponent<LakeDescriptionComponent>();
            _levelStageManagerComponent = GameObject.Find("LevelStageManagerObject").GetComponent<LevelStageManagerComponent>();
            _enterCollidersList.Add(_lakeDescriptionComponent.transform.Find("Water/WaterBorder/Rivers/RiverNorth/TriggerEnteredCollider").gameObject);           
            _enterCollidersList.Add(_lakeDescriptionComponent.transform.Find("Water/WaterBorder/Rivers/RiverSouth/TriggerEnteredCollider").gameObject);
            _enterCollidersList.Add(_lakeDescriptionComponent.transform.Find("Water/WaterBorder/Rivers/RiverWest/TriggerEnteredCollider").gameObject);       
            _enterCollidersList.Add(_lakeDescriptionComponent.transform.Find("Water/WaterBorder/Rivers/RiverEast/TriggerEnteredCollider").gameObject);

        }


    }
}