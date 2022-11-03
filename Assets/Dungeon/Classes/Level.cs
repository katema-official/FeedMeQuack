using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelStageNamespaces
{

    public class Level
    {
        private List<Stage> _stagesList;

        public Level(int nStages)
        {
            //nStages means "how many stages should this level have?
            for(int i = 0; i < nStages; i++)
            {
                _stagesList.Add(new Stage());
            }


        }




    }
}



