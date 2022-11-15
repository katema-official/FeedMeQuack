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
            if (_levelStageManagerComponent.GetLakeDescriptionSO().IsLakeCleared == false && collider.gameObject.name == "DummyPlayer")
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
            }
        }




        // Start is called before the first frame update
        void Start()
        {
            _enterCollidersList = new List<GameObject>();
            _lakeDescriptionComponent = transform.parent.parent.parent.gameObject.GetComponent<LakeDescriptionComponent>();
            _levelStageManagerComponent = GameObject.Find("LevelStageManagerObject").GetComponent<LevelStageManagerComponent>();
            _enterCollidersList.Add(_lakeDescriptionComponent.transform.Find("Rivers/North/TriggerEnteredCollider").gameObject);           
            _enterCollidersList.Add(_lakeDescriptionComponent.transform.Find("Rivers/South/TriggerEnteredCollider").gameObject);
            _enterCollidersList.Add(_lakeDescriptionComponent.transform.Find("Rivers/West/TriggerEnteredCollider").gameObject);       
            _enterCollidersList.Add(_lakeDescriptionComponent.transform.Find("Rivers/East/TriggerEnteredCollider").gameObject);

        }


    }
}