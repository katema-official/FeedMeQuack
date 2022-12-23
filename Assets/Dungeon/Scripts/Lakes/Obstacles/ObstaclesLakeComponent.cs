using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace LevelStageNamespace
{
    public class ObstaclesLakeComponent : MonoBehaviour
    {
        //method to apply the given set of obstacles, and only them
        virtual public void SetObstacles(int setIndex, List<(int, int)> quadrants_obstacles)
        {
            //first of all: disable all child gameobjects
            foreach (Transform child in transform)
            {
                DisableGameobjectRecursively(child.gameObject);
            }

            //there are some cases in which we didn't set any obstacle explicitly (for example: starting room).
            //In that case, don't do anything
            if (setIndex < 0 || quadrants_obstacles == null)
            {
                return;
            }

            //then, re-activate only the right ones
            string obstacleSetSTRING = "ObstacleSet" + setIndex;
            GameObject obstacleSetGO = transform.Find(obstacleSetSTRING).gameObject;
            obstacleSetGO.SetActive(true);

            GameObject quadrant;
            GameObject obs;
            foreach ((int, int) quadrant_obs in quadrants_obstacles)
            {
                string quadrantSTRING = "Quadrant" + quadrant_obs.Item1;
                string obsSTRING = "Obs" + quadrant_obs.Item2;

                quadrant = obstacleSetGO.transform.Find(quadrantSTRING).gameObject;
                obs = quadrant.transform.Find(obsSTRING).gameObject;

                quadrant.SetActive(true);
                obs.SetActive(true);
            }


        }

        //method to disable a gameobject and ALL of its children recursively
        protected void DisableGameobjectRecursively(GameObject go)
        {
            foreach (Transform child in go.transform)
            {
                DisableGameobjectRecursively(child.gameObject);
            }
            go.SetActive(false);
        }



        //method to retrieve the list of all active Obs (useful to study their tilemaps)
        public List<Tilemap> GetAllActiveObs(int setIndex, List<(int, int)> quadrants_obstacles)
        {
            if(setIndex < 0 || quadrants_obstacles == null)
            {
                return new List<Tilemap>();
            }

            string obstacleSetSTRING = "ObstacleSet" + setIndex;
            GameObject obstacleSetGO = transform.Find(obstacleSetSTRING).gameObject;

            GameObject quadrant;
            GameObject obs;

            List<Tilemap> ret = new List<Tilemap>();

            foreach ((int, int) quadrant_obs in quadrants_obstacles)
            {
                string quadrantSTRING = "Quadrant" + quadrant_obs.Item1;
                string obsSTRING = "Obs" + quadrant_obs.Item2;

                quadrant = obstacleSetGO.transform.Find(quadrantSTRING).gameObject;
                obs = quadrant.transform.Find(obsSTRING).gameObject;

                ret.Add(obs.GetComponent<Tilemap>());
            }

            return ret;
        }
    }
}