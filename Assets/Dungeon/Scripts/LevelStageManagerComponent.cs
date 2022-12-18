using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LevelStageNamespace
{

    public class LevelStageManagerComponent : MonoBehaviour
    {

        //this script controls how many levels there are, how many stages there must be for each level, and
        //so on.

        [SerializeField] private GameObject _blackSquarePrefab;
        private GameObject _blackSquare;
        [SerializeField] public List<LevelSO> Levels;

        private Dictionary<int, int> _numberOfStagesPerLevel = new Dictionary<int, int>(); //<i,j> means: "level i has j stages in total"


        private int _currentLevel;
        private int _currentStage;

        private int _xOfCurrentLake;
        private int _yOfCurrentLake;


        // Start is called before the first frame update
        void Start()
        {

            for(int i = 1; i < Levels.Count + 1; i++)
            {
                LevelSO level = Levels[i-1];
                _numberOfStagesPerLevel.Add(i, level.Stages.Count);
            }

            _currentLevel = 0;  //level 0 and stage 0 DO NO EXIST. Only levels and stages with 1+ index are ok.
            _currentStage = 0;
            GoToNextLevel();
        }

        // Update is called once per frame
        void Update()
        {

        }

        //#######################################################################################################################################################
        //#######################################################################################################################################################
        //#######################################################################################################################################################
        //#######################################################################################################################################################
        //#######################################################################################################################################################

        private LevelSO GetLevel(int i)
        {
            return Levels[i - 1];
        }

        private StageSO GetStage(LevelSO level, int i)
        {
            return level.Stages[i - 1];
        }

        public int GetCurrentLevelIndex()
        {
            return _currentLevel;
        }

        public int GetCurrentStageIndex()
        {
            return _currentStage;
        }



        //#######################################################################################################################################################
        //#######################################################################################################################################################
        //#######################################################################################################################################################
        //#######################################################################################################################################################
        //#######################################################################################################################################################

        public void GoToNextLevel()
        {
            if(_numberOfStagesPerLevel.ContainsKey(_currentLevel + 1))  //if a next level exists...
            {
                _currentLevel += 1;
                _currentStage = 0;
                GoToNextStage();
            }
            else
            {
                Debug.Log("No more levels!!!");
            }
            

        }

        private bool[,] _currentStageBitMap;
        private LakeDescriptionSO[,] _currentStageMap;    //I think these numbers are big enough for whatever map we might come up with

        public void GoToNextStage()
        {
            if (_currentStage < _numberOfStagesPerLevel[_currentLevel]) //if in this level there is a next stage...
            {
                _xOfCurrentLake = GenerateDungeonClass.StartMapX;
                _yOfCurrentLake = GenerateDungeonClass.StartMapY;
                _currentStage += 1;
                _currentStageBitMap = GenerateDungeonClass.GenerateStageLayout(GetStage(GetLevel(_currentLevel), _currentStage));
                //we have the bitmap of this stage. Let's now get the actual stage description (a matrix of LakeDescriptionSO)
                _currentStageMap = GenerateDungeonClass.GenerateStageWithActualLakeDescriptionSO(_currentStageBitMap, GetStage(GetLevel(_currentLevel), _currentStage));
                
                //could also have called GoToLake, but this should make things simpler: the initial lake is always small and without enemies, after all.
                //The only thing to touch is the rivers.
                SceneManager.LoadScene("LakeSmall");

                //MUSIC: Reproduce calm music when entering new stage
                Music.UniversalAudio.PlayMusic("Swimming", false);
            }
            else
            {
                GoToNextLevel();    //this was the last stage of this level, go to the next level.
            }
        }

        //#######################################################################################################################################################
        //#######################################################################################################################################################
        //################################################################### EXIT LAKE LOGIC ###################################################################
        //#######################################################################################################################################################
        //#######################################################################################################################################################


        public void ExitLake(EnumsDungeon.CompassDirection exitDirectionFromCurrentLake)
        {

            //SPECIAL CASE: when the player is exiting from the stage, we have to check its BreadPoints.
            //If they are enough, it can go to the shop. Otherwise, it's game over.
            if (GetLakeDescriptionSO().IsFinalRoom && exitDirectionFromCurrentLake == GetLakeDescriptionSO().ExitStageDirection)
            {
                if (GameObject.FindWithTag("Player").GetComponent<Player.PlayerController>().GetBreadPoints() >= GetStage(GetLevel(_currentLevel), _currentStage).BreadPointsRequiredToCompleteStage)
                {
                    Debug.Log("CE L'HAI FATTA, HAI MANGIATO ABBASTANZA!");
                    GameObject.FindWithTag("Player").GetComponent<Player.PlayerController>().NotifyStageCompleted(GetStage(GetLevel(_currentLevel), _currentStage).BreadPointsRequiredToCompleteStage);
                    FadeOutGoToShop();
                    return;

                }
                else
                {
                    Debug.Log("COMPLIMENTI, SEI MORTO DI FAME!");
                    //Destroy(this.gameObject);
                    SceneManager.LoadScene("GameOverScreen");
                    return;
                }
            }



            //when this function gets called, the parameters tell in which lake the player currently is in (_xOfCurrentLake and
            //_yOfCurrentLake of this class) and the direction in which it moved (north, south, west or east), and, based
            //on this information, moves the player in another lake.

            //first: a fade out effect
            FadeOutGoToLake();

            //second: update the x and y of the current stage depending on where the player exited, and save, in the LakeDescriptionSO of the
            //new room, from where the player arrives
            switch (exitDirectionFromCurrentLake)
            {
                case EnumsDungeon.CompassDirection.North:
                    _xOfCurrentLake -= 1;
                    _currentStageMap[_xOfCurrentLake, _yOfCurrentLake].PlayerSpawnDirection = EnumsDungeon.CompassDirection.South;
                    break;
                case EnumsDungeon.CompassDirection.South:
                    _xOfCurrentLake += 1;
                    _currentStageMap[_xOfCurrentLake, _yOfCurrentLake].PlayerSpawnDirection = EnumsDungeon.CompassDirection.North;
                    break;
                case EnumsDungeon.CompassDirection.West:
                    _yOfCurrentLake -= 1;
                    _currentStageMap[_xOfCurrentLake, _yOfCurrentLake].PlayerSpawnDirection = EnumsDungeon.CompassDirection.East;
                    break;
                case EnumsDungeon.CompassDirection.East:
                    _yOfCurrentLake += 1;
                    _currentStageMap[_xOfCurrentLake, _yOfCurrentLake].PlayerSpawnDirection = EnumsDungeon.CompassDirection.West;
                    break;
            }

            Debug.Log("The player comes from " + GetLakeDescriptionSO().PlayerSpawnDirection);
            Debug.LogFormat("New room is: [{0},{1}]", _xOfCurrentLake, _yOfCurrentLake);

            //this function continues in EnterLake, called by the black square

        }

        //#######################################################################################################################################################
        //#######################################################################################################################################################
        //################################################################### ENTER LAKE LOGIC ##################################################################
        //#######################################################################################################################################################
        //#######################################################################################################################################################

        public void EnterLake()
        {
            //third: go to the right lake
            switch (GetLakeDescriptionSO().Dimension)
            {
                case EnumsDungeon.LakeDimension.Small:
                    SceneManager.LoadScene("LakeSmall");
                    break;
                case EnumsDungeon.LakeDimension.Medium:
                    SceneManager.LoadScene("LakeMedium");    //TODO: LakeMedium
                    break;
                case EnumsDungeon.LakeDimension.Large:
                    SceneManager.LoadScene("LakeSmall");    //TODO: LakeLarge
                    break;
                default:
                    Debug.Log("Non dovresti assolutamente essere qui");
                    break;
            }
        }

        //#######################################################################################################################################################
        //#######################################################################################################################################################
        //###################################################################### SHOP LOGIC #####################################################################
        //#######################################################################################################################################################
        //#######################################################################################################################################################

        public void EnterShop()
        {
            SceneManager.LoadScene("Shop1");
        }

        public void ExitShop()
        {
            GoToNextStage();
        }





        //#######################################################################################################################################################
        //#######################################################################################################################################################
        //################################################################ FADE AND GO SOMEWHERE ################################################################
        //#######################################################################################################################################################
        //#######################################################################################################################################################

        public void FadeOutGoToLake()
        {
            _blackSquare = Instantiate(_blackSquarePrefab, new Vector3(0, 0, 0), Quaternion.identity);
            _blackSquare.GetComponent<FadeBlackComponent>().fadeToBlackAndGoToLake();  //-> will call EnterLake when it's done
        }

        public void FadeIn()
        {
            _blackSquare = Instantiate(_blackSquarePrefab, new Vector3(0, 0, 0), Quaternion.identity);
            _blackSquare.GetComponent<FadeBlackComponent>().fadeFromBlack();  //-> will call EnterLake when it's done
        }


        public void FadeOutGoToShop()
        {
            _blackSquare = Instantiate(_blackSquarePrefab, new Vector3(0, 0, 0), Quaternion.identity);
            _blackSquare.GetComponent<FadeBlackComponent>().fadeToBlackAndGoToShop();  //-> will call EnterShop when it's done
        }

        //#######################################################################################################################################################
        //#######################################################################################################################################################
        //################################################# UTILITY METHODS FOR LakeDescriptionComponent ########################################################
        //#######################################################################################################################################################
        //#######################################################################################################################################################



        public LakeDescriptionSO GetLakeDescriptionSO()
        {
            return _currentStageMap[_xOfCurrentLake, _yOfCurrentLake];
        }

        public void SetLakeAsCleared()
        {
            _currentStageMap[_xOfCurrentLake, _yOfCurrentLake].IsLakeCleared = true;
        }

        public bool IsCurrentLakeCleared()
        {
            return _currentStageMap[_xOfCurrentLake, _yOfCurrentLake].IsLakeCleared;
        }

        public BreadSpawnSO GetBreadSpawnSO()
        {
            return GetStage(GetLevel(_currentLevel), _currentStage).ListBreadSpawnSO[(int) GetLakeDescriptionSO().Dimension];
        }


        





    }

}
