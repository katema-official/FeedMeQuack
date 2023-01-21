using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

namespace HUDNamespace
{
    public class BigMapManagerV2: MonoBehaviour
    {
        //metodo che di volta in volta ottiene le stanze adiacenti a quella attuale, e con queste nuove informazioni arricchisco di volta in volta la minimappa
        //che vedo il giocatore
        private int tilesMatrixSize = 15;
        private int _wholeMapSize = 31;
        private int currX, currY, xDelta, yDelta;
        private int _shiftRow, _shiftCol;
        private float minimapX, minimapY;
        [SerializeField] private float cellSize;

        [SerializeField] private Camera _camera;
        private int [,] _map;
        [SerializeField] private int[,] _wholeMap;
        [SerializeField] private GameObject squarePrefab;
        public GameObject[,] mapTiles;
        
        [SerializeField] private GameObject _miniDuckPrefab;
        private GameObject _miniDuckGO;
        
        /*
         * 0 se non compare, 1 grigia, 2 bianca, 3 se è la exit
         */

        private void ChangeVisualization(){
            for (int row = 0; row < tilesMatrixSize; row++){
                for (int col = 0; col < tilesMatrixSize; col++){
                    int relativeX = xDelta + col;
                    int relativeY = yDelta + row;
                    GameObject tile = mapTiles[row, col];
                    Renderer outer = tile.GetComponentsInChildren<Renderer>()[0];
                    Renderer inner = tile.GetComponentsInChildren<Renderer>()[1];
                    //int value = _wholeMap[relativeX+_shiftRow, relativeY+_shiftCol];
                    int value = _wholeMap[relativeX, relativeY];
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
                        inner.material.color= Color.white;      //yellow
                        _miniDuckGO.transform.parent = outer.transform;
                        _miniDuckGO.transform.position = outer.transform.position;
                    }else if(value == 4)
                    {
                        outer.material.color = Color.black;
                        inner.material.color = Color.green;
                        _miniDuckGO.transform.parent = outer.transform;
                        _miniDuckGO.transform.position = outer.transform.position;
                    }
                }
            }

            AdjustCamera();
            //FindObjectOfType<BigMapManager>().DisplayBigMap();
        }

        private void AdjustCamera(){
            int centerIndex = _wholeMapSize/2;
            Tuple<int, int, int, int> tuple = GetBorders();
            int initCol=tuple.Item1, finCol=tuple.Item2, initRow=tuple.Item3, finRow=tuple.Item4;
            int diffCol = finCol - initCol, diffRow = finRow - initRow;

            var containerSize = gameObject.GetComponent<RectTransform>().rect;

            float cameraPosX = containerSize.height/2 + ((initRow+finRow)/2f-centerIndex)* cellSize;
            float cameraPosY = containerSize.width/2 - ((initCol+finCol)/2f-centerIndex)* cellSize;

            //Vector2 containerLeftBottomCornerPos = transform.position;

            //Vector3 newCameraPos = containerLeftBottomCornerPos + new Vector2(cameraPosX, cameraPosY);

            float cameraSizeX = (diffCol + 1) * cellSize;
            float cameraSizeY = (diffRow + 1) * cellSize;

            float size = Math.Max(cameraSizeX, cameraSizeY);

            if (diffCol >= 0 && diffRow >= 0){
                _camera.transform.localPosition = new Vector3(cameraPosX, cameraPosY, -1);
                //_camera.transform.position = new Vector3(cameraPosX, cameraPosY);

            }
            else _camera.transform.localPosition = new Vector3(0,0,-1);
            _camera.orthographicSize = size/3.90f;
        }

        private void Start(){
            SetCellSize();
            _shiftCol = 0; 
            _shiftRow = 0;
            int wholeMapSize = 31;
            var position = gameObject.transform.position;
            minimapX = position.x;
            minimapY = position.y;
            currX = tilesMatrixSize / 2;
            currY = currX;
            xDelta = wholeMapSize / 2 - currX;
            yDelta = xDelta;
            _map = new int[tilesMatrixSize,tilesMatrixSize];
            _wholeMap = new int[wholeMapSize, wholeMapSize];
            squarePrefab.transform.localScale = new Vector3(cellSize, cellSize, 1);
            mapTiles = new GameObject[tilesMatrixSize, tilesMatrixSize];
            for (int row = 0; row < tilesMatrixSize; row++){
                for (int col = 0; col < tilesMatrixSize; col++){
                    float xPos = minimapX + (row+0.5f) * cellSize;
                    float yPos = minimapY + (tilesMatrixSize-col-1+0.5f) * cellSize;
                    Vector2 pos = new Vector2(xPos, yPos);
                    GameObject tile=Instantiate(squarePrefab, pos, Quaternion.identity);
                    tile.name = $"Tile {row},{col}";
                    tile.transform.parent = gameObject.transform;
                    tile.transform.localPosition = new Vector3(cellSize * (row+0.5f), (tilesMatrixSize - col- 0.5f) * cellSize);
                    tile.transform.localScale = new Vector3(cellSize, cellSize);
                    mapTiles[row,col] = tile;
                    _map[row, col] = 0;
                }
            }
            for (int row = 0; row < wholeMapSize; row++)
                for (int col = 0; col < wholeMapSize; col++)
                    _wholeMap[row, col] = 0;
            
            _miniDuckGO = _miniDuckGO == null ? _miniDuckGO = Instantiate(_miniDuckPrefab) : _miniDuckGO;
            _miniDuckGO.transform.position = new Vector3(5000, 5000, 0);
            
            ChangeVisualization();
        }

        private void SetCellSize(){

            Vector3 size = gameObject.GetComponent<RectTransform>().rect.size;

            float fatherW = size.x/tilesMatrixSize;
            float fatherH = size.y/tilesMatrixSize;

            cellSize = Math.Min(fatherH, fatherW);
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
            //FindObjectOfType<BigMapManager>().StartNewLevel();
            _miniDuckGO = null;
            
            Start();
        }

        public int[,] GetWholeMap(){
            return _wholeMap;
        }

        private void OnEnable(){
            
        }
        
        private Tuple<int,int,int,int>  GetBorders(){
            int initCol=_wholeMapSize, initRow=_wholeMapSize, finCol=0, finRow=0;
            for (int row = 0; row < _wholeMapSize; row++){
                for (int col = 0; col < _wholeMapSize; col++){
                    int val = _wholeMap[col, row];
                    if (val != 0){
                        if (col < initCol) initCol = col;
                        if (col > finCol) finCol = col;
                        if (row < initRow) initRow = row;
                        if (row > finRow) finRow = row;
                    }
                }
            }
            int diffCol = finCol - initCol, diffRow = finRow - initRow;

            return Tuple.Create(initCol, finCol, initRow, finRow);
        }
    }
}