using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObstaclesLakeSmallComponent : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GenerateObstaclesDescription();
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

        //then, I have to choose which quadrants do I want to activate
        Dictionary<int, List<int>> quadrantsDict = _obstacleSetsDictionary[setIndex];
        List<int> quadrantsKeys = quadrantsDict.Keys.ToList<int>();
        List<int> quadrantsActiveIndexes = quadrantsKeys.OrderBy(x => Random.Range(0f, 1f)).Take(Random.Range(0, quadrantsKeys.Count + 1)).ToList<int>();

        //for each of these quadrants, I have to choose one Obs
        List<int> obsChosenInOrder = new List<int>();
        foreach(int i in quadrantsActiveIndexes)
        {
            int obs = Random.Range(1, quadrantsDict[i].Count + 1);
            obsChosenInOrder.Add(obs);
        }
        Debug.Log("In the end, we chose set " + setIndex + "with the following quadrants-obs:");
        for(int i = 0; i < quadrantsActiveIndexes.Count; i++)
        {
            Debug.LogFormat("Q {0} -> Obs {1}", quadrantsActiveIndexes[i], obsChosenInOrder[i]);

        }


        /*List<int> my = new List<int>() {1,2,3,4,5};
        List<int> ret = my.OrderBy(x => Random.Range(0f,1f)).Take(Random.Range(0, my.Count + 1)).ToList<int>();
        foreach(int i in ret)
        {
            Debug.Log(i);
        }
        Debug.Log(ret);*/


        return (0, new List<(int, int)>());



    }



}
