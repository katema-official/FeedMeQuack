using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapComponentToStudy : MonoBehaviour
{

    public Tilemap tileMap = null;

    public List<Vector3> availablePlaces;


    // Start is called before the first frame update
    void Start()
    {
        /*tileMap = transform.GetComponent<Tilemap>();
        availablePlaces = new List<Vector3>();
        Debug.Log("min and max: " + tileMap.cellBounds.xMin + ", " + tileMap.cellBounds.xMax);

        for (int n = tileMap.cellBounds.xMin; n < tileMap.cellBounds.xMax; n++)
        {
            for (int p = tileMap.cellBounds.yMin; p < tileMap.cellBounds.yMax; p++)
            {
                Vector3Int localPlace = (new Vector3Int(n, p, (int)tileMap.transform.position.y));
                Debug.Log("LocalPlace is " + localPlace);
                Vector3 place = tileMap.CellToWorld(localPlace);
                if (tileMap.HasTile(localPlace))
                {
                    //Tile at "place"
                    availablePlaces.Add(place);
                    Debug.Log("Can place in " + place);
                }
                else
                {
                    //No tile at "place"
                    Debug.Log("Cannot place in " + place);
                }

                Vector3 point = place;
                float range = 1000f;

                Ray rayNorth = new Ray(point, new Vector3(0, 1, 0));
                Ray raySouth = new Ray(point, new Vector3(0, -1, 0));
                Ray rayWest = new Ray(point, new Vector3(1, 0, 0));
                Ray rayEast = new Ray(point, new Vector3(-1, 0, 0));

                Debug.DrawRay(point, new Vector3(0, 1, 0) * range, Color.red, 10f, false);
                Debug.DrawRay(point, new Vector3(0, -1, 0) * range, Color.red, 10f, false);
                Debug.DrawRay(point, new Vector3(1, 0, 0) * range, Color.red, 10f, false);
                Debug.DrawRay(point, new Vector3(-1, 0, 0) * range, Color.red, 10f, false);

                RaycastHit2D hit1 = Physics2D.Raycast(new Vector2(point.x, point.y), new Vector2(0, 1), range);
                RaycastHit2D hit2 = Physics2D.Raycast(new Vector2(point.x, point.y), new Vector2(0, -1), range);
                RaycastHit2D hit3 = Physics2D.Raycast(new Vector2(point.x, point.y), new Vector2(1, 0), range);
                RaycastHit2D hit4 = Physics2D.Raycast(new Vector2(point.x, point.y), new Vector2(-1, 0), range);//, LayerMask.GetMask("WaterLayer"));

                RaycastHit2D ppp = Physics2D.Raycast(new Vector2(point.x, point.y), Vector2.zero, 0);
                

                if (hit1.collider && hit2.collider && hit3.collider && hit4.collider)
                {
                    //Debug.Log("INSIDE");
                    Debug.LogFormat("Place: {4}, Respective names of layers: {0}, {1}, {2} and {3}",
                        LayerMask.LayerToName(hit1.collider.gameObject.layer), LayerMask.LayerToName(hit2.collider.gameObject.layer),
                        LayerMask.LayerToName(hit3.collider.gameObject.layer), LayerMask.LayerToName(hit4.collider.gameObject.layer), place);
                }
                else
                {
                    //Debug.Log("OUTSIDE");
                    Debug.Log("Place: " + place +" No bitches?");
                }

                //Debug.Log("The layer of this place is " + ppp.collider.gameObject.layer);
            }
        }

        Vector3 myPlace = new Vector3(-13.5f, -7.5f, 0f);
        Debug.Log("I expect " + myPlace + " to be translated in -5, -3, 0: " + tileMap.WorldToCell(myPlace));
        */
        InitializeGraph1(3f, 3f, transform.GetComponent<Tilemap>(), new List<Tilemap>() { transform.parent.Find("Obstacle1").GetComponent<Tilemap>(), transform.parent.Find("Obstacle2").GetComponent<Tilemap>() });
    }




    private Dictionary<int, Dictionary<int, bool>> lakeAbstractMap = new Dictionary<int, Dictionary<int, bool>>();

    private void InitializeGraph1(float cellSizeX, float cellSizeY, Tilemap water, List<Tilemap> obstacles)
    {
        //List<Vector3> availablePlacesWater = new List<Vector3>();

        //I iterate over all the cells of the lake
        for (int x = water.cellBounds.xMin; x < water.cellBounds.xMax; x++)
        {
            for (int y = water.cellBounds.yMin; y < water.cellBounds.yMax; y++)
            {
                //I'm saving the cells that contain a water tile

                Vector3Int localPlace = (new Vector3Int(x, y, (int)water.transform.position.y));
                Debug.Log("LocalPlace is " + localPlace);
                Vector3 place = water.CellToWorld(localPlace) + new Vector3(cellSizeX / 2, cellSizeY / 2);

                if (lakeAbstractMap.ContainsKey(x) == false)
                {
                    lakeAbstractMap[x] = new Dictionary<int, bool>();
                }

                if (water.HasTile(localPlace))
                {

                    lakeAbstractMap[x][y] = true;

                    //DrawPoint(place.x, place.y);

                }
                else
                {
                    lakeAbstractMap[x][y] = false;
                }

            }
        }

        //now I have to remove the tiles in which there is an obstacle
        foreach (Tilemap tilemapObstacle in obstacles)
        {
            for (int x = tilemapObstacle.cellBounds.xMin; x < tilemapObstacle.cellBounds.xMax; x++)
            {
                for (int y = tilemapObstacle.cellBounds.yMin; y < tilemapObstacle.cellBounds.yMax; y++)
                {
                    Vector3Int localPlaceObs = (new Vector3Int(x, y, (int)tilemapObstacle.transform.position.y));
                    if (tilemapObstacle.HasTile(localPlaceObs))
                    {
                        Vector3 place = tilemapObstacle.CellToWorld(localPlaceObs) + new Vector3(cellSizeX / 2, cellSizeY / 2);
                        Vector3Int localPlaceWater = water.WorldToCell(place);
                        int xW = localPlaceWater.x;
                        int yW = localPlaceWater.y;
                        lakeAbstractMap[xW][yW] = false;
                    }
                }
            }
        }

        for (int x = water.cellBounds.xMin; x < water.cellBounds.xMax; x++)
        {
            for (int y = water.cellBounds.yMin; y < water.cellBounds.yMax; y++)
            {
                if(lakeAbstractMap[x][y] == true){
                    Vector3Int localPlace = (new Vector3Int(x, y, (int)water.transform.position.y));
                    Vector3 place = water.CellToWorld(localPlace) + new Vector3(cellSizeX / 2, cellSizeY / 2);
                    DrawPoint(place.x, place.y);
                }
            }
        }



    }


    private void DrawPoint(float x, float y)
    {
        Vector3 point = new Vector3(x, y, 0);
        float range = 1f;
        Ray rayNorth = new Ray(point, new Vector3(0, 1, 0));
        Ray raySouth = new Ray(point, new Vector3(0, -1, 0));
        Ray rayWest = new Ray(point, new Vector3(1, 0, 0));
        Ray rayEast = new Ray(point, new Vector3(-1, 0, 0));

        Debug.DrawRay(point, new Vector3(0, 1, 0) * range, Color.red, 10f, false);
        Debug.DrawRay(point, new Vector3(0, -1, 0) * range, Color.red, 10f, false);
        Debug.DrawRay(point, new Vector3(1, 0, 0) * range, Color.red, 10f, false);
        Debug.DrawRay(point, new Vector3(-1, 0, 0) * range, Color.red, 10f, false);
    }


    public static void hello()
    {
        Debug.Log("Hello");
    }
}
