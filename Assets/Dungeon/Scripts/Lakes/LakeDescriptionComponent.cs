using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelStageNamespace {
    public class LakeDescriptionComponent : MonoBehaviour
    {

        private LevelStageManagerComponent _levelStageManager;
        private GameObject _playerObject;

        //component that specifies the nature of a lake

        private LakeDescriptionSO _lakeDescriptionForThisLake;
        private BreadSpawnSO _breadSpawnForThisLake;


        public float OffsetXTerrain;
        public float OffsetYTerrain;
        public float OffsetXLake;
        public float OffsetYLake;

        private GameObject _lake;
        private GameObject _grass;


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

        private List<float> _arrayBreadSpawnTime;                       //position i: says that the i-th bread will spawn elem[i] time after the time the i-1 th element spawned
        private List<EnumsDungeon.BreadType> _arrayBreadSpawnType;      //position i: says what kind of bread needs to be spawned as i-th bread for this lake
        private float _minIntervalTimeSpawnBread;
        private float _maxIntervaltimeSpawnBread;

        [SerializeField] public bool LakeCleared;

        private void Awake()
        {

            _levelStageManager = GameObject.Find("LevelStageManagerObject").GetComponent<LevelStageManagerComponent>();
            _lakeDescriptionForThisLake = _levelStageManager.GetLakeDescriptionSO();
            ManageRiversOfthisLake();
            _breadSpawnForThisLake = _levelStageManager.GetBreadSpawnSO();
            _lake = transform.Find("Water").gameObject;
            _grass = transform.Find("Terrain").gameObject;

            //first of all, a fade in effect
            _levelStageManager.FadeIn();

            //now we have to place the player in the correct spot
            _playerObject = GameObject.Find("DummyPlayer");     //TODO: change in actual player when you have it

            //if the room is the initial one and the player just arrived in the stage, we want it to be on the center of the room
            if (!_lakeDescriptionForThisLake.IsLakeCleared && _lakeDescriptionForThisLake.IsStartingRoom)
            {
                _playerObject.transform.position = new Vector3(0, 0, 0);
                _levelStageManager.SetLakeAsCleared();
            }
            else
            {
                //otherwise, we first of all set the correct position of the player
                SpawnPlayer = _lakeDescriptionForThisLake.PlayerSpawnDirection;
                switch (SpawnPlayer)
                {
                    case EnumsDungeon.CompassDirection.North:
                        _playerObject.transform.position = transform.Find("Rivers/North").transform.position;
                        break;
                    case EnumsDungeon.CompassDirection.South:
                        _playerObject.transform.position = transform.Find("Rivers/South").transform.position;
                        break;
                    case EnumsDungeon.CompassDirection.West:
                        _playerObject.transform.position = transform.Find("Rivers/West").transform.position;
                        break;
                    case EnumsDungeon.CompassDirection.East:
                        _playerObject.transform.position = transform.Find("Rivers/East").transform.position;
                        break;
                }
            }

            //at this point, there are two cases:
            //-Either this lake is visited for the first time and needs to generate bread, ducks, and so no
            //-Or it was previously completed and no more actions need to be carried out
            if (!_lakeDescriptionForThisLake.IsLakeCleared)
            {
                //in this situation, we need to generate all the stuff in the lake.
                //First of all: let's generate the times of spawn of the bread pieces.
                GenerateArrayBreadSpawn();

                //then, we can place in the map the enemies. Their position needs to be inside the lake, and
                //not too close to the player
                GenerateEnemies();
            }

        }







        


        private void ManageRiversOfthisLake()
        {
            LakeDescriptionComponent lakeDesc = GetComponent<LakeDescriptionComponent>();
            GameObject northRiver = transform.Find("Rivers/North").gameObject;
            GameObject southRiver = transform.Find("Rivers/South").gameObject;
            GameObject westRiver = transform.Find("Rivers/West").gameObject;
            GameObject eastRiver = transform.Find("Rivers/East").gameObject;
            if(_lakeDescriptionForThisLake.HasNorthRiver == false)
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
            if (_lakeDescriptionForThisLake.HasSouthRiver == false)
            {
                var t = southRiver.transform.Find("Sprite").gameObject.GetComponent<Transform>().localScale;
                southRiver.transform.Find("Sprite").gameObject.GetComponent<Transform>().localScale = new Vector3(0, t.y, t.z);
            }
            else
            {
                southRiver.transform.Find("BlockingCollider").gameObject.SetActive(false);
                southRiver.transform.Find("TriggerEnteredCollider").gameObject.SetActive(false);
            }
            if (_lakeDescriptionForThisLake.HasWestRiver == false)
            {
                var t = westRiver.transform.Find("Sprite").gameObject.GetComponent<Transform>().localScale;
                westRiver.transform.Find("Sprite").gameObject.GetComponent<Transform>().localScale = new Vector3(t.x, 0, t.z);
            }
            else
            {
                westRiver.transform.Find("BlockingCollider").gameObject.SetActive(false);
                westRiver.transform.Find("TriggerEnteredCollider").gameObject.SetActive(false);
            }
            if (_lakeDescriptionForThisLake.HasEastRiver == false)
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

        private void GenerateArrayBreadSpawn()
        {
            _arrayBreadSpawnTime = new List<float>();
            _arrayBreadSpawnType = new List<EnumsDungeon.BreadType>();
            _minIntervalTimeSpawnBread = _breadSpawnForThisLake.MinSpawnTime;
            _maxIntervaltimeSpawnBread = _breadSpawnForThisLake.MaxSpawnTime;

            //based on the bread we decided to spawn, we initialize the _arrayBreadSpawnType
            _totalNumberOfBreadPiecesToSpawn = 0;
            foreach (KeyValuePair<EnumsDungeon.BreadType, int> entry in _lakeDescriptionForThisLake.BreadToSpawnMap)
            {
                for (int i = 0; i < entry.Value; i++)
                {
                    _arrayBreadSpawnType.Add(entry.Key);
                }
                _totalNumberOfBreadPiecesToSpawn += entry.Value;
            }

            //we then need to shuffle it
            for (int i = _arrayBreadSpawnType.Count - 1; i > 0; i--)
            {
                int randomIndex = Random.Range(0, i);
                EnumsDungeon.BreadType temp = _arrayBreadSpawnType[i];
                _arrayBreadSpawnType[i] = _arrayBreadSpawnType[randomIndex];
                _arrayBreadSpawnType[randomIndex] = temp;
            }

            //we now generate the random interval times between pieces of bread thrown
            for (int i = 0; i < _totalNumberOfBreadPiecesToSpawn; i++)
            {
                _arrayBreadSpawnTime.Add(Random.Range(_minIntervalTimeSpawnBread, _maxIntervaltimeSpawnBread));
            }

        }

        private void GenerateEnemies()
        {
            //We iterate on the map of enemies to spawn, and decide a valid point in which to spawn them
        }









        //function that generates a point on the immediate outside of the lake. Is used to simulate the starting point from which
        //a piece of bread is thrown
        public (float, float) GeneratePointOutsideLake()
        {
            float widthGrass = _grass.transform.localScale.x;
            float heightGrass = _grass.transform.localScale.y;
            float xCenterGrass = _grass.transform.position.x;
            float yCenterGrass = _grass.transform.position.y;
            float offsetXTerrain = _grass.transform.parent.gameObject.GetComponent<LevelStageNamespace.LakeDescriptionComponent>().OffsetXTerrain;
            float offsetYTerrain = _grass.transform.parent.gameObject.GetComponent<LevelStageNamespace.LakeDescriptionComponent>().OffsetYTerrain;
            int left_right___or___above_below = Random.Range(0, 2);
            float x = Random.Range(0 - offsetXTerrain, widthGrass + offsetXTerrain + 1);
            float y = Random.Range(0 - offsetYTerrain, heightGrass + offsetYTerrain + 1);

            if (left_right___or___above_below == 0)
            {
                y = Random.Range(yCenterGrass - (heightGrass / 2) - offsetYTerrain, yCenterGrass + heightGrass / 2 + offsetYTerrain + 1);
                int coin = Random.Range(0, 2);
                if (coin == 0)
                {
                    x = Random.Range(xCenterGrass - (widthGrass / 2) - offsetXTerrain, xCenterGrass - (widthGrass / 2));
                }
                else
                {
                    x = Random.Range(xCenterGrass + 1 + (widthGrass / 2), xCenterGrass + (widthGrass / 2) + offsetXTerrain);
                }
            }
            else
            {
                x = Random.Range(xCenterGrass - (widthGrass / 2) - offsetXTerrain, xCenterGrass + (widthGrass / 2) + offsetXTerrain + 1);
                int coin = Random.Range(0, 2);
                if (coin == 0)
                {
                    y = Random.Range(yCenterGrass - (heightGrass / 2) - offsetYTerrain, yCenterGrass - (heightGrass / 2));
                }
                else
                {
                    y = Random.Range(yCenterGrass + 1 + (heightGrass / 2), yCenterGrass + (heightGrass / 2) + offsetYTerrain);
                }
            }

            return (x, y);
        }




        //function that returns a point inside the lake
        public (float, float) GeneratePointInsideLake()
        {
            
            float widthLake = _lake.transform.localScale.x;
            float heightLake = _lake.transform.localScale.y;
            float xCenterLake = _lake.transform.position.x;
            float yCenterLake = _lake.transform.position.y;

            bool generatedEnd = false;
            float x = 0f;
            float y = 0f;
            while (!generatedEnd)
            {
                x = Random.Range((xCenterLake - widthLake / 2) + OffsetXLake, (xCenterLake + widthLake / 2) - OffsetXLake);
                y = Random.Range((yCenterLake - heightLake / 2) + OffsetYLake, (yCenterLake + heightLake / 2) - OffsetYLake);

                float one = Mathf.Pow((2 * x) / widthLake, 2);
                float two = Mathf.Pow((2 * y) / heightLake, 2);

                float distanceFromCenter = Mathf.Sqrt(one + two);

                if (distanceFromCenter <= 1)
                {
                    generatedEnd = true;
                }
            }

            return (x, y);

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



