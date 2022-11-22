using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LevelStageNamespace
{
    public class LakeShopDescriptionComponent : MonoBehaviour
    {

        protected GameObject _terrain;

        //per Ivan

        //function used to check if a certain point is inside the lake
        public bool Contains(Vector3 point)
        {
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


            if (hit1 && hit2 && hit3 && hit4)
            {
                Debug.Log("INSIDE");
                return true;
            }
            else
            {
                Debug.Log("OUTSIDE");
                return false;
            }
        }

        //Method used to get the bounds of the terrain
        public Bounds GetTerrainBounds()
        {
            return _terrain.GetComponent<SpriteRenderer>().bounds;

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
            _terrain = transform.Find("Terrain").gameObject;
        }

        
    }
}