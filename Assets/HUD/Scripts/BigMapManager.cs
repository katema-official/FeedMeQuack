using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

namespace HUDNamespace
{
    public class BigMapManager: MonoBehaviour
    {
        //metodo che di volta in volta ottiene le stanze adiacenti a quella attuale, e con queste nuove informazioni arricchisco di volta in volta la minimappa
        //che vedo il giocatore
        private int dimSize = 5;
        private int currX, currY, xDelta, yDelta;
        private int _shiftRow, _shiftCol;
        private bool _isNewLevel=true;
        [SerializeField] private int cellSize;
        [SerializeField] private int[,] _wholeMap;
        [SerializeField] private GameObject squarePrefab;
        public GameObject[,] mapTiles;
        
        /*
         * 0 se non compare, 1 grigia, 2 bianca, 3 se è la exit
         */

        public void StartNewLevel(){
            _isNewLevel = true;
            foreach(GameObject tileGO in mapTiles)
            {
                if(tileGO) Destroy(tileGO);
            }
        }
        /**
         * return initCol, finCol, initRow, finRow
         */
        private Tuple<int,int,int,int>  GetBorders(){
            int initCol=15, initRow=15, finCol=0, finRow=0;
            for (int row = 0; row < 15; row++){
                for (int col = 0; col < 15; col++){
                    int val = _wholeMap[col, row];
                    if (val != 0){
                        if (col < initCol) initCol = col;
                        if (col > finCol) finCol = col;
                        if (row < initRow) initRow = row;
                        if (row > finRow) finRow = row;
                    }
                }
            }
            if ( _isNewLevel)
                BuildNewMap(6, 8, 6, 8);
            else if (finCol - initCol >= dimSize || finRow - initRow >= dimSize)
                BuildNewMap(initCol, finCol, initRow, finRow);
            return Tuple.Create(initCol, finCol, initRow, finRow);
        }

        private void BuildNewMap(int initCol,int finCol,int initRow,int finRow){
            int diffCol = finCol - initCol, diffRow = finRow - initRow;
            mapTiles = new GameObject[diffRow + 1, diffCol + 1];
            cellSize = TileSizeFromDimensions(initCol, finCol, initRow, finRow);
            var position = gameObject.transform.position;
            float minimapX = position.x;
            float minimapY = position.y;
            foreach(GameObject tileGO in mapTiles)
                if(tileGO) Destroy(tileGO);
            for (int row = 0; row <= diffRow; row++){
                for (int col = 0; col <= diffCol; col++){
                    float xPos = (row) * cellSize + minimapX;
                    float yPos = (diffCol-col-1) * cellSize + minimapY;
                    Vector2 pos = new Vector2(xPos, yPos);
                    GameObject tile=Instantiate(squarePrefab, pos, Quaternion.identity);
                    tile.name = $"Tile {row},{col}";
                    tile.transform.parent = gameObject.transform;
                    Renderer outer = tile.GetComponentsInChildren<Renderer>()[0];
                    Renderer inner = tile.GetComponentsInChildren<Renderer>()[1];
                    outer.material.color= Color.clear;
                    inner.material.color= Color.clear;
                    //tile.transform.position = tile.transform.parent.position+ (Vector3)pos;
                    mapTiles[row,col] = tile;
                }
            }

            _isNewLevel = false;
        }

        private int TileSizeFromDimensions(int initCol,int finCol,int initRow,int finRow){
            int div = Math.Max(finCol - initCol,finRow-initRow);
            squarePrefab.transform.localScale = new Vector3((float)15*dimSize / div, (float) 15*dimSize / div, 1);
            return 15*dimSize / div;
        }

        public void DisplayBigMap(){
            _wholeMap = FindObjectOfType<MapManager>().GetWholeMap();
            PrintMap(_wholeMap);
            
            Tuple<int,int,int,int> tuple = GetBorders();
            int initCol=tuple.Item1, finCol=tuple.Item2, initRow=tuple.Item3, finRow=tuple.Item4;
            int diffCol = finCol - initCol, diffRow = finRow - initRow;
            cellSize = TileSizeFromDimensions(initCol, finCol, initRow, finRow);
            Debug.Log("Diff col/row: "+ diffCol+" "+ diffRow);
            for (int row = 0; row <= diffRow; row++){
                for (int col = 0; col <= diffCol; col++){
                    GameObject tile = mapTiles[row, col];
                    Renderer outer = tile.GetComponentsInChildren<Renderer>()[0];
                    Renderer inner = tile.GetComponentsInChildren<Renderer>()[1];
                    outer.material.color= Color.black;
                    int value = _wholeMap[col+initCol,row+initRow];
                    Debug.Log(tile.name+" value: "+value);
                    if (value == 0){
                        outer.material.color= Color.clear;
                        inner.material.color= Color.clear;
                    }
                    else if (value == 1){
                        inner.material.color= Color.gray;
                    }
                    else if (value == 2){
                        inner.material.color= Color.white;
                        Debug.Log("R: "+row+" C: "+col);
                    }
                    else if (value == 3){
                        inner.material.color= Color.green;
                    }
                    else if (value == -1){
                        inner.material.color= Color.yellow;
                    }
                }
            }
        }

        private void PrintMap(int[,] wholeMap){
            String str = "";
            for (int r = 0; r < wholeMap.GetLength(0); r++){
                for (int c = 0; c < wholeMap.GetLength(1); c++){
                    str += " " + wholeMap[r, c];
                    if(wholeMap[r,c]!=0) Debug.Log("DIVERSO DA 0!!!");
                }
                Debug.Log(str);
                str = "";
            }

            Debug.Log("/////////////////////////////////////////////////");
        }
    }
}