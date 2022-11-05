using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class GoToTrueLakeScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public LevelStageNamespace.StageSO StageToTry;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            SceneManager.LoadScene("StartRunLoading");
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            bool[,] bitMap = LevelStageNamespace.GenerateDungeonClass.GenerateStage(StageToTry);
            for(int i = 55; i < 75; i++)
            {
                string row = "";
                for(int j = 55; j < 75; j++)
                {
                    if(bitMap[i,j] == false)
                    {
                        row += "-";
                    }
                    else
                    {
                        row += "+";
                        //Debug.Log("room at [" + i + "," + j + "]");
                    }
                }
                Debug.Log(row);
            }
        }
    }
}
