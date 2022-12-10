using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace LevelStageNamespace {

    public class TileGraphComponent : MonoBehaviour
    {
        //this is just a component used to build the graph of the current lake. This component in particular gives a matrix representation
        //of the walkable tiles of this lake (true-false 2D matrix)

        private Tilemap _myTilemap;
        private Dictionary<int,Dictionary<int, bool>> _tileGraphBinary;
        private int _xMin;
        private int _xMax;
        private int _yMin;
        private int _yMax;

        void Start()
        {
            _myTilemap = transform.Find("TileGraph").GetComponent<Tilemap>();
            float offsetX = _myTilemap.cellSize.x / 2f;
            float offsetY = _myTilemap.cellSize.y / 2f;
            //Debug.LogFormat("{0} and {1}", offsetX, offsetY);

            _xMin = _myTilemap.cellBounds.xMin;
            _xMax = _myTilemap.cellBounds.xMax;
            _yMin = _myTilemap.cellBounds.yMin;
            _yMax = _myTilemap.cellBounds.yMax;

            _tileGraphBinary = new Dictionary<int, Dictionary<int, bool>>();
            Vector3 offsetVector = new Vector3(offsetX, offsetY, 0);

            LakeShopDescriptionComponent lakeShopDescriptionComponent = GameObject.Find("WholeLake").GetComponent<LakeShopDescriptionComponent>();
            //Debug.LogFormat("x and y: {0},{1} and {2},{3}", _myTilemap.cellBounds.xMin, _myTilemap.cellBounds.xMax, _myTilemap.cellBounds.yMin, _myTilemap.cellBounds.yMax);
            for (int x = _xMin; x < _xMax; x++)
            {
                for (int y = _yMin; y < _yMax; y++)
                {
                    Vector3Int localPlace = (new Vector3Int(x, y, (int)_myTilemap.transform.position.y));
                    Vector3 actualPlace = _myTilemap.CellToWorld(localPlace) + offsetVector;
                    bool placeable = lakeShopDescriptionComponent.Contains(actualPlace);
                    if (_tileGraphBinary.ContainsKey(x) == false)
                    {
                        _tileGraphBinary[x] = new Dictionary<int, bool>();
                    }
                    _tileGraphBinary[x][y] = placeable;

                }
            }

            /*
            for (int i = _xMin; i < _xMax; i++)
            {
                string s = "";
                for (int j = _yMin; j < _yMax; j++)
                {
                    //s += (_tileGraphBinary[i][j] ? 1 : 0);
                    if (_tileGraphBinary[i][j]) DrawPoint(_myTilemap.CellToWorld(new Vector3Int(i, j, 0)) + offsetVector);

                }
                //Debug.Log(s);
            }
            */

        }


        



        private void DrawPoint(Vector3 point)
        {
            float range = 1f;

            Ray rayNorth = new Ray(point, new Vector3(0, 1, 0));
            Ray raySouth = new Ray(point, new Vector3(0, -1, 0));
            Ray rayWest = new Ray(point, new Vector3(1, 0, 0));
            Ray rayEast = new Ray(point, new Vector3(-1, 0, 0));

            Debug.DrawRay(point, new Vector3(0, 1, 0) * range, Color.red, 3f, false);
            Debug.DrawRay(point, new Vector3(0, -1, 0) * range, Color.red, 3f, false);
            Debug.DrawRay(point, new Vector3(1, 0, 0) * range, Color.red, 3f, false);
            Debug.DrawRay(point, new Vector3(-1, 0, 0) * range, Color.red, 3f, false);
        }


    }
}