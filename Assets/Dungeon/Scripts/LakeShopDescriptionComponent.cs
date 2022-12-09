using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


namespace LevelStageNamespace
{
    public class LakeShopDescriptionComponent : MonoBehaviour
    {

        protected LevelStageManagerComponent _levelStageManager;

        protected GameObject _terrain;
        protected GameObject _obstacles = null;
        protected GameObject _water;

        //per Ivan

        //function used to check if a certain point is inside the lake
        public bool Contains(Vector3 point)
        {
            //to check if a point is inside the lake, we use the tilemaps.
            //A point is considered inside the lake if it is in water and not on the terrain (terrain = both the bounding terrain of the lake and the obstacles)
            Tilemap terrainTilemap = _terrain.GetComponent<Tilemap>();
            Vector3Int localPoint = terrainTilemap.WorldToCell(point);

            Tilemap waterCenterTilemap = _water.transform.Find("WaterCenter").GetComponent<Tilemap>();
            if (!waterCenterTilemap.HasTile(localPoint))
            {
                Debug.Log("point not inside watercenter");
                return false;
            }


            if (terrainTilemap.HasTile(localPoint))
            {
                Debug.Log("point in terrain");
                return false;
            }

            //if there are no obstacles, the only check that needed to be done was against the external terrain
            if (_obstacles)
            {
                (int, List<(int, int)>) obstacles = _levelStageManager.GetLakeDescriptionSO().ObstaclesDescription;
                List<Tilemap> tilemapsObstacles = _obstacles.GetComponent<ObstaclesLakeComponent>().GetAllActiveObs(obstacles.Item1, obstacles.Item2);
                foreach(Tilemap t in tilemapsObstacles)
                {
                    if(t.HasTile(localPoint)){
                        Debug.Log("Point in obstacle");
                        return false;
                    }
                }
            }

            return true;





            float range = 10000f;

            Ray rayNorth = new Ray(point, new Vector3(0, 1, 0));
            Ray raySouth = new Ray(point, new Vector3(0, -1, 0));
            Ray rayWest = new Ray(point, new Vector3(1, 0, 0));
            Ray rayEast = new Ray(point, new Vector3(-1, 0, 0));

            Debug.DrawRay(point, new Vector3(0, 1, 0) * range, Color.red, 10f, false);
            Debug.DrawRay(point, new Vector3(0, -1, 0) * range, Color.red, 10f, false);
            Debug.DrawRay(point, new Vector3(1, 0, 0) * range, Color.red, 10f, false);
            Debug.DrawRay(point, new Vector3(-1, 0, 0) * range, Color.red, 10f, false);

            RaycastHit2D hit1 = Physics2D.Raycast(new Vector2(point.x, point.y), new Vector2(0, 1), range, LayerMask.GetMask("WaterLayer"));
            RaycastHit2D hit2 = Physics2D.Raycast(new Vector2(point.x, point.y), new Vector2(0, -1), range, LayerMask.GetMask("WaterLayer"));
            RaycastHit2D hit3 = Physics2D.Raycast(new Vector2(point.x, point.y), new Vector2(1, 0), range, LayerMask.GetMask("WaterLayer"));
            RaycastHit2D hit4 = Physics2D.Raycast(new Vector2(point.x, point.y), new Vector2(-1, 0), range, LayerMask.GetMask("WaterLayer"));


            if (hit1.collider && hit2.collider && hit3.collider && hit4.collider)
            {
                //Debug.Log("INSIDE");
                return true;
            }
            else
            {
                //Debug.Log("OUTSIDE");
                return false;
            }
        }

        //Method used to get the bounds of the terrain
        public Bounds GetTerrainBounds()
        {
            return _terrain.GetComponent<TilemapRenderer>().bounds;

            /*
            GameObject terrain = transform.Find("Terrain").gameObject;
            float terrainWidth = terrain.transform.localScale.x;
            float terrainHeight = terrain.transform.localScale.y;
            float terrainCenterX = terrain.transform.position.x;
            float terrainCenterY = terrain.transform.position.y;
            Vector2 topRightCorner = new Vector2(1, 1);
            Vector2 edgeVector = Camera.main.ViewportToWorldPoint(topRightCorner);
            float height = edgeVector.y * 2;
            float width = edgeVector.x * 2;
            float cameraWidth = width;
            float cameraHeight = height;
            return (terrainWidth, terrainHeight);
            */
        }





        // Start is called before the first frame update
        protected virtual void Awake()
        {
            _levelStageManager = GameObject.Find("LevelStageManagerObject").GetComponent<LevelStageManagerComponent>();

            _terrain = transform.Find("Terrain").gameObject;
            _obstacles = GameObject.Find("Obstacles")?.transform.GetChild(0).gameObject;
            _water = transform.Find("Water").gameObject;
        }

        
    }
}