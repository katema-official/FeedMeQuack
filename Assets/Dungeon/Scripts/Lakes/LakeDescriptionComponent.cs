using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelStageNamespace {
    public class LakeDescriptionComponent : MonoBehaviour
    {
        //component that specifies the nature of a lake

        private LevelStageManagerComponent _levelStageManager;
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
        public float OffsetXPlayer;
        public float OffsetYPlayer;

        //radius of area around the player in which we don't want enemies to spawn
        public float RadiusAroundPlayer;

        //radius of area arount the enemies in which we don't want other enemies to spawn
        public float RadiusAroundEnemy;



        private GameObject _lake;
        private GameObject _terrain;


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
            _terrain = transform.Find("Terrain").gameObject;

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
                        _playerObject.transform.position = transform.Find("Rivers/North").transform.position + new Vector3(0, -OffsetYPlayer, 0);
                        break;
                    case EnumsDungeon.CompassDirection.South:
                        _playerObject.transform.position = transform.Find("Rivers/South").transform.position + new Vector3(0, OffsetYPlayer, 0);
                        break;
                    case EnumsDungeon.CompassDirection.West:
                        _playerObject.transform.position = transform.Find("Rivers/West").transform.position + new Vector3(OffsetXPlayer, 0, 0);
                        break;
                    case EnumsDungeon.CompassDirection.East:
                        _playerObject.transform.position = transform.Find("Rivers/East").transform.position + new Vector3(-OffsetXPlayer, 0, 0);
                        break;
                }

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

                //the setup of the lake is done. Now we wait until the player enters in the actual lake from the river. To do so, he will need
                //to pass through the TriggerEnteredCollider of the river in which he is.

            }

        }


        //########################################################################################################################################################
        //########################################################################################################################################################
        //################################################################# RIVERS MANAGEMENT ####################################################################
        //########################################################################################################################################################
        //########################################################################################################################################################


        private void ManageRiversOfthisLake()
        {
            _northRiver = transform.Find("Rivers/North").gameObject;
            _southRiver = transform.Find("Rivers/South").gameObject;
            _westRiver = transform.Find("Rivers/West").gameObject;
            _eastRiver = transform.Find("Rivers/East").gameObject;
            if (_lakeDescriptionForThisLake.HasNorthRiver == false)
            {
                //if the north lake is not present, we obscure its sprite, and leave the colliders as they are
                var t = _northRiver.transform.Find("Sprite").gameObject.GetComponent<Transform>().localScale;
                _northRiver.transform.Find("Sprite").gameObject.GetComponent<Transform>().localScale = new Vector3(0, t.y, t.z);
                _northRiver.transform.Find("TriggerEnteredCollider").gameObject.SetActive(false);
            }
            else
            {
                //otherwise, we leave the sprite as it is, but we remove the collider (We do the same for the other rivers)
                _northRiver.transform.Find("BlockingCollider").gameObject.SetActive(false);

            }
            if (_lakeDescriptionForThisLake.HasSouthRiver == false)
            {
                var t = _southRiver.transform.Find("Sprite").gameObject.GetComponent<Transform>().localScale;
                _southRiver.transform.Find("Sprite").gameObject.GetComponent<Transform>().localScale = new Vector3(0, t.y, t.z);
                _southRiver.transform.Find("TriggerEnteredCollider").gameObject.SetActive(false);
            }
            else
            {
                _southRiver.transform.Find("BlockingCollider").gameObject.SetActive(false);
            }
            if (_lakeDescriptionForThisLake.HasWestRiver == false)
            {
                var t = _westRiver.transform.Find("Sprite").gameObject.GetComponent<Transform>().localScale;
                _westRiver.transform.Find("Sprite").gameObject.GetComponent<Transform>().localScale = new Vector3(t.x, 0, t.z);
                _westRiver.transform.Find("TriggerEnteredCollider").gameObject.SetActive(false);
            }
            else
            {
                _westRiver.transform.Find("BlockingCollider").gameObject.SetActive(false);
            }
            if (_lakeDescriptionForThisLake.HasEastRiver == false)
            {
                var t = _eastRiver.transform.Find("Sprite").gameObject.GetComponent<Transform>().localScale;
                _eastRiver.transform.Find("Sprite").gameObject.GetComponent<Transform>().localScale = new Vector3(t.x, 0, t.z);
                _eastRiver.transform.Find("TriggerEnteredCollider").gameObject.SetActive(false);
            }
            else
            {
                _eastRiver.transform.Find("BlockingCollider").gameObject.SetActive(false);
            }
        }

        //called by EnterPlayerInLakeComponent to signal that the player entered the lake, and the rivers must be closed
        public void CloseLakesWithAnimation()
        {
            if(_lakeDescriptionForThisLake.HasNorthRiver == true)
            {
                _northRiver.transform.Find("BlockingCollider").gameObject.SetActive(true);
                StartCoroutine(CloseLakeCoroutine(_northRiver.transform.Find("Sprite"), EnumsDungeon.CompassDirection.North));
            }
            if (_lakeDescriptionForThisLake.HasSouthRiver == true)
            {
                _southRiver.transform.Find("BlockingCollider").gameObject.SetActive(true);
                StartCoroutine(CloseLakeCoroutine(_southRiver.transform.Find("Sprite"), EnumsDungeon.CompassDirection.South));
            }
            if (_lakeDescriptionForThisLake.HasWestRiver == true)
            {
                _westRiver.transform.Find("BlockingCollider").gameObject.SetActive(true);
                StartCoroutine(CloseLakeCoroutine(_westRiver.transform.Find("Sprite"), EnumsDungeon.CompassDirection.West));
            }
            if (_lakeDescriptionForThisLake.HasEastRiver == true)
            {
                _eastRiver.transform.Find("BlockingCollider").gameObject.SetActive(true);
                StartCoroutine(CloseLakeCoroutine(_eastRiver.transform.Find("Sprite"), EnumsDungeon.CompassDirection.East));
            }
        }

        private IEnumerator CloseLakeCoroutine(Transform river, EnumsDungeon.CompassDirection direction)
        {
            //TODO: for this moment, I'll just stick the values here, because otherwise I would have to manage too many variables in this monobehaviour,
            //and I also don't know if I want exactly this kind of animation.
            float timeBetweenShrinking = 0.005f;
            float percentageToReducePerUnitOfTime = 0.01f;

            float length;
            float amountToReduce;
            Vector3 reduceVector = Vector3.zero;
            switch (direction)
            {
                case EnumsDungeon.CompassDirection.North:
                case EnumsDungeon.CompassDirection.South:
                    length = river.localScale.x;
                    amountToReduce = length * percentageToReducePerUnitOfTime;
                    reduceVector = new Vector3(amountToReduce, 0, 0);
                    break;

                case EnumsDungeon.CompassDirection.West:
                case EnumsDungeon.CompassDirection.East:
                    length = river.localScale.y;
                    amountToReduce = length * percentageToReducePerUnitOfTime;
                    reduceVector = new Vector3(0, amountToReduce, 0);
                    break;
            }

            switch (direction)
            {
                case EnumsDungeon.CompassDirection.North:
                    while(river.localScale.x > 0)
                    {
                        river.localScale -= reduceVector;
                        yield return new WaitForSeconds(timeBetweenShrinking);
                    }
                    break;
                case EnumsDungeon.CompassDirection.South:
                    while (river.localScale.x > 0)
                    {
                        river.localScale -= reduceVector;
                        yield return new WaitForSeconds(timeBetweenShrinking);
                    }
                    break;
                case EnumsDungeon.CompassDirection.West:
                    while (river.localScale.y > 0)
                    {
                        river.localScale -= reduceVector;
                        yield return new WaitForSeconds(timeBetweenShrinking);
                    }
                    break;
                case EnumsDungeon.CompassDirection.East:
                    while (river.localScale.y > 0)
                    {
                        river.localScale -= reduceVector;
                        yield return new WaitForSeconds(timeBetweenShrinking);
                    }
                    break;
            }


            yield return null;
        }

        public void OpenLakesWithAnimation()
        {

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

            float xPixelSprite = 100f;
            float yPixelSprite = 100f;

            for(int i = 0; i < _totalNumberOfBreadPiecesToSpawn; i++)
            {
                GameObject newBread = Instantiate(BreadToThrow);
                Vector2 spriteSize = Vector2.zero;

                Sprite sprite = null;
                switch (_arrayBreadSpawnType[i])
                {
                    case EnumsDungeon.BreadType.Small:
                        sprite = SmallBreadPrefab.GetComponent<SpriteRenderer>().sprite;
                        spriteSize = sprite.rect.size;
                        newBread.GetComponent<BreadNamespace.ThrowBreadComponent2>().BreadToSpawnPrefab = SmallBreadPrefab;
                        break;
                    case EnumsDungeon.BreadType.Medium:
                        sprite = MediumBreadPrefab.GetComponent<SpriteRenderer>().sprite;
                        spriteSize = sprite.rect.size;
                        newBread.GetComponent<BreadNamespace.ThrowBreadComponent2>().BreadToSpawnPrefab = MediumBreadPrefab;
                        break;
                    case EnumsDungeon.BreadType.Large:
                        sprite = LargeBreadPrefab.GetComponent<SpriteRenderer>().sprite;
                        spriteSize = sprite.rect.size;
                        newBread.GetComponent<BreadNamespace.ThrowBreadComponent2>().BreadToSpawnPrefab = LargeBreadPrefab;
                        break;
                }
                newBread.GetComponent<BreadNamespace.ThrowBreadComponent2>().dimension = _arrayBreadSpawnType[i];


                float amountToDivideX = spriteSize.x / xPixelSprite;
                float amountToDivideY = spriteSize.y / yPixelSprite;

                Transform breadThrownTransform = newBread.transform.Find("AirSprite");

                breadThrownTransform.gameObject.GetComponent<SpriteRenderer>().sprite = sprite;
                breadThrownTransform.localScale = new Vector3(breadThrownTransform.localScale.x / amountToDivideX, breadThrownTransform.localScale.y / amountToDivideY, 0);
                yield return new WaitForSeconds(_arrayBreadSpawnTime[i]);
            }


            yield return null;
        }


        //function used by the bread pieces to notify that they have been eaten completely
        public void NotifyBreadEaten()
        {
            _totalNumberOfBreadPiecesEaten += 1;
            if(_totalNumberOfBreadPiecesEaten == _totalNumberOfBreadPiecesToBeEaten)
            {
                //CALL A FUNCTION THAT ENDS THE LAKE
            }
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
        //################################################################# POINTS GENERATION ###################################################################
        //########################################################################################################################################################
        //########################################################################################################################################################


        //function that generates a point on the immediate outside of the lake. Is used to simulate the starting point from which
        //a piece of bread is thrown
        public (float, float) GeneratePointOutsideLake()
        {
            float widthTerrain = _terrain.transform.localScale.x;
            float heightTerrain = _terrain.transform.localScale.y;
            float xCenterTerrain = _terrain.transform.position.x;
            float yCenterTerrain = _terrain.transform.position.y;
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



