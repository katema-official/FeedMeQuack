using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstaclesLakeSmallComponent : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }



    //this value must be updated whenever the prefab is modified (I could'nt think of anything better, sorry
    private static int _nObstaclesSet = 3;

    //yeah yeah I know it's very ugly but I can't come up with anything better at the moment, sorryyyyyyyy
    private static Dictionary<int, Dictionary<int, List<int>>> _obstacleSetsDictionary = new Dictionary<int, Dictionary<int, List<int>>>()
    {
        {1, new Dictionary<int, List<int>>()
            {
                {1, new List<int>(){1,2,3,4,5} },
                {2, new List<int>(){1,2,3,4,5} },
                {3, new List<int>(){1,2,3,4,5} },
                {4, new List<int>(){1,2,3,4,5} }
            }
        },

        {2,  new Dictionary<int, List<int>>()
            {
                {1, new List<int>{1} }
            }
        },

        {3, new Dictionary<int, List<int>>()
            {
                {1, new List<int>(){1,2,3,4} },
                {2, new List<int>(){1,2,3,4} }
            }
        },
    };





    public static (int, List<(int, int)>) GenerateObstaclesDescription()
    {
        
        //first of all, we have to choose a random ObstacleSet
        int setIndex = Random.Range(1, _nObstaclesSet + 1);

        //then, to consider its quadrants, I have to go from the index to the actual content of the ObstacleSet
        //GameObject obstacleSetGO = _obstacleSets

        return (0, new List<(int, int)>());



    }



}
