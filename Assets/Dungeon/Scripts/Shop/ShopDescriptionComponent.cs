using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LevelStageNamespace;
using System.Linq;
using PowerUpsNamespace;
using UnityEngine.Tilemaps;
using HUDNamespace;

namespace LevelStageNamespace
{
    public class ShopDescriptionComponent : LakeShopDescriptionComponent
    {

        [SerializeField] private List<ShopSO> _listOfShops;
        [SerializeField] private GameObject _powerUpPrefab;

        private GameObject _platformsForPowerUpsGameobject;
        private MapManager _mapManager;



        // Start is called before the first frame update
        void Start()
        {
            GameObject player = GameObject.FindWithTag("Player");
            player.transform.position = new Vector3(0, 0, 0);
            _levelStageManager = GameObject.Find("LevelStageManagerObject").GetComponent<LevelStageManagerComponent>();
            _platformsForPowerUpsGameobject = GameObject.Find("PlatformsForPowerUps");
            _levelStageManager.FadeIn();
            InitializeShop();
        }

        protected override void Awake()
        {
            base.Awake();
            _mapManager = GameObject.FindObjectOfType<MapManager>();
            _mapManager.StartNewLevel();    //reset the minimap

            switch (_levelStageManager.GetCurrentLevelIndex())
            {
                case 1:
                    //Music.UniversalAudio.PlayMusic("Shop1", false);
                    break;
                case 2:
                    Music.UniversalAudio.PlayMusic("Shop2", false);
                    break;
                case 3:
                    Music.UniversalAudio.PlayMusic("Shop2", false);
                    break;
                default:
                    break;
            }

            
        }



        private void InitializeShop()
        {
            int levelIndex = _levelStageManager.GetCurrentLevelIndex();
            ShopSO currentShopSO = _listOfShops[levelIndex -1]; //-1!!! the first stage is "1", but its index in the list is "0"!

            //first of all, let's decide how many power ups do we want to generate
            int howManyPUToSell = Random.Range(currentShopSO.MinAmountOfPowerUpsToSell, currentShopSO.MaxAmountOfPowerUpsToSell + 1);

            for (int i = 0; i < _platformsForPowerUpsGameobject.transform.childCount; i++)
            {
                GameObject platform = _platformsForPowerUpsGameobject.transform.Find("P" + ((i + 1).ToString())).gameObject;
                if (i < howManyPUToSell)
                {
                    //let's enable the platforms of the items to sell
                    platform.SetActive(true);

                    //and let's put a random power up on top of them
                    int index = Random.Range(0, currentShopSO.ListOfPowerUps.Count);
                    PowerUpSO powerUpSO = currentShopSO.ListOfPowerUps[index];

                    //now let's create the real power up...
                    GameObject actualPowerUp = Instantiate(_powerUpPrefab);
                    actualPowerUp.GetComponent<PowerUpComponent>().Initialize(powerUpSO);

                    //...and put it on top of the platform
                    Bounds b = platform.GetComponent<TilemapRenderer>().bounds;
                    actualPowerUp.transform.position = b.center + new Vector3(0f, 2f, 0f);
                }
                else
                {
                    //let's disable all the other platforms
                    platform.SetActive(false);
                }
            }

        }


    }
}