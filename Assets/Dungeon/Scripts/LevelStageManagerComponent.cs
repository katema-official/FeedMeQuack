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
            }
            else
            {
                GoToNextLevel();    //this was the last stage of this level, go to the next level.
            }
        }

        //#######################################################################################################################################################
        //#######################################################################################################################################################
        //#######################################################################################################################################################
        //#######################################################################################################################################################
        //#######################################################################################################################################################


        public void GoToLake(EnumsDungeon.CompassDirection exitDirectionFromCurrentLake)
        {
            //when this function gets called, the parameters tell in which lake the player currently is in (_xOfCurrentLake and
            //_yOfCurrentLake) and the direction in which it moved (north, south, west or east), and, based on this information,
            //moves the player in another lake.

            //TODO: dopo che hai teletrasportato il giocatore nella scena dopo, per dirgli da dove sei venuto potresti usare l'opposto
            //di exitDirectionFrom...
            //-prendi il WhileLake
            //nel suo component gli imposti la direzione di provenienza (il tutto dentro questa funzione)

            _blackSquare = Instantiate(_blackSquarePrefab, new Vector3(0, 0, 0), Quaternion.identity);
            _blackSquare.GetComponent<FadeBlackComponent>().fadeToBlack();

        }



        public LakeDescriptionSO GetLakeDescriptionSO()
        {
            return _currentStageMap[_xOfCurrentLake, _yOfCurrentLake];
        }

        
        


    }

}
