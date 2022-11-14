using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelStageNamespace {
    public class LakeDescriptionComponent : MonoBehaviour
    {

        private LevelStageManagerComponent _levelStageManager;

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

        }








        private void ManageRiversOfthisLake()
        {
            GameObject wholeLake = GameObject.Find("WholeLake");
            LakeDescriptionComponent lakeDesc = wholeLake.GetComponent<LakeDescriptionComponent>();
            GameObject northRiver = wholeLake.transform.Find("Rivers/North").gameObject;
            northRiver.SetActive(false);    //vuoi solo togliere lo sprite zio
            Debug.LogFormat("lakeDesc = {0}", lakeDesc.NumberOfCoot);
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



