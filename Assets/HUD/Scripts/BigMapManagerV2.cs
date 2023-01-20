﻿using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

namespace HUDNamespace
{
    public class BigMapManagerV2: MonoBehaviour
    {
        //metodo che di volta in volta ottiene le stanze adiacenti a quella attuale, e con queste nuove informazioni arricchisco di volta in volta la minimappa
        //che vedo il giocatore
        private int dimSize = 9;
        private int currX, currY, xDelta, yDelta;
        private int _shiftRow, _shiftCol;
        private float minimapX, minimapY;
        [SerializeField] private float cellSize;
        private int [,] _map;
        [SerializeField] private int[,] _wholeMap;
        [SerializeField] private GameObject squarePrefab;
        public GameObject[,] mapTiles;
        
        /*
         * 0 se non compare, 1 grigia, 2 bianca, 3 se è la exit
         */

        private void ChangeVisualization(){
            for (int row = 0; row < dimSize; row++){
                for (int col = 0; col < dimSize; col++){
                    int relativeX = xDelta + col;
                    int relativeY = yDelta + row;
                    GameObject tile = mapTiles[row, col];
                    Renderer outer = tile.GetComponentsInChildren<Renderer>()[0];
                    Renderer inner = tile.GetComponentsInChildren<Renderer>()[1];
                    int value = _wholeMap[relativeX+_shiftRow, relativeY+_shiftCol];
                    if (value == 0){
                        outer.material.color= Color.clear;
                        inner.material.color= Color.clear;
                    }
                    else if (value == 1){
                        outer.material.color= Color.black;
                        inner.material.color= Color.gray;
                    }
                    else if (value == 2){
                        outer.material.color= Color.black;
                        inner.material.color= Color.white;
                    }
                    else if (value == 3){
                        outer.material.color= Color.black;
                        inner.material.color= Color.green;
                    }
                    else if (value == -1){
                        outer.material.color= Color.black;
                        inner.material.color= Color.yellow;
                    }
                }
            }
            //FindObjectOfType<BigMapManager>().DisplayBigMap();
        }

        private void Start(){
            _shiftCol = 0; 
            _shiftRow = 0;
            int wholeMapSize = 15;
            var position = gameObject.transform.position;
            minimapX = position.x;
            minimapY = position.y;
            currX = dimSize / 2;
            currY = currX;
            xDelta = wholeMapSize / 2 - currX;
            yDelta = xDelta;
            _map = new int[dimSize,dimSize];
            _wholeMap = new int[wholeMapSize, wholeMapSize];
            squarePrefab.transform.localScale = new Vector3(cellSize, cellSize, 1);
            mapTiles = new GameObject[dimSize, dimSize];
            for (int row = 0; row < dimSize; row++){
                for (int col = 0; col < dimSize; col++){
                    float xPos = minimapX + (row - 0.5f) * cellSize;
                    float yPos = minimapY + (dimSize-col-1 - 0.5f) * cellSize;
                    Vector2 pos = new Vector2(xPos, yPos);
                    GameObject tile=Instantiate(squarePrefab, pos, Quaternion.identity);
                    tile.name = $"Tile {row},{col}";
                    tile.transform.parent = gameObject.transform;
                    mapTiles[row,col] = tile;
                    _map[row, col] = 0;
                }
            }
            for (int row = 0; row < wholeMapSize; row++)
            for (int col = 0; col < wholeMapSize; col++)
                _wholeMap[row, col] = 0;
            ChangeVisualization();
        }

        //0: no room exists there
        //1: the room exists but hasn't been visited yet
        //2: the room exists and has been cleared
        //3: the room is the exit
        public void UpdateMinimapAfterRiver(MapManager.CardinalDirection dir, int nord, int sud, int est, int ovest){
            ChangePos(dir);
            int x = xDelta + currX;
            int y = yDelta + currY;
            _wholeMap[y,x]=-1;
            _wholeMap[y+1,x]=sud;
            _wholeMap[y-1,x]=nord;
            _wholeMap[y,x+1]=est;
            _wholeMap[y,x-1]=ovest;
            ChangeVisualization();
        }

        private void ChangePos(MapManager.CardinalDirection dir){
            switch (dir){
                case MapManager.CardinalDirection.nord:
                    currY--;
                    _shiftRow--;
                    break;
                case MapManager.CardinalDirection.sud:
                    currY++;
                    _shiftRow++;
                    break;
                case MapManager.CardinalDirection.est:
                    currX++;
                    _shiftCol++;
                    break;
                case MapManager.CardinalDirection.ovest:
                    currX--;
                    _shiftCol--;
                    break;
                case MapManager.CardinalDirection.none:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(dir), dir, null);
            }
        }

        public void StartNewLevel(){
            foreach(GameObject tileGO in mapTiles)
                if(tileGO) Destroy(tileGO);
            Start();
            //FindObjectOfType<BigMapManager>().StartNewLevel();
        }

        public int[,] GetWholeMap(){
            return _wholeMap;
        }
    }
}