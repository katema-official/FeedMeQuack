using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelStageNamespace
{

    public class LevelStageManagerComponent : MonoBehaviour
    {

        //this script controls how many levels there are, how many stages there must be for each level, and
        //so on.

        [SerializeField] public List<LevelSO> Levels;

        private Dictionary<int, int> _numberOfStagesPerLevel; //<i,j> means: "level i has j stages in total"


        private int _currentLevel;
        private int _currentStage;



        // Start is called before the first frame update
        void Start()
        {

            for(int i = 1; i < Levels.Count + 1; i++)
            {
                LevelSO level = Levels[i];
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
            

        }

        public void GoToNextStage()
        {
            if (_currentStage < _numberOfStagesPerLevel[_currentLevel]) //if in this level there is a next stage...
            {
                _currentStage += 1;
                GenerateStage();
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
        
        private void GenerateStage()
        {
            //this method needs to generate the current stage.
            StageSO stageDescription = GetStage(GetLevel(_currentLevel), _currentStage);


        }


    }

}
