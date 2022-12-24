using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LevelStageNamespace;


namespace DuckEnemies
{
    public class IdentifyPlayerComponent : MonoBehaviour
    {
        private float _circle1PlayerRadius;                  //the radius of the three circles that define if a enemy
        private float _circle2PlayerRadius;                  //duck saw the player
        private float _circle3PlayerRadius;
        private float _circle1PlayerProbability;             //probabilities that a duck sees the player
        private float _circle2PlayerProbability;             //when it enters in a particular circle
        private float _circle3PlayerProbability;

        //these booleans tell us if the duck refused to see the player in a certain circle
        //TRUE = I ignore the player that passes in that circle, FALSE = I don't immediately ignore the player in that circle
        private bool _refusedPlayerCircle1 = false;
        private bool _refusedPlayerCircle2 = false;
        private bool _refusedPlayerCircle3 = false;
        private GameObject _identifiedPlayer = null;

        private PlayerCircleComponent _playerCircleComponent1;
        private PlayerCircleComponent _playerCircleComponent2;
        private PlayerCircleComponent _playerCircleComponent3;

        private LakeDescriptionComponent _lakeDescriptionComponent;


        void Awake()
        {
            _playerCircleComponent1 = transform.Find("PlayerCollider1").GetComponent<PlayerCircleComponent>();
            _playerCircleComponent2 = transform.Find("PlayerCollider2").GetComponent<PlayerCircleComponent>();
            _playerCircleComponent3 = transform.Find("PlayerCollider3").GetComponent<PlayerCircleComponent>();
            _lakeDescriptionComponent = GameObject.Find("WholeLake").GetComponent<LakeDescriptionComponent>();
        }

        //Method used by the children circle colliders equipped with a PlayerCircleComponent to notify this
        //component of the fact that the player has been identified (that means, has passed in that circle)
        public void NotifyPlayerIdentified(GameObject playerGO, int idCircle)
        {
            if (_identifiedPlayer != null) return;
            float r = Random.Range(0f, 1f);
            switch (idCircle)
            {
                case 1:
                    if (_refusedPlayerCircle1 == false)
                    {
                        if (r < _circle1PlayerProbability)
                        {
                            SaveIdentifiedPlayer(playerGO);
                        }
                        else
                        {
                            _refusedPlayerCircle1 = true;
                            _refusedPlayerCircle2 = true;
                            _refusedPlayerCircle3 = true;
                        }
                    }
                    break;
                case 2:
                    if (_refusedPlayerCircle2 == false)
                    {
                        if (r < _circle2PlayerProbability)
                        {
                            SaveIdentifiedPlayer(playerGO);
                        }
                        else
                        {
                            _refusedPlayerCircle2 = true;
                            _refusedPlayerCircle3 = true;
                        }
                    }
                    break;
                case 3:
                    if (_refusedPlayerCircle3 == false)
                    {
                        if (r < _circle3PlayerProbability)
                        {
                            SaveIdentifiedPlayer(playerGO);
                        }
                        else
                        {
                            _refusedPlayerCircle3 = true;
                        }
                    }
                    break;
                default:
                    break;
            }

        }

        private void SaveIdentifiedPlayer(GameObject playerGO)
        {
            _identifiedPlayer = playerGO;
        }

        public bool IsThereAnObjectivePlayer()
        {
            return _identifiedPlayer != null;

        }

        //function to call whenever we want to clean the data structures.
        //Basically, call this function when you want the duck to be aware again of
        //the Player in this lake.
        public void ForgetAboutPlayer()
        {
            _refusedPlayerCircle1 = false;
            _refusedPlayerCircle2 = false;
            _refusedPlayerCircle3 = false;
            _identifiedPlayer = null;
        }


        public GameObject GetObjectivePlayer()
        {
            return _identifiedPlayer;
        }

    }
}