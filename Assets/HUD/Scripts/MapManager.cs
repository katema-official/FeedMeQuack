using System;
using UnityEngine;

namespace HUD
{
    public class MapManager: MonoBehaviour
    {
        //metodo che di volta in volta ottiene le stanze adiacenti a quella attuale, e con queste nuove informazioni arricchisco di volta in volta la minimappa
        //che vedo il giocatore
        private int dimSize = 5;
        private int currX, currY;
        private float minimapX, minimapY;
        [SerializeField] private int cellSize;
        private int [,] _map;
        [SerializeField] private GameObject squarePrefab;
        public GameObject[,] mapTiles;
        
        /*
         * 0 se non compare, 1 grigia, 2 bianca, 3 se è la exit
         */

        public void UpdateMap(int[,] newMap){
            _map = newMap;
            ChangeVisualization();
        }

        private void ChangeVisualization(){
            for (int row = 0; row < dimSize; row++){
                for (int col = 0; col < dimSize; col++){
                    GameObject tile = mapTiles[row, col];
                    Renderer outer = tile.GetComponentsInChildren<Renderer>()[0];
                    Renderer inner = tile.GetComponentsInChildren<Renderer>()[1];
                    if (_map[row, col] == 0){
                        outer.material.color= Color.clear;
                        inner.material.color= Color.clear;
                    }
                    else if (_map[row, col] == 1){
                        outer.material.color= Color.black;
                        inner.material.color= Color.gray;
                    }
                    else if (_map[row, col] == 2){
                        outer.material.color= Color.black;
                        inner.material.color= Color.white;
                    }
                    else if (_map[row, col] == 3){
                        outer.material.color= Color.black;
                        inner.material.color= Color.green;
                    }
                    else if (_map[row, col] == -1){
                        outer.material.color= Color.black;
                        inner.material.color= Color.yellow;
                    }
                }
            }
        }

        private void Start(){
            var position = gameObject.transform.position;
            minimapX = position.x;
            minimapY = position.y;
            currX = dimSize / 2;
            currY = currX;
            _map = new int[dimSize,dimSize];
            squarePrefab.transform.localScale = new Vector3(cellSize, cellSize, 1);
            mapTiles = new GameObject[dimSize, dimSize];
            for (int row = 0; row < dimSize; row++){
                for (int col = 0; col < dimSize; col++){
                    float xPos = minimapX + row * cellSize;
                    float yPos= minimapY + (dimSize-col-1) * cellSize;
                    Vector2 pos = new Vector2(xPos, yPos);
                    GameObject tile=Instantiate(squarePrefab, pos, Quaternion.identity);
                    tile.name = $"Tile {row},{col}";
                    tile.transform.parent = gameObject.transform;
                    Debug.Log(tile.name);
                    mapTiles[row,col] = tile;
                    _map[row, col] = 0;
                }
            }
            ChangeVisualization();
        }

        public void UpdateMinimapAfterRiver(CardinalDirection dir, int nord, int sud, int est, int ovest){
            changePos(dir);
            _map[currX + 1, currY] = est;
            _map[currX - 1, currY] = ovest;
            _map[currX, currY + 1] = nord;
            _map[currX, currY - 1] = sud;
            _map[currX, currY] = -1;
            ChangeVisualization();
        }

        private void changePos(CardinalDirection dir){
            switch (dir){
                case CardinalDirection.nord:
                    currY++;
                    break;
                case CardinalDirection.sud:
                    currY--;
                    break;
                case CardinalDirection.est:
                    currX++;
                    break;
                case CardinalDirection.overs:
                    currY--;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(dir), dir, null);
            }
        }

        public enum CardinalDirection{
            nord,
            sud,
            est,
            overs,
        }
    }
}