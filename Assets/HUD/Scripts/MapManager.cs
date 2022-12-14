using UnityEngine;

namespace HUD
{
    public class MapManager: MonoBehaviour
    {
        //metodo che di volta in volta ottiene le stanze adiacenti a quella attuale, e con queste nuove informazioni arricchisco di volta in volta la minimappa
        //che vedo il giocatore
        private int dimSize = 5;
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
                    if (_map[row, col] == 0){
                        //todo: assign color
                    }
                }
            }
        }

        private void Start(){
            var position = gameObject.transform.position;
            minimapX = position.x;
            minimapY = position.y;
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
                }
            }
        }
    }
}