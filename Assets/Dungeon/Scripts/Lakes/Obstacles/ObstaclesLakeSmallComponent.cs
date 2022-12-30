using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LevelStageNamespace
{
    public class ObstaclesLakeSmallComponent : ObstaclesLakeComponent
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }


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

        {4,  new Dictionary<int, List<int>>()
            {
                {1, new List<int>{1} }
            }
        },
    };

        //the probability, for each set, to be chosen, Must sum up to one
        private static List<float> _probabilitiesOfSets = new List<float>() { 0.60f, 0.10f, 0.25f, 0.05f };



        //a static method used to choose the obstacles that we want to place in the lake small
        public static (int, List<(int, int)>) GenerateObstaclesDescription()
        {
            //first of all, we have to choose a random ObstacleSet
            float v = Random.Range(0f, 1f);
            bool chose = false;
            int idx = 0;
            float sum = _probabilitiesOfSets[idx];
            while (!chose)
            {
                if (v <= sum)
                {
                    chose = true;
                }
                else
                {
                    idx++;
                    sum += _probabilitiesOfSets[idx];
                }
            }
            int setIndex = idx + 1; //because our sets go from 1 to n

            //then, I have to choose which quadrants do I want to activate (at least one, ok?)
            Dictionary<int, List<int>> quadrantsDict = _obstacleSetsDictionary[setIndex];
            List<int> quadrantsKeys = quadrantsDict.Keys.ToList<int>();
            List<int> quadrantsActiveIndexes = quadrantsKeys.OrderBy(x => Random.Range(0f, 1f)).Take(Random.Range(1, quadrantsKeys.Count + 1)).ToList<int>();

            //for each of these quadrants, I have to choose one Obs
            List<int> obsChosenInOrder = new List<int>();
            foreach (int i in quadrantsActiveIndexes)
            {
                int obs = Random.Range(1, quadrantsDict[i].Count + 1);
                obsChosenInOrder.Add(obs);
            }

            List<(int, int)> quadrants_obstacles = new List<(int, int)>();
            //Debug.Log("In the end, we chose set " + setIndex + "with the following quadrants-obs:");
            for (int i = 0; i < quadrantsActiveIndexes.Count; i++)
            {
                quadrants_obstacles.Add((quadrantsActiveIndexes[i], obsChosenInOrder[i]));
                //Debug.LogFormat("Q {0} -> Obs {1}", quadrants_obstacles[i].Item1, quadrants_obstacles[i].Item2);
            }


            return (setIndex, quadrants_obstacles);
        }


        public override void SetObstacles(int setIndex, List<(int, int)> quadrants_obstacles)
        {
            base.SetObstacles(setIndex, quadrants_obstacles);
        }

    }



}