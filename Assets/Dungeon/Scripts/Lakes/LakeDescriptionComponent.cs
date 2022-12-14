using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Player;
using HUDNamespace;
using System.Linq;
using GraphLakeNamespace;

namespace LevelStageNamespace {
    public class LakeDescriptionComponent : LakeShopDescriptionComponent
    {
        //component that specifies the nature of a lake

        
        private GameObject _playerObject;

        private LakeDescriptionSO _lakeDescriptionForThisLake;
        private BreadSpawnSO _breadSpawnForThisLake;

        private GameObject _northRiver;
        private GameObject _southRiver;
        private GameObject _westRiver;
        private GameObject _eastRiver;




        //offsets for points relative in some way to the water or the terrain
        public float OffsetXTerrain;
        public float OffsetYTerrain;
        public float OffsetXLake;
        public float OffsetYLake;

        //offset for the spawn of the player
        public float OffsetXPlayer = 0f;
        public float OffsetYPlayer = 0f;

        //radius of area around the player in which we don't want enemies to spawn
        public float RadiusAroundPlayer;

        //radius of area arount the enemies in which we don't want other enemies to spawn
        public float RadiusAroundEnemy;

        //original y scale of the west and east river
        private float _yScaleOfRiver;
        //original x scale of the north and south river
        private float _xScaleOfRiver;



        //########################################################################################################################################################
        //############################################################# INFORMATIONS ABOUT ENEMIES ###############################################################
        //########################################################################################################################################################

        [SerializeField] public int NumberOfMallard;
        [SerializeField] public int NumberOfCoot;
        [SerializeField] public int NumberOfGoose;
        [SerializeField] public int NumberOfFish;       //we'll think about them in the future
        [SerializeField] public int NumberOfSeagull;    //we'll think about them in the future

        [SerializeField] private GameObject MallardPrefab;
        [SerializeField] private GameObject CootPrefab;
        [SerializeField] private GameObject GoosePrefab;
        [SerializeField] private GameObject FishPrefab;
        [SerializeField] private GameObject SeagullPrefab;

        //########################################################################################################################################################
        //############################################################## INFORMATIONS ABOUT RIVERS ###############################################################
        //########################################################################################################################################################

        [SerializeField] private Transform NorthRiver;
        [SerializeField] private Transform SouthRiver;
        [SerializeField] private Transform WestRiver;
        [SerializeField] private Transform EastRiver;

        [Header("From where does the player come? North, south, east or west?")]
        [SerializeField] public EnumsDungeon.CompassDirection SpawnPlayer;

        //########################################################################################################################################################
        //############################################################## INFORMATIONS ABOUT BREAD ################################################################
        //########################################################################################################################################################

        [SerializeField] private GameObject SmallBreadPrefab;
        [SerializeField] private GameObject MediumBreadPrefab;
        [SerializeField] private GameObject LargeBreadPrefab;

        [SerializeField] private GameObject BreadToThrow;

        //########################################################################################################################################################
        //######################################################## INFORMATIONS ABOUT QUICK TIME EVENT ###########################################################
        //########################################################################################################################################################

        [SerializeField] private GameObject QTEMinigamePrefab;

        public Dictionary<EnumsDungeon.BreadType, int> BreadToSpawnMap;

        [SerializeField] private int _totalNumberOfBreadPiecesToSpawn;       //can be obtained scanning BreadToSpawnMap
        [SerializeField] private int _totalNumberOfBreadPiecesToBeEaten;     //it's _totalNumberOfBreadPiecesToSpawn initially, but can get bigger if a new piece of bread is spawned because of stealing, for example
        [SerializeField] private int _totalNumberOfBreadPiecesEaten;         //counts how many pieces of bread have been eaten in this lake

        private List<float> _arrayBreadSpawnTime;                       //position i: says that the i-th bread will spawn elem[i] time after the time the i-1 th element spawned
        private List<EnumsDungeon.BreadType> _arrayBreadSpawnType;      //position i: says what kind of bread needs to be spawned as i-th bread for this lake
        private float _minIntervalTimeSpawnBread;
        private float _maxIntervaltimeSpawnBread;

        //[SerializeField] public bool LakeCleared;

        private MapManager _mapManager;





        protected override void Awake()
        {
            base.Awake();

            Application.targetFrameRate = 60;

            _lakeDescriptionForThisLake = _levelStageManager.GetLakeDescriptionSO();
            ManageRiversOfthisLake();
            _breadSpawnForThisLake = _levelStageManager.GetBreadSpawnSO();

            //update the map interface
            _mapManager = GameObject.FindObjectOfType<MapManager>();
            MapManager.CardinalDirection direction;
            switch (_lakeDescriptionForThisLake.PlayerSpawnDirection)
            {
                case EnumsDungeon.CompassDirection.North:
                    direction = MapManager.CardinalDirection.sud;
                    break;
                case EnumsDungeon.CompassDirection.South:
                    direction = MapManager.CardinalDirection.nord;
                    break;
                case EnumsDungeon.CompassDirection.West:
                    direction = MapManager.CardinalDirection.est;
                    break;
                case EnumsDungeon.CompassDirection.East:
                    direction = MapManager.CardinalDirection.ovest;
                    break;
                default:
                    direction = MapManager.CardinalDirection.none;
                    Debug.Log("LOL");
                    break;
            }


            LakeDescriptionSO[,] currentStageMap = _levelStageManager.GetCurrentStageMap();
            int xOfCurrentLake = _levelStageManager.GetXOfCurrentLake();
            int yOfCurrentLake = _levelStageManager.GetYOfCurrentLake();

            int nord = GetValueForMinimap(currentStageMap[xOfCurrentLake - 1, yOfCurrentLake]);
            int sud = GetValueForMinimap(currentStageMap[xOfCurrentLake + 1, yOfCurrentLake]);
            int ovest = GetValueForMinimap(currentStageMap[xOfCurrentLake, yOfCurrentLake - 1]);
            int est = GetValueForMinimap(currentStageMap[xOfCurrentLake, yOfCurrentLake + 1]);




            //first of all, a fade in effect
            _levelStageManager.FadeIn();

            //now we have to place the player in the correct spot
            _playerObject = GameObject.FindWithTag("Player");     //TODO: change in actual player when you have it

            //if the lake
            //is the initial one and the player just arrived in the stage, we want it to be on the center of the room
            if (!_lakeDescriptionForThisLake.IsLakeCleared && _lakeDescriptionForThisLake.IsStartingRoom)
            {
                _playerObject.transform.position = new Vector3(0, 0, 0);
                _levelStageManager.SetLakeAsCleared();

                //update minimap
                direction = MapManager.CardinalDirection.none;
                _mapManager.UpdateMinimapAfterRiver(direction, nord, sud, est, ovest);
            }
            else
            {
                //otherwise, we first of all set the correct position of the player
                SpawnPlayer = _lakeDescriptionForThisLake.PlayerSpawnDirection;
                string riversString = "Water/WaterBorder/Rivers";
                switch (SpawnPlayer)
                {
                    case EnumsDungeon.CompassDirection.North:
                        _playerObject.transform.position = transform.Find(riversString + "/RiverNorth/Position").transform.position;// + new Vector3(0, -OffsetYPlayer, 0);
                        break;
                    case EnumsDungeon.CompassDirection.South:
                        _playerObject.transform.position = transform.Find(riversString + "/RiverSouth/Position").transform.position;// + new Vector3(0, OffsetYPlayer, 0);
                        break;
                    case EnumsDungeon.CompassDirection.West:
                        _playerObject.transform.position = transform.Find(riversString + "/RiverWest/Position").transform.position;// + new Vector3(OffsetXPlayer, 0, 0);
                        break;
                    case EnumsDungeon.CompassDirection.East:
                        _playerObject.transform.position = transform.Find(riversString + "/RiverEast/Position").transform.position;// + new Vector3(-OffsetXPlayer, 0, 0);
                        break;
                }

                //update minimap
                _mapManager.UpdateMinimapAfterRiver(direction, nord, sud, est, ovest);

                if (_lakeDescriptionForThisLake.EnemiesToSpawnMap != null)
                {
                    NumberOfMallard = _lakeDescriptionForThisLake.EnemiesToSpawnMap[EnumsDungeon.EnemyType.Mallard];
                    NumberOfCoot = _lakeDescriptionForThisLake.EnemiesToSpawnMap[EnumsDungeon.EnemyType.Coot];
                    NumberOfGoose = _lakeDescriptionForThisLake.EnemiesToSpawnMap[EnumsDungeon.EnemyType.Goose];
                    NumberOfFish = _lakeDescriptionForThisLake.EnemiesToSpawnMap[EnumsDungeon.EnemyType.Fish];
                    NumberOfSeagull = _lakeDescriptionForThisLake.EnemiesToSpawnMap[EnumsDungeon.EnemyType.Seagull];
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

                //disable the exit triggers
                EnableExitTriggers(false);

                //the setup of the lake is done. Now we wait until the player enters in the actual lake from the river. To do so, he will need
                //to pass through the TriggerEnteredCollider of the river in which he is.

            }

            //generate all the obstacles
            _obstacles.GetComponent<ObstaclesLakeComponent>().
                SetObstacles(_lakeDescriptionForThisLake.ObstaclesDescription.Item1, _lakeDescriptionForThisLake.ObstaclesDescription.Item2);

            

        }


        //########################################################################################################################################################
        //########################################################################################################################################################
        //################################################################# RIVERS MANAGEMENT ####################################################################
        //########################################################################################################################################################
        //########################################################################################################################################################


        private void ManageRiversOfthisLake()
        {
            string riversString = "Water/WaterBorder/Rivers";
            _northRiver = transform.Find(riversString + "/RiverNorth").gameObject;
            _southRiver = transform.Find(riversString + "/RiverSouth").gameObject;
            _westRiver = transform.Find(riversString + "/RiverWest").gameObject;
            _eastRiver = transform.Find(riversString + "/RiverEast").gameObject;
            if (_lakeDescriptionForThisLake.HasNorthRiver == false)
            {
                //if the north lake is not present, we obscure it and activate the collider for that part of the lake
                _northRiver.transform.Find("0").gameObject.SetActive(true);
                _northRiver.transform.Find("CloseCollider").gameObject.SetActive(true);
                _northRiver.transform.Find("1").gameObject.SetActive(false);
                _northRiver.transform.Find("2").gameObject.SetActive(false);
                _northRiver.transform.Find("3").gameObject.SetActive(false);
                _northRiver.transform.Find("TriggerEnteredCollider").gameObject.SetActive(false);
                _northRiver.transform.Find("TriggerExitedCollider").gameObject.SetActive(false);
            }
            else
            {
                //otherwise, we leave the sprite open and the colliders for entering and exiting working (we do the same for the other rivers)
                _northRiver.transform.Find("0").gameObject.SetActive(false);
                _northRiver.transform.Find("CloseCollider").gameObject.SetActive(false);
                _northRiver.transform.Find("1").gameObject.SetActive(false);
                _northRiver.transform.Find("2").gameObject.SetActive(false);
                _northRiver.transform.Find("3").gameObject.SetActive(true);
                _northRiver.transform.Find("TriggerEnteredCollider").gameObject.SetActive(true);
                _northRiver.transform.Find("TriggerExitedCollider").gameObject.SetActive(true);
                if (_lakeDescriptionForThisLake.IsFinalRoom && _lakeDescriptionForThisLake.ExitStageDirection == EnumsDungeon.CompassDirection.North) SetRiverAsFinal(_northRiver);

            }
            if (_lakeDescriptionForThisLake.HasSouthRiver == false)
            {
                _southRiver.transform.Find("0").gameObject.SetActive(true);
                _southRiver.transform.Find("CloseCollider").gameObject.SetActive(true);
                _southRiver.transform.Find("1").gameObject.SetActive(false);
                _southRiver.transform.Find("2").gameObject.SetActive(false);
                _southRiver.transform.Find("3").gameObject.SetActive(false);
                _southRiver.transform.Find("TriggerEnteredCollider").gameObject.SetActive(false);
                _southRiver.transform.Find("TriggerExitedCollider").gameObject.SetActive(false);
            }
            else
            {
                _southRiver.transform.Find("0").gameObject.SetActive(false);
                _southRiver.transform.Find("CloseCollider").gameObject.SetActive(false);
                _southRiver.transform.Find("1").gameObject.SetActive(false);
                _southRiver.transform.Find("2").gameObject.SetActive(false);
                _southRiver.transform.Find("3").gameObject.SetActive(true);
                _southRiver.transform.Find("TriggerEnteredCollider").gameObject.SetActive(true);
                _southRiver.transform.Find("TriggerExitedCollider").gameObject.SetActive(true);
                if (_lakeDescriptionForThisLake.IsFinalRoom && _lakeDescriptionForThisLake.ExitStageDirection == EnumsDungeon.CompassDirection.South) SetRiverAsFinal(_southRiver);
            }
            if (_lakeDescriptionForThisLake.HasWestRiver == false)
            {
                _westRiver.transform.Find("0").gameObject.SetActive(true);
                _westRiver.transform.Find("CloseCollider").gameObject.SetActive(true);
                _westRiver.transform.Find("1").gameObject.SetActive(false);
                _westRiver.transform.Find("2").gameObject.SetActive(false);
                _westRiver.transform.Find("3").gameObject.SetActive(false);
                _westRiver.transform.Find("TriggerEnteredCollider").gameObject.SetActive(false);
                _westRiver.transform.Find("TriggerExitedCollider").gameObject.SetActive(false);
            }
            else
            {
                _westRiver.transform.Find("0").gameObject.SetActive(false);
                _westRiver.transform.Find("CloseCollider").gameObject.SetActive(false);
                _westRiver.transform.Find("1").gameObject.SetActive(false);
                _westRiver.transform.Find("2").gameObject.SetActive(false);
                _westRiver.transform.Find("3").gameObject.SetActive(true);
                _westRiver.transform.Find("TriggerEnteredCollider").gameObject.SetActive(true);
                _westRiver.transform.Find("TriggerExitedCollider").gameObject.SetActive(true);
                if (_lakeDescriptionForThisLake.IsFinalRoom && _lakeDescriptionForThisLake.ExitStageDirection == EnumsDungeon.CompassDirection.West) SetRiverAsFinal(_westRiver);
            }
            if (_lakeDescriptionForThisLake.HasEastRiver == false)
            {
                _eastRiver.transform.Find("0").gameObject.SetActive(true);
                _eastRiver.transform.Find("CloseCollider").gameObject.SetActive(true);
                _eastRiver.transform.Find("1").gameObject.SetActive(false);
                _eastRiver.transform.Find("2").gameObject.SetActive(false);
                _eastRiver.transform.Find("3").gameObject.SetActive(false);
                _eastRiver.transform.Find("TriggerEnteredCollider").gameObject.SetActive(false);
                _eastRiver.transform.Find("TriggerExitedCollider").gameObject.SetActive(false);
            }
            else
            {
                _eastRiver.transform.Find("0").gameObject.SetActive(false);
                _eastRiver.transform.Find("CloseCollider").gameObject.SetActive(false);
                _eastRiver.transform.Find("1").gameObject.SetActive(false);
                _eastRiver.transform.Find("2").gameObject.SetActive(false);
                _eastRiver.transform.Find("3").gameObject.SetActive(true);
                _eastRiver.transform.Find("TriggerEnteredCollider").gameObject.SetActive(true);
                _eastRiver.transform.Find("TriggerExitedCollider").gameObject.SetActive(true);
                if (_lakeDescriptionForThisLake.IsFinalRoom && _lakeDescriptionForThisLake.ExitStageDirection == EnumsDungeon.CompassDirection.East) SetRiverAsFinal(_eastRiver);
            }
        }

        private void EnableExitTriggers(bool b)
        {
            _northRiver.transform.Find("TriggerExitedCollider").gameObject.SetActive(b);
            _southRiver.transform.Find("TriggerExitedCollider").gameObject.SetActive(b);
            _westRiver.transform.Find("TriggerExitedCollider").gameObject.SetActive(b);
            _eastRiver.transform.Find("TriggerExitedCollider").gameObject.SetActive(b);
        }

        //########################################################## CLOSE RIVER MANAGEMENT ##########################################################

        //called by EnterPlayerInLakeComponent to signal that the player entered the lake, and the rivers must be closed
        public void CloseLakesWithAnimation()
        {
            if(_lakeDescriptionForThisLake.HasNorthRiver == true)
            {
                _northRiver.transform.Find("CloseCollider").gameObject.SetActive(true);
                StartCoroutine(CloseLakeCoroutine(_northRiver, EnumsDungeon.CompassDirection.North));
            }
            if (_lakeDescriptionForThisLake.HasSouthRiver == true)
            {
                _southRiver.transform.Find("CloseCollider").gameObject.SetActive(true);
                StartCoroutine(CloseLakeCoroutine(_southRiver, EnumsDungeon.CompassDirection.South));
            }
            if (_lakeDescriptionForThisLake.HasWestRiver == true)
            {
                _westRiver.transform.Find("CloseCollider").gameObject.SetActive(true);
                StartCoroutine(CloseLakeCoroutine(_westRiver, EnumsDungeon.CompassDirection.West));
            }
            if (_lakeDescriptionForThisLake.HasEastRiver == true)
            {
                _eastRiver.transform.Find("CloseCollider").gameObject.SetActive(true);
                StartCoroutine(CloseLakeCoroutine(_eastRiver, EnumsDungeon.CompassDirection.East));
            }
        }

        private IEnumerator CloseLakeCoroutine(GameObject river, EnumsDungeon.CompassDirection direction)
        {
            //TODO: for this moment, I'll just stick the values here, because otherwise I would have to manage too many variables in this monobehaviour,
            //and I also don't know if I want exactly this kind of animation.

            float closingTime = 1f;
            float numberOfStatesF = 3f;  //we currently have only 3 states for the rivers
            int numberOfStatesI = 3;

            switch (direction)
            {
                case EnumsDungeon.CompassDirection.North:
                    for(int i = numberOfStatesI; i > 0; i--)
                    {
                        _northRiver.transform.Find(i.ToString()).gameObject.SetActive(false);
                        _northRiver.transform.Find((i-1).ToString()).gameObject.SetActive(true);
                        yield return new WaitForSeconds(closingTime / numberOfStatesF);
                    }
                    break;
                case EnumsDungeon.CompassDirection.South:
                    for (int i = numberOfStatesI; i > 0; i--)
                    {
                        _southRiver.transform.Find(i.ToString()).gameObject.SetActive(false);
                        _southRiver.transform.Find((i - 1).ToString()).gameObject.SetActive(true);
                        yield return new WaitForSeconds(closingTime / numberOfStatesF);
                    }
                    break;
                case EnumsDungeon.CompassDirection.West:
                    for (int i = numberOfStatesI; i > 0; i--)
                    {
                        _westRiver.transform.Find(i.ToString()).gameObject.SetActive(false);
                        _westRiver.transform.Find((i - 1).ToString()).gameObject.SetActive(true);
                        yield return new WaitForSeconds(closingTime / numberOfStatesF);
                    }
                    break;
                case EnumsDungeon.CompassDirection.East:
                    for (int i = numberOfStatesI; i > 0; i--)
                    {
                        _eastRiver.transform.Find(i.ToString()).gameObject.SetActive(false);
                        _eastRiver.transform.Find((i - 1).ToString()).gameObject.SetActive(true);
                        yield return new WaitForSeconds(closingTime / numberOfStatesF);
                    }
                    break;
            }
            yield return null;
        }

        //########################################################## OPEN RIVER MANAGEMENT ##########################################################

        public void OpenLakesWithAnimation()
        {
            if (_lakeDescriptionForThisLake.HasNorthRiver == true)
            {
                _northRiver.transform.Find("CloseCollider").gameObject.SetActive(false);
                StartCoroutine(OpenLakeCoroutine(_northRiver, EnumsDungeon.CompassDirection.North));
            }
            if (_lakeDescriptionForThisLake.HasSouthRiver == true)
            {
                _southRiver.transform.Find("CloseCollider").gameObject.SetActive(false);
                StartCoroutine(OpenLakeCoroutine(_southRiver, EnumsDungeon.CompassDirection.South));
            }
            if (_lakeDescriptionForThisLake.HasWestRiver == true)
            {
                _westRiver.transform.Find("CloseCollider").gameObject.SetActive(false);
                StartCoroutine(OpenLakeCoroutine(_westRiver, EnumsDungeon.CompassDirection.West));
            }
            if (_lakeDescriptionForThisLake.HasEastRiver == true)
            {
                _eastRiver.transform.Find("CloseCollider").gameObject.SetActive(false);
                StartCoroutine(OpenLakeCoroutine(_eastRiver, EnumsDungeon.CompassDirection.East));
            }

            EnableExitTriggers(true);
        }

        private IEnumerator OpenLakeCoroutine(GameObject river, EnumsDungeon.CompassDirection direction)
        {
            //TODO: for this moment, I'll just stick the values here, because otherwise I would have to manage too many variables in this monobehaviour,
            //and I also don't know if I want exactly this kind of animation.

            float closingTime = 1f;
            float numberOfStatesF = 3f;  //we currently have only 3 states for the rivers
            int numberOfStatesI = 3;

            switch (direction)
            {
                case EnumsDungeon.CompassDirection.North:
                    for (int i = 0; i < numberOfStatesI; i++)
                    {
                        _northRiver.transform.Find(i.ToString()).gameObject.SetActive(false);
                        _northRiver.transform.Find((i + 1).ToString()).gameObject.SetActive(true);
                        yield return new WaitForSeconds(closingTime / numberOfStatesF);
                    }
                    break;
                case EnumsDungeon.CompassDirection.South:
                    for (int i = 0; i < numberOfStatesI; i++)
                    {
                        _southRiver.transform.Find(i.ToString()).gameObject.SetActive(false);
                        _southRiver.transform.Find((i + 1).ToString()).gameObject.SetActive(true);
                        yield return new WaitForSeconds(closingTime / numberOfStatesF);
                    }
                    break;
                case EnumsDungeon.CompassDirection.West:
                    for (int i = 0; i < numberOfStatesI; i++)
                    {
                        _westRiver.transform.Find(i.ToString()).gameObject.SetActive(false);
                        _westRiver.transform.Find((i + 1).ToString()).gameObject.SetActive(true);
                        yield return new WaitForSeconds(closingTime / numberOfStatesF);
                    }
                    break;
                case EnumsDungeon.CompassDirection.East:
                    for (int i = 0; i < numberOfStatesI; i++)
                    {
                        _eastRiver.transform.Find(i.ToString()).gameObject.SetActive(false);
                        _eastRiver.transform.Find((i + 1).ToString()).gameObject.SetActive(true);
                        yield return new WaitForSeconds(closingTime / numberOfStatesF);
                    }
                    break;
            }
            yield return null;
        }




        //I NEED to solve this bug, and I will do whatever it takes
        private IEnumerator BruteForceOpenRivers()
        {
            int breadID = 0;
            GameObject chosenBreadToInvestigate = null;
            int breadPoints = 0;

            while(_levelStageManager.IsCurrentLakeCleared() == false)
            {
                //Debug.Log("Emergence: START WAITING 3 SECONDS");
                yield return new WaitForSeconds(2.5f);
                PlayerState playerState = _playerObject.GetComponent<PlayerController>().GetState();

                if (GameObject.FindGameObjectsWithTag("FoodThrown").Length > 0 ||
                    GameObject.FindGameObjectsWithTag("FoodInWater").Length > 0 ||
                    playerState == PlayerState.Carrying ||
                    playerState == PlayerState.Stealing ||
                    playerState == PlayerState.GettingRobbed)
                {
                    yield return null;
                }
                else
                {
                    //we only have breadInMouth. If there is any, let's take one of them.
                    GameObject[] breadsInMouth = GameObject.FindGameObjectsWithTag("FoodInMouth");
                    if (breadsInMouth.Length > 0)
                    {

                        if (breadID == 0)
                        {
                            chosenBreadToInvestigate = breadsInMouth[0];
                            breadID = chosenBreadToInvestigate.GetInstanceID();
                            breadPoints = chosenBreadToInvestigate.GetComponent<BreadNamespace.BreadInMouthComponent>().GetBreadPoints();
                        }
                        else
                        {
                            bool found = false;
                            for (int i = 0; i < breadsInMouth.Length; i++)
                            {
                                if (breadsInMouth[i].GetInstanceID() == breadID)
                                {
                                    found = true;
                                    if (breadsInMouth[i].GetComponent<BreadNamespace.BreadInMouthComponent>().GetBreadPoints() == breadPoints)
                                    {
                                        //there is a piece of bread (in mouth) that in three seconds wasn't eaten by a bit. Right now, this cannot happen.
                                        //So, open the rivers
                                        CompleteLake();
                                        //Debug.Log("Emergence: EMERGENCE PROCEDURE ACTIVATED 1");
                                    }
                                }
                            }
                            if (!found && breadsInMouth.Length > 0)
                            {
                                chosenBreadToInvestigate = breadsInMouth[0];
                                breadID = chosenBreadToInvestigate.GetInstanceID();
                                breadPoints = chosenBreadToInvestigate.GetComponent<BreadNamespace.BreadInMouthComponent>().GetBreadPoints();
                            }
                            else
                            {
                                breadID = 0;
                            }

                        }
                    }
                    else
                    {
                        //there is NO BREAD at all, of any kind: we can consider the lake finished, but let's do a double check
                        yield return new WaitForSeconds(0.5f);
                        breadsInMouth = GameObject.FindGameObjectsWithTag("FoodInMouth");
                        if (breadsInMouth.Length == 0)
                        {
                            CompleteLake();
                            //Debug.Log("Emergence: EMERGENCE PROCEDURE ACTIVATED 2");
                        }
                    }
                }
            }
            //Debug.Log("Emergence: EXIT");
            yield return null;
        }



        //###################################################### RIVER TO NEXT STAGE MANAGEMENT ######################################################

        private void SetRiverAsFinal(GameObject river)
        {
            river.transform.Find("Props").gameObject.SetActive(true);
        }



        //########################################################################################################################################################
        //########################################################################################################################################################
        //########################################################### BREAD TYPE AND TIME GENERATION #############################################################
        //########################################################################################################################################################
        //########################################################################################################################################################

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

            _totalNumberOfBreadPiecesToBeEaten = _totalNumberOfBreadPiecesToSpawn;
            _totalNumberOfBreadPiecesEaten = 0;

        }


        public void StartThrowingAllTheBread()
        {
            StartCoroutine(ThrowBreads());
        }

        private IEnumerator ThrowBreads()
        {

            //float xPixelSprite = 100f;
            //float yPixelSprite = 100f;

            for(int i = 0; i < _totalNumberOfBreadPiecesToSpawn; i++)
            {
                GameObject newBread = Instantiate(BreadToThrow);
                newBread.GetComponent<BreadNamespace.BreadThrownComponent>().InitializeBreadThrownFromPeople(_arrayBreadSpawnType[i], 60f, 80f, 85f);
                yield return new WaitForSeconds(_arrayBreadSpawnTime[i]);
            }

            StartCoroutine(BruteForceOpenRivers());

            yield return null;
        }


        //function used by the bread pieces to notify that they have been eaten completely
        public void NotifyBreadEaten()
        {
            _totalNumberOfBreadPiecesEaten += 1;
            if(_totalNumberOfBreadPiecesEaten == _totalNumberOfBreadPiecesToBeEaten)
            {
                //CompleteLake();   //TODO: non credo tu lo debba rimuovere
            }
        }

        //function to call whenever the lake has been completed (all bread has been eaten)
        private void CompleteLake()
        {
            //CALL A FUNCTION THAT ENDS THE LAKE
            //Debug.Log("ALL BREAD EATEN; OPEN THE GATES!");
            OpenLakesWithAnimation();
            _levelStageManager.SetLakeAsCleared();

            //MUSIC: since all bread has been eaten, this lake goes back to "normal" music
            Music.UniversalAudio.PlayMusic("Swimming", false);
            //SFX: the lake has been cleared
            Music.UniversalAudio.PlaySound("LakeClear", transform);
        }

        //function used to notify that a new piece of bread has been, somehow, generated (somehow = e.g. spawned because of a stealing action)
        public void NotifyBreadCreated()
        {
            _totalNumberOfBreadPiecesToBeEaten += 1;
        }



        //########################################################################################################################################################
        //########################################################################################################################################################
        //################################################################# ENEMIES GENERATION ###################################################################
        //########################################################################################################################################################
        //########################################################################################################################################################

        private void GenerateEnemies()
        {
            //We iterate on the map of enemies to spawn, and decide a valid point in which to spawn them
            //A valid point is a point that:
            //-is inside the water
            //-is not too close to the player
            //-is not too close to other enemies

            List<Vector2> pointsOfEnemies = new List<Vector2>();

            for (int i = 0; i < NumberOfMallard; i++)
            {
                Vector2 newPoint = GenerateMallard(pointsOfEnemies);
                pointsOfEnemies.Add(newPoint);
            }
            for (int i = 0; i < NumberOfCoot; i++)
            {
                Vector2 newPoint = GenerateCoot(pointsOfEnemies);
                pointsOfEnemies.Add(newPoint);
            }
            for (int i = 0; i < NumberOfGoose; i++)
            {
                Vector2 newPoint = GenerateGoose(pointsOfEnemies);
                pointsOfEnemies.Add(newPoint);
            }
            for (int i = 0; i < NumberOfFish; i++)
            {
                Vector2 newPoint = GenerateFish(pointsOfEnemies);
                pointsOfEnemies.Add(newPoint);
            }
            for (int i = 0; i < NumberOfSeagull; i++)
            {
                Vector2 newPoint = GenerateSeagull(pointsOfEnemies);
                pointsOfEnemies.Add(newPoint);
            }

        }


        //function used to get a point in which to spawn an enemy.
        private Vector2 GenerateEnemyPoint(List<Vector2> otherEnemiesPoints)
        {
            Vector2 currentPoint = Vector2.zero;
            bool ok = false;
            float x, y;
            while (!ok)
            {
                (x, y) = GeneratePointInsideLakeFarFromPlayer();
                currentPoint = new Vector2(x, y);
                ok = true;
                for (int j = 0; j < otherEnemiesPoints.Count; j++)
                {
                    var enemyPoint = otherEnemiesPoints[j];
                    if (Vector2.Distance(currentPoint, enemyPoint) < RadiusAroundEnemy)
                    {
                        ok = false;
                    }
                }
            }
            return currentPoint;
        }

        //function that creates a mallard in the lake and returns the point in which the mallard spawned. The argument represent a list of point in which lie
        //other ducks
        private Vector2 GenerateMallard(List<Vector2> otherEnemiesPoints)
        {
            Vector2 point = GenerateEnemyPoint(otherEnemiesPoints);
            Instantiate(MallardPrefab, point, Quaternion.identity);
            return point;
        }

        private Vector2 GenerateCoot(List<Vector2> otherEnemiesPoints)
        {
            Vector2 point = GenerateEnemyPoint(otherEnemiesPoints);
            Instantiate(CootPrefab, point, Quaternion.identity);
            return point;
        }

        private Vector2 GenerateGoose(List<Vector2> otherEnemiesPoints)
        {
            Vector2 point = GenerateEnemyPoint(otherEnemiesPoints);
            Instantiate(GoosePrefab, point, Quaternion.identity);
            return point;
        }

        private Vector2 GenerateFish(List<Vector2> otherEnemiesPoints)
        {
            Vector2 point = GenerateEnemyPoint(otherEnemiesPoints);
            Instantiate(FishPrefab, point, Quaternion.identity);
            return point;
        }

        private Vector2 GenerateSeagull(List<Vector2> otherEnemiesPoints)
        {
            Vector2 point = GenerateEnemyPoint(otherEnemiesPoints);
            Instantiate(SeagullPrefab, point, Quaternion.identity);
            return point;
        }







        //########################################################################################################################################################
        //########################################################################################################################################################
        //################################################### POINTS GENERATION AND GENERAL POINT MANAGEMENT #####################################################
        //########################################################################################################################################################
        //########################################################################################################################################################


        //function that generates a point on the immediate outside of the lake. Is used to simulate the starting point from which
        //a piece of bread is thrown
        public (float, float) GeneratePointOutsideLake()
        {

            Bounds terrainBounds = GetTerrainBounds();

            float widthTerrain = terrainBounds.extents.x * 2;
            float heightTerrain = terrainBounds.extents.y * 2;
            float xCenterTerrain = terrainBounds.center.x;
            float yCenterTerrain = terrainBounds.center.y;
            int left_right___or___above_below = Random.Range(0, 2);
            float x = Random.Range(0 - OffsetXTerrain, widthTerrain + OffsetXTerrain + 1);
            float y = Random.Range(0 - OffsetYTerrain, heightTerrain + OffsetYTerrain + 1);

            if (left_right___or___above_below == 0)
            {
                y = Random.Range(yCenterTerrain - (heightTerrain / 2) - OffsetYTerrain, yCenterTerrain + heightTerrain / 2 + OffsetYTerrain + 1);
                int coin = Random.Range(0, 2);
                if (coin == 0)
                {
                    x = Random.Range(xCenterTerrain - (widthTerrain / 2) - OffsetXTerrain, xCenterTerrain - (widthTerrain / 2));
                }
                else
                {
                    x = Random.Range(xCenterTerrain + 1 + (widthTerrain / 2), xCenterTerrain + (widthTerrain / 2) + OffsetXTerrain);
                }
            }
            else
            {
                x = Random.Range(xCenterTerrain - (widthTerrain / 2) - OffsetXTerrain, xCenterTerrain + (widthTerrain / 2) + OffsetXTerrain + 1);
                int coin = Random.Range(0, 2);
                if (coin == 0)
                {
                    y = Random.Range(yCenterTerrain - (heightTerrain / 2) - OffsetYTerrain, yCenterTerrain - (heightTerrain / 2));
                }
                else
                {
                    y = Random.Range(yCenterTerrain + 1 + (heightTerrain / 2), yCenterTerrain + (heightTerrain / 2) + OffsetYTerrain);
                }
            }

            return (x, y);
        }




        //function that returns a point inside the lake
        public (float, float) GeneratePointInsideLake()
        {

            Bounds waterCenterBounds = _water.transform.Find("WaterCenter").GetComponent<TilemapRenderer>().bounds;

            float x = Random.Range(waterCenterBounds.min.x, waterCenterBounds.max.x);
            float y = Random.Range(waterCenterBounds.min.y, waterCenterBounds.max.y);

            while(!Contains(new Vector3(x, y, 0)))
            {
                x = Random.Range(waterCenterBounds.min.x, waterCenterBounds.max.x);
                y = Random.Range(waterCenterBounds.min.y, waterCenterBounds.max.y);
                //Debug.Log("WHILE IN LAKEDESCRIPTIONCOMPONENT, RIGA CIRCA 724");
            }


            /*
            float widthLake = _lake.transform.Find("WaterCenter").localScale.x;
            float heightLake = _lake.transform.Find("WaterCenter").localScale.y;
            float xCenterLake = _lake.transform.Find("WaterCenter").position.x;
            float yCenterLake = _lake.transform.Find("WaterCenter").position.y;

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
            }*/

            return (x, y);

        }



        //function used to spawn an enemy somewhere in the lake, but not too close to the player
        private (float, float) GeneratePointInsideLakeFarFromPlayer()
        {
            bool ok = false;
            float x = 0f, y = 0f;
            var playerPoint = _playerObject.transform.position;
            while (!ok)
            {
                (x, y) = GeneratePointInsideLake();
                var currentPoint = new Vector2(x, y);
                if (Vector2.Distance(currentPoint, playerPoint) >= RadiusAroundPlayer)
                {
                    ok = true;
                }
            }
            return (x, y);
        }


        //can be considered as a less strict version of the Contains() method used in the LakeShopDescriptionComponent.
        //HANDLE WITH CARE: this function exists because, at this very moment, it's impossible to have the bread fall exactly in the point that we decided
        //it should have fallen in the lake. In fact, it falls a bit offsetted (is it even a word?) wrt the originally planned point. SO, if somehow the minimum
        //distance that there must be between a duck and a breadInWater changes (even more if the change is a shrinking of this distance), it is not guaranteed
        //anymore that this function gives a value that makes the game work.
        public bool IsBreadInWaterInLake(Vector3 point, float breadInWaterRadius)
        {
            //simple case
            if (base.Contains(point)) return true;

            //if the Contains returned false, it means that the piece of bread is on some kind of terrain.
            //Let's give to this bread another chance: maybe it's just a bit outside of the (properly said) lake, but it's
            //still reachable from other ducks.
            //We do this check by considering the adjacent positions wrt this point (above, below, to the left, to the right, and also the diagonal ones) 

            float range = (2f*breadInWaterRadius) / 3f;

            /*Debug.DrawRay(point, new Vector3(0, 1, 0) * range, Color.red, 10f, false);
            Debug.DrawRay(point, new Vector3(0, -1, 0) * range, Color.red, 10f, false);
            Debug.DrawRay(point, new Vector3(1, 0, 0) * range, Color.red, 10f, false);
            Debug.DrawRay(point, new Vector3(-1, 0, 0) * range, Color.red, 10f, false);
            Debug.DrawRay(point, new Vector3(1, 1, 0) * range, Color.red, 10f, false);
            Debug.DrawRay(point, new Vector3(1, -1, 0) * range, Color.red, 10f, false);
            Debug.DrawRay(point, new Vector3(-1, 1, 0) * range, Color.red, 10f, false);
            Debug.DrawRay(point, new Vector3(-1, -1, 0) * range, Color.red, 10f, false);
            */

            Vector2 point2d = new Vector2(point.x, point.y);
            
            if (Contains(point + new Vector3(range, 0, 0)) || Contains(point + new Vector3(-range, 0, 0)) ||
                Contains(point + new Vector3(0, -range, 0)) || Contains(point + new Vector3(0, -range, 0)) ||
                Contains(point + new Vector3(range, range, 0)) || Contains(point + new Vector3(-range, range, 0)) ||
                Contains(point + new Vector3(range, -range, 0)) || Contains(point + new Vector3(-range, -range, 0)))
                    {
                        return true;
                    }
            return false;

            /*

            Ray rayNorth = new Ray(point, new Vector3(0, 1, 0));
            Ray raySouth = new Ray(point, new Vector3(0, -1, 0));
            Ray rayWest = new Ray(point, new Vector3(1, 0, 0));
            Ray rayEast = new Ray(point, new Vector3(-1, 0, 0));

            Ray rayNorthEast = new Ray(point, new Vector3(1, 1, 0));
            Ray raySouthEast = new Ray(point, new Vector3(1, -1, 0));
            Ray rayNorthWest = new Ray(point, new Vector3(-1, 1, 0));
            Ray raySouthWest = new Ray(point, new Vector3(-1, -1, 0));

            Debug.DrawRay(point, new Vector3(0, 1, 0) * range, Color.red, 10f, false);
            Debug.DrawRay(point, new Vector3(0, -1, 0) * range, Color.red, 10f, false);
            Debug.DrawRay(point, new Vector3(1, 0, 0) * range, Color.red, 10f, false);
            Debug.DrawRay(point, new Vector3(-1, 0, 0) * range, Color.red, 10f, false);
            Debug.DrawRay(point, new Vector3(1, 1, 0) * range, Color.red, 10f, false);
            Debug.DrawRay(point, new Vector3(1, -1, 0) * range, Color.red, 10f, false);
            Debug.DrawRay(point, new Vector3(-1, 1, 0) * range, Color.red, 10f, false);
            Debug.DrawRay(point, new Vector3(-1, -1, 0) * range, Color.red, 10f, false);

            RaycastHit2D hit1 = Physics2D.Raycast(point2d, new Vector2(0, 1), range, LayerMask.GetMask("TerrainLayer"));
            RaycastHit2D hit2 = Physics2D.Raycast(point2d, new Vector2(0, -1), range, LayerMask.GetMask("TerrainLayer"));
            RaycastHit2D hit3 = Physics2D.Raycast(point2d, new Vector2(1, 0), range, LayerMask.GetMask("TerrainLayer"));
            RaycastHit2D hit4 = Physics2D.Raycast(point2d, new Vector2(-1, 0), range, LayerMask.GetMask("TerrainLayer"));
            RaycastHit2D hit5 = Physics2D.Raycast(point2d, new Vector2(1, 1), range, LayerMask.GetMask("TerrainLayer"));
            RaycastHit2D hit6 = Physics2D.Raycast(point2d, new Vector2(1, -1), range, LayerMask.GetMask("TerrainLayer"));
            RaycastHit2D hit7 = Physics2D.Raycast(point2d, new Vector2(-1, 1), range, LayerMask.GetMask("TerrainLayer"));
            RaycastHit2D hit8 = Physics2D.Raycast(point2d, new Vector2(-1, -1), range, LayerMask.GetMask("TerrainLayer"));


            if (hit1.collider || hit2.collider || hit3.collider || hit4.collider ||
                hit5.collider || hit6.collider || hit7.collider || hit8.collider)
            {
                Debug.Log("INSIDE");
                return true;
            }
            else
            {
                Debug.Log("OUTSIDE");
                return false;
            }
            */
        }



        //########################################################################################################################################################
        //########################################################################################################################################################
        //################################################################### PLAYER STEALING ####################################################################
        //########################################################################################################################################################
        //########################################################################################################################################################

        private GameObject _disputedBread = null;

        private GameObject _playerReference;

        //function that the player can call when it tries to steal bread from another duck. It requires as argument the piece of bread that the player
        //is trying to steal, and at its end will call a function of the player to notify him of the result of the stealing.
        public void PlayerStartStealFromEnemy(GameObject playerReference, GameObject breadToSteal, float x, float y)
        {
            _playerReference = playerReference;
            _disputedBread = breadToSteal;
            GameObject qteManager = Instantiate(QTEMinigamePrefab);
            qteManager.GetComponent<QTEStealNamespace.QTEManagerComponment>().Initialize(x, y,
                _levelStageManager.GetCurrentLevelIndex()*2, 
                _levelStageManager.GetCurrentLevelIndex()*3 + _levelStageManager.GetCurrentStageIndex(),
                0, 
                (3 + _levelStageManager.GetCurrentLevelIndex()*3 + _levelStageManager.GetCurrentStageIndex()));

            
        }

        //function called from the QTEManagerComponent (after its gameobject has been created by PlayerStartStealFromEnemy) to collect the result
        //of the stealing action initiated by the player and notify him of the result
        public void PlayerEndStealFromEnemy(int correct, int total) {
            int disputedBreadBP = _disputedBread.GetComponent<BreadNamespace.BreadInMouthComponent>().GetBreadPoints();
            bool disputedBreadIsLastPiece = _disputedBread.GetComponent<BreadNamespace.BreadInMouthComponent>().GetIsLastPiece();

            float fraction = (float)(correct) / (float)(total);

            int bpForPlayer = (int) Mathf.Floor((float) disputedBreadBP * fraction);

            int bpForEnemy = (int)Mathf.Ceil((float)disputedBreadBP * (1f - fraction));

            int playerMouthSize = _playerReference.GetComponent<Player.PlayerEatSkill>().GetMouthSize();

            if(bpForPlayer > playerMouthSize)
            {
                //if the player is so good that is about to steal to the other duck more bread than the one it can take,
                //let's make it steal all the bread that can stay in its mouth
                bpForPlayer = playerMouthSize;
                bpForEnemy = disputedBreadBP - bpForPlayer;
            }

            GameObject breadInMouthForPlayer = null;
            GameObject breadInMouthForEnemy = null;

            if(bpForPlayer > 0 && bpForEnemy > 0)
            {
                //both the player and the enemy get a breadInMouth
                breadInMouthForPlayer = Instantiate(_disputedBread);
                breadInMouthForEnemy = Instantiate(_disputedBread);

                if (disputedBreadIsLastPiece)
                {
                    NotifyBreadCreated();   //when both the player and the enemy created a last piece of bread from the piece that was originally of the enemy,
                                            //it's like a new piece of bread gets created, that must be notified to the LakeDescriptionComponent.
                }

                breadInMouthForPlayer.GetComponent<BreadNamespace.BreadInMouthComponent>().Initialize(bpForPlayer, disputedBreadIsLastPiece);
                breadInMouthForEnemy.GetComponent<BreadNamespace.BreadInMouthComponent>().Initialize(bpForEnemy, disputedBreadIsLastPiece);

            }
            else if(bpForPlayer > 0)
            {
                //only the player has a breadInMouth
                breadInMouthForPlayer = Instantiate(_disputedBread);
                breadInMouthForPlayer.GetComponent<BreadNamespace.BreadInMouthComponent>().Initialize(bpForPlayer, disputedBreadIsLastPiece);
            }
            else if(bpForEnemy > 0)
            {
                //only the enemy has a breadInMouth
                breadInMouthForEnemy = Instantiate(_disputedBread);
                breadInMouthForEnemy.GetComponent<BreadNamespace.BreadInMouthComponent>().Initialize(bpForEnemy, disputedBreadIsLastPiece);
            }


            Destroy(_disputedBread);
            _disputedBread = null;


            //call here a function on the player passing to him:
            //-as first argument, the piece of bread that the player stole
            //-as second, the piece of bread still in the enemy's mouth
            _playerReference.GetComponent<Player.PlayerStealSkill>().NotifyFinishedQTE(
                breadInMouthForPlayer, 
                breadInMouthForEnemy
            );

            //Debug.Log("BP FOR ENEMY = " + bpForEnemy + ", BP FOR PLAYER = " + bpForPlayer);


        }

        //########################################################################################################################################################
        //########################################################################################################################################################
        //####################################################################### MINIMAP ########################################################################
        //########################################################################################################################################################
        //########################################################################################################################################################



        //Minimap management
        private int GetValueForMinimap(LakeDescriptionSO lake)
        {
            int ret;
            if (lake == null)
            {
                ret = 0;
            }
            else
            {
                if (lake.IsLakeCleared == false)
                {
                    ret = 1;
                }
                else
                {
                    ret = 2;
                }

                if (lake.IsFinalRoom)
                {
                    ret = 3;
                }
            }
            return ret;
        }


        //########################################################################################################################################################
        //########################################################################################################################################################
        //############################################################# CORRECTION OF DUCK PLACEMENT #############################################################
        //########################################################################################################################################################
        //########################################################################################################################################################

        //the upper left, lower etc.. are referred to where the grass is in that tileset
        private const string tilesetTerrainUpperLeft = "tileset-grassland-water_0";
        private const string tilesetTerrainUpper = "tileset-grassland-water_1";
        private const string tilesetTerrainUpperRight = "tileset-grassland-water_2";
        private const string tilesetIslandLowerRight = "tileset-grassland-water_3";
        private const string tilesetIslandLowerLeft = "tileset-grassland-water_4";
        private const string tilesetTerrainLeft = "tileset-grassland-water_6";
        private const string tilesetTerrainRight = "tileset-grassland-water_8";
        private const string tilesetIslandUpperRight = "tileset-grassland-water_9";
        private const string tilesetIslandUpperLeft = "tileset-grassland-water_10";
        private const string tilesetTerrainLowerLeft = "tileset-grassland-water_12";
        private const string tilesetTerrainLower = "tileset-grassland-water_13";
        private const string tilesetTerrainLowerRight = "tileset-grassland-water_14";

        private float xLenTile = 1.5f;
        private float yLenTile = 1.5f;

        //function that, given the current position of an objects, positions it inside the lake, in the closest possible
        //water tile
        //if the first return value is "false", the currentPos is already in water.
        //if the first return value is "true, the new position in which the object must be positioned is given in the second return value.
        public (bool, Vector3) AdjustPlacement(Vector3 currentPos)
        {
            //simple case
            if (base.Contains(currentPos)) return (false, Vector3.zero);

            //we must now return the closes point inside the lake
            Tilemap terrainTilemap = _terrain.GetComponent<Tilemap>();
            Vector3Int localPoint = terrainTilemap.WorldToCell(currentPos);

            /*
            List<Vector3Int> nearbyPoints = new List<Vector3Int>();
            nearbyPoints.Add(localPoint + new Vector3Int(1, 0, 0));
            nearbyPoints.Add(localPoint + new Vector3Int(-1, 0, 0));
            nearbyPoints.Add(localPoint + new Vector3Int(0, 1, 0));
            nearbyPoints.Add(localPoint + new Vector3Int(0, -1, 0));
            nearbyPoints.Add(localPoint + new Vector3Int(1, 1, 0));
            nearbyPoints.Add(localPoint + new Vector3Int(1, -1, 0));
            nearbyPoints.Add(localPoint + new Vector3Int(-1, 1, 0));
            nearbyPoints.Add(localPoint + new Vector3Int(-1, -1, 0));
            Tilemap waterCenterTilemap = _water.transform.Find("WaterCenter").GetComponent<Tilemap>();
            nearbyPoints.RemoveAll(x => !waterCenterTilemap.HasTile(x));
            */

            string tileName = "";
            Vector3 pointCenter = Vector3.zero;

            Tilemap waterBorderTilemap = _water.transform.Find("WaterBorder").GetComponent<Tilemap>();
            Tilemap borderWest = waterBorderTilemap.transform.Find("Rivers/RiverWest/0").GetComponent<Tilemap>();
            Tilemap borderEast = waterBorderTilemap.transform.Find("Rivers/RiverEast/0").GetComponent<Tilemap>();
            Tilemap borderNorth = waterBorderTilemap.transform.Find("Rivers/RiverNorth/0").GetComponent<Tilemap>();
            Tilemap borderSouth = waterBorderTilemap.transform.Find("Rivers/RiverSouth/0").GetComponent<Tilemap>();

            if (waterBorderTilemap.HasTile(localPoint))
            {
                //Debug.Log("Tile type: " + terrainTilemap.GetTile(localPoint));
                tileName = waterBorderTilemap.GetTile(localPoint).ToString();
                pointCenter = waterBorderTilemap.GetCellCenterWorld(localPoint);
                //pointCenter = terrainTilemap.CellToWorld(localPoint) + new Vector3(xLenTile, yLenTile);
            }else if (borderWest.HasTile(localPoint))
            {
                tileName = borderWest.GetTile(localPoint).ToString();
                pointCenter = borderWest.GetCellCenterWorld(localPoint);
            }
            else if (borderEast.HasTile(localPoint))
            {
                tileName = borderEast.GetTile(localPoint).ToString();
                pointCenter = borderEast.GetCellCenterWorld(localPoint);
            }
            else if (borderNorth.HasTile(localPoint))
            {
                tileName = borderNorth.GetTile(localPoint).ToString();
                pointCenter = borderNorth.GetCellCenterWorld(localPoint);
            }
            else if (borderSouth.HasTile(localPoint))
            {
                tileName = borderSouth.GetTile(localPoint).ToString();
                pointCenter = borderSouth.GetCellCenterWorld(localPoint);
            }
            else
            {
                (int, List<(int, int)>) obstacles = _levelStageManager.GetLakeDescriptionSO().ObstaclesDescription;
                List<Tilemap> tilemapsObstacles = _obstacles.GetComponent<ObstaclesLakeComponent>().GetAllActiveObs(obstacles.Item1, obstacles.Item2);
                foreach (Tilemap t in tilemapsObstacles)
                {
                    if (t.HasTile(localPoint))
                    {
                        //Debug.Log("Tile type: " + t.GetTile(localPoint));
                        tileName = t.GetTile(localPoint).ToString();
                        pointCenter = t.GetCellCenterWorld(localPoint);
                        //pointCenter = t.CellToWorld(localPoint) + new Vector3(xLenTile, yLenTile);
                    }
                }
            }

            Vector3 newPoint = GetTilePointInsideLake(tileName, currentPos, pointCenter);

            if (newPoint == currentPos) return (false, Vector3.zero);

            /*


            List<Vector3> trueNearbyPoints = new List<Vector3>();
            foreach(Vector3Int point in nearbyPoints)
            {
                trueNearbyPoints.Add(waterCenterTilemap.CellToWorld(point) + new Vector3(1.5f, 1.5f));
            }

            Vector3 newPoint;

            newPoint = trueNearbyPoints.OrderByDescending(adj => Vector3.Distance(currentPos, adj)).Last();

            foreach(Vector3 p in trueNearbyPoints)
            {
                TileGraphComponent.DrawRayPointToPoint(currentPos, p);
            }

            */

            return (true, newPoint);
        }


        private Vector3 GetTilePointInsideLake(string tileName, Vector3 currentPosOfObj, Vector3 pointCenterOfTile)
        {
            tileName = tileName.Split(" ")[0];
            //Debug.Log("tileName = " + tileName);
            Vector3 ret = currentPosOfObj;
            switch (tileName)
            {
                case tilesetTerrainUpperLeft:
                    //this if (and the following ones) mean: "if you are not inside the water..."
                    if(!(currentPosOfObj.x > pointCenterOfTile.x && currentPosOfObj.y < pointCenterOfTile.y))
                    {
                        ret = pointCenterOfTile + new Vector3(xLenTile/4, -yLenTile/4, 0);
                    }
                    break;
                case tilesetTerrainUpper:
                    if(!(currentPosOfObj.y < pointCenterOfTile.y))
                    {
                        ret = pointCenterOfTile;
                    }
                    break;
                case tilesetTerrainUpperRight:
                    if (!(currentPosOfObj.x < pointCenterOfTile.x && currentPosOfObj.y < pointCenterOfTile.y))
                    {
                        ret = pointCenterOfTile + new Vector3(-xLenTile / 4, -yLenTile / 4, 0);
                    }
                    break;
                case tilesetIslandLowerRight:
                    //here it's easier to say: "if you are inside the terrain..."
                    if ((currentPosOfObj.x > pointCenterOfTile.x && currentPosOfObj.y < pointCenterOfTile.y))
                    {
                        ret = pointCenterOfTile;
                    }
                    break;
                case tilesetIslandLowerLeft:
                    if ((currentPosOfObj.x < pointCenterOfTile.x && currentPosOfObj.y < pointCenterOfTile.y))
                    {
                        ret = pointCenterOfTile;
                    }
                    break;
                case tilesetTerrainLeft:
                    if(!(currentPosOfObj.x > pointCenterOfTile.x))
                    {
                        ret = pointCenterOfTile + new Vector3(xLenTile / 4, 0, 0);
                    }
                    break;
                case tilesetTerrainRight:
                    if (!(currentPosOfObj.x < pointCenterOfTile.x))
                    {
                        ret = pointCenterOfTile + new Vector3(-xLenTile / 4, 0, 0);
                    }
                    break;
                case tilesetIslandUpperRight:
                    if ((currentPosOfObj.x > pointCenterOfTile.x && currentPosOfObj.y > pointCenterOfTile.y))
                    {
                        ret = pointCenterOfTile;
                    }
                    break;
                case tilesetIslandUpperLeft:
                    if ((currentPosOfObj.x < pointCenterOfTile.x && currentPosOfObj.y > pointCenterOfTile.y))
                    {
                        ret = pointCenterOfTile;
                    }
                    break;
                case tilesetTerrainLowerLeft:
                    if (!(currentPosOfObj.x > pointCenterOfTile.x && currentPosOfObj.y > pointCenterOfTile.y))
                    {
                        ret = pointCenterOfTile + new Vector3(xLenTile / 4 + xLenTile / 6, yLenTile / 4 + yLenTile / 6, 0);
                    }
                    break;
                case tilesetTerrainLower:
                    if(!(currentPosOfObj.y > pointCenterOfTile.y))
                    {
                        ret = pointCenterOfTile + new Vector3(0, yLenTile / 4, 0);
                    }
                    break;
                case tilesetTerrainLowerRight:
                    if (!(currentPosOfObj.x < pointCenterOfTile.x && currentPosOfObj.y > pointCenterOfTile.y))
                    {
                        ret = pointCenterOfTile + new Vector3(-xLenTile / 4, yLenTile / 4, 0);
                    }
                    break;

                default:
                    break;
            }

            //Debug.Log("CORRECTION APPLIED. OLD POINT WAS " + currentPosOfObj + ", NEW POINT IS " + ret);

            return ret;
        }





        //check if a point is in a terrain tile, and around it there are terrain tiles
        public bool IsEntityInTerrain(Vector3 point)
        {
            //simple case
            if (base.Contains(point)) return false;

            Tilemap terrainTilemap = _terrain.GetComponent<Tilemap>();
            Vector3Int localPoint = terrainTilemap.WorldToCell(point);

            List<Vector3Int> nearbyPoints = new List<Vector3Int>();
            nearbyPoints.Add(localPoint + new Vector3Int(1, 0, 0));
            nearbyPoints.Add(localPoint + new Vector3Int(-1, 0, 0));
            nearbyPoints.Add(localPoint + new Vector3Int(0, 1, 0));
            nearbyPoints.Add(localPoint + new Vector3Int(0, -1, 0));
            nearbyPoints.Add(localPoint + new Vector3Int(1, 1, 0));
            nearbyPoints.Add(localPoint + new Vector3Int(1, -1, 0));
            nearbyPoints.Add(localPoint + new Vector3Int(-1, 1, 0));
            nearbyPoints.Add(localPoint + new Vector3Int(-1, -1, 0));
            nearbyPoints.RemoveAll(x => terrainTilemap.HasTile(x));

            if (nearbyPoints.Count > 0) return true;

            return false;

        }




#if UNITY_EDITOR
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                _playerObject.GetComponent<PlayerController>().AddBreadPoints(20);
                CompleteLake();
            }
        }
#endif

    }
}



