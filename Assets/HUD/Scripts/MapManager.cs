using UnityEngine;

namespace HUD
{
    public class MapManager: MonoBehaviour
    {
        private int dimSize = 8;
        [SerializeField] private int minimapX, minimapY, cellSize;
        private int [][] _map;
        [SerializeField] private GameObject squarePrefab;
        public GameObject[,] mapTiles;
        
        /*
         * 0 se non compare, 1 grigia, 2 bianca, 3 se è la exit
         */

        public void UpdateMap(int[][] newMap){
            _map = newMap;
            ChangeVisualization();
        }

        private void ChangeVisualization(){
            
        }

        private void Start(){
            squarePrefab.transform.localScale = new Vector3(cellSize, cellSize);
            mapTiles = new GameObject[dimSize, dimSize];
            for (int row = 0; row < dimSize; row++){
                for (int col = 0; col < dimSize; col++){
                    int xPos = minimapX + row * cellSize;
                    int yPos= minimapY + (dimSize-col-1) * cellSize;
                    Vector2 pos = new Vector2(xPos, yPos);
                    GameObject tile=Instantiate(squarePrefab, pos, Quaternion.identity);
                    tile.name = $"Tile {row},{col}";
                    Debug.Log(tile.name);
                    mapTiles[row,col] = tile;
                }
            }
        }
    }
}