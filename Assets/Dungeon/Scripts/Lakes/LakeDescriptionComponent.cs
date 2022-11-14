using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelStageNamespace {
    public class LakeDescriptionComponent : MonoBehaviour
    {

        private LevelStageManagerComponent _levelStageManager;
        private GameObject _playerObject;
        private GameObject _wholeLake;

        //component that specifies the nature of a lake


        private LakeDescriptionSO _datasForThisLake;

        [SerializeField] public int NumberOfMallard;
        [SerializeField] public int NumberOfCoot;
        [SerializeField] public int NumberOfGoose;
        [SerializeField] public int NumberOfFish;       //we'll think about them in the future
        [SerializeField] public int NumberOfSeagull;    //we'll think about them in the future

        [SerializeField] private Transform NorthRiver;
        [SerializeField] private Transform SouthRiver;
        [SerializeField] private Transform WestRiver;
        [SerializeField] private Transform EastRiver;

        [Header("From where does the player come? North, south, east or west?")]
        [SerializeField] public EnumsDungeon.CompassDirection SpawnPlayer;

        [SerializeField] public EnumsDungeon.LakeDimension Dimension;

        public Dictionary<EnumsDungeon.BreadType, int> BreadToSpawnMap;

        private int _totalNumberOfBreadPiecesToSpawn;       //can be obtained scanning BreadToSpawnMap
        private int _totalNumberOfBreadPiecesToBeEaten;     //it's _totalNumberOfBreadPiecesToSpawn initially, but can get bigger if a new piece of bread is spawned because of stealing, for example
        private int _totalNumberOfBreadPiecesEaten;         //counts how many pieces of bread have been eaten in this lake

        private float _arrayBreadSpawnTime;
        private float _minIntervalTimeSpawnBread;
        private float _maxIntervaltimeSpawnBread;

        [SerializeField] public bool LakeCleared;

        private void Awake()
        {

            _levelStageManager = GameObject.Find("LevelStageManagerObject").GetComponent<LevelStageManagerComponent>();
            _datasForThisLake = _levelStageManager.GetLakeDescriptionSO();
            ManageRiversOfthisLake();
            _wholeLake = GameObject.Find("WholeLake");

            //first of all, a fade in effect
            _levelStageManager.FadeIn();

            //now we have to place the player in the correct spot
            _playerObject = GameObject.Find("DummyPlayer");     //TODO: change in actual player when you have it

            //if the room is the initial one and the player just arrived in the stage, we want it to be on the center of the room
            if (!_datasForThisLake.IsLakeCleared && _datasForThisLake.IsStartingRoom)
            {
                _playerObject.transform.position = new Vector3(0, 0, 0);
                _levelStageManager.SetLakeAsCleared();
            }
            else
            {
                //otherwise, we first of all set the correct position of the player
                switch (_datasForThisLake.PlayerSpawnDirection)
                {
                    case EnumsDungeon.CompassDirection.North:
                        _playerObject.transform.position = _wholeLake.transform.Find("Rivers/North").transform.position;
                        break;
                    case EnumsDungeon.CompassDirection.South:
                        _playerObject.transform.position = _wholeLake.transform.Find("Rivers/South").transform.position;
                        break;
                    case EnumsDungeon.CompassDirection.West:
                        _playerObject.transform.position = _wholeLake.transform.Find("Rivers/West").transform.position;
                        break;
                    case EnumsDungeon.CompassDirection.East:
                        _playerObject.transform.position = _wholeLake.transform.Find("Rivers/East").transform.position;
                        break;
                }
            }

        }








        private void ManageRiversOfthisLake()
        {
            GameObject wholeLake = GameObject.Find("WholeLake");
            LakeDescriptionComponent lakeDesc = wholeLake.GetComponent<LakeDescriptionComponent>();
            GameObject northRiver = wholeLake.transform.Find("Rivers/North").gameObject;
            GameObject southRiver = wholeLake.transform.Find("Rivers/South").gameObject;
            GameObject westRiver = wholeLake.transform.Find("Rivers/West").gameObject;
            GameObject eastRiver = wholeLake.transform.Find("Rivers/East").gameObject;
            if(_datasForThisLake.HasNorthRiver == false)
            {
                //if the north lake is not present, we obscure its sprite, and leave the colliders as they are
                var t = northRiver.transform.Find("Sprite").gameObject.GetComponent<Transform>().localScale;
                northRiver.transform.Find("Sprite").gameObject.GetComponent<Transform>().localScale = new Vector3(0, t.y, t.z);
            }
            else
            {
                //otherwise, we leave the sprite as it is, but we remove the collider (We do the same for the other rivers)
                northRiver.transform.Find("BlockingCollider").gameObject.SetActive(false);
                northRiver.transform.Find("TriggerEnteredCollider").gameObject.SetActive(false);

            }
            if (_datasForThisLake.HasSouthRiver == false)
            {
                var t = southRiver.transform.Find("Sprite").gameObject.GetComponent<Transform>().localScale;
                southRiver.transform.Find("Sprite").gameObject.GetComponent<Transform>().localScale = new Vector3(0, t.y, t.z);
            }
            else
            {
                southRiver.transform.Find("BlockingCollider").gameObject.SetActive(false);
                southRiver.transform.Find("TriggerEnteredCollider").gameObject.SetActive(false);
            }
            if (_datasForThisLake.HasWestRiver == false)
            {
                var t = westRiver.transform.Find("Sprite").gameObject.GetComponent<Transform>().localScale;
                westRiver.transform.Find("Sprite").gameObject.GetComponent<Transform>().localScale = new Vector3(t.x, 0, t.z);
            }
            else
            {
                westRiver.transform.Find("BlockingCollider").gameObject.SetActive(false);
                westRiver.transform.Find("TriggerEnteredCollider").gameObject.SetActive(false);
            }
            if (_datasForThisLake.HasEastRiver == false)
            {
                var t = eastRiver.transform.Find("Sprite").gameObject.GetComponent<Transform>().localScale;
                eastRiver.transform.Find("Sprite").gameObject.GetComponent<Transform>().localScale = new Vector3(t.x, 0, t.z);
            }
            else
            {
                eastRiver.transform.Find("BlockingCollider").gameObject.SetActive(false);
                eastRiver.transform.Find("TriggerEnteredCollider").gameObject.SetActive(false);
            }
        }





        // Start is called before the first frame update
        void Start()
        {

           



        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}



