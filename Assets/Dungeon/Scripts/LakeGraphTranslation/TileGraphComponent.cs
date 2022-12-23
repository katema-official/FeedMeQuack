using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using LevelStageNamespace;

namespace GraphLakeNamespace {

    public class TileGraphComponent : MonoBehaviour
    {
        //this is just a component used to build the graph of the current lake. This component in particular gives a matrix representation
        //of the walkable tiles of this lake (true-false 2D matrix)

        //########################################################################################################################################################
        //################################################################### TILEMAP INFOS ######################################################################
        //########################################################################################################################################################
        private Tilemap _myTilemap;
        private Dictionary<int,Dictionary<int, bool>> _tileGraphBinary;
        private int _xMin;
        private int _xMax;
        private int _yMin;
        private int _yMax;
        private float _offsetX;
        private float _offsetY;

        //########################################################################################################################################################
        //#################################################################### GRAPH INFOS #######################################################################
        //########################################################################################################################################################
        private Graph _lakeGraph;


        void Start()
        {
            if (GameObject.Find("LevelStageManagerObject").GetComponent<LevelStageManagerComponent>().IsCurrentLakeCleared()) return;
            _myTilemap = transform.Find("TileGraph").GetComponent<Tilemap>();
            _offsetX = _myTilemap.cellSize.x / 2f;
            _offsetY = _myTilemap.cellSize.y / 2f;
            //Debug.LogFormat("{0} and {1}", offsetX, offsetY);

            _xMin = _myTilemap.cellBounds.xMin;
            _xMax = _myTilemap.cellBounds.xMax;
            _yMin = _myTilemap.cellBounds.yMin;
            _yMax = _myTilemap.cellBounds.yMax;

            _tileGraphBinary = new Dictionary<int, Dictionary<int, bool>>();
            Vector3 offsetVector = new Vector3(_offsetX, _offsetY, 0);

            LakeShopDescriptionComponent lakeShopDescriptionComponent = GameObject.Find("WholeLake").GetComponent<LakeShopDescriptionComponent>();
            //Debug.LogFormat("x and y: {0},{1} and {2},{3}", _myTilemap.cellBounds.xMin, _myTilemap.cellBounds.xMax, _myTilemap.cellBounds.yMin, _myTilemap.cellBounds.yMax);
            for (int x = _xMin; x < _xMax; x++)
            {
                for (int y = _yMin; y < _yMax; y++)
                {
                    Vector3Int localPlace = (new Vector3Int(x, y, (int)_myTilemap.transform.position.y));
                    Vector3 actualPlace = _myTilemap.CellToWorld(localPlace) + offsetVector;    //!!! IT'S VERY IMPORTANT TO NOT FORGET ABOUT THE OFFSET VECTOR
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


            //it's much likely that I can merge this for and the preceeding one together
            _lakeGraph = new Graph();
            for (int x = _xMin; x < _xMax; x++)
            {
                for (int y = _yMin; y < _yMax; y++)
                {
                    if (_tileGraphBinary[x][y])
                    {
                        //if in that position of the tileset there is a water spot, let's create a node
                        _lakeGraph.AddNode(new GNode(x, y));
                    }

                }
            }

            //now I need to connect the nodes. In this implementation (might change in the future), each node will be connected with at most 8 nodes,
            //the eight adjacent ones (it's like a chessboard
            foreach(GNode g in _lakeGraph.getNodes())
            {
                float x = g.x;
                float y = g.y;
                List<(float, float)> possibleAdjacentNodes = new List<(float, float)>() {
                    (x+1f, y),
                    (x-1f, y),
                    (x, y+1f),
                    (x, y-1f),
                    //(x+1f, y+1f),
                    //(x+1f, y-1f),
                    //(x-1f, y+1f),
                    //(x-1f, y-1f)
                };

                foreach((float,float) p in possibleAdjacentNodes)
                {
                    GNode h;
                    if((h = _lakeGraph.getNodeAtCoordinates(p.Item1, p.Item2)) != null)
                    {
                        float dist = _lakeGraph.distance(g.x, g.y, p.Item1, p.Item2);
                        _lakeGraph.AddEdge(new GEdge(g, h, dist));
                    }
                }
            }
            DrawGraph(_lakeGraph);

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


        private void DrawGraph(Graph graph)
        {
            Vector3 offsetVector = new Vector3(_offsetX, _offsetY);
            foreach(GNode node in graph.getNodes())
            {
                foreach(GEdge edge in graph.getConnections(node))
                {
                    DrawRayPointToPoint(edge);
                }
            }
        }

        private void DrawRayPointToPoint(GEdge edge)
        {
            Vector3 start = FromNodeToPosition(edge.from);
            Vector3 end = FromNodeToPosition(edge.to);
            Debug.DrawRay(start, end - start, Color.yellow, 5f, false);
        }

        private void DrawPath(List<Vector3> path)
        {
            for(int i = 0; i < path.Count - 1; i++)
            {
                Debug.DrawRay(path[i], path[i+1] - path[i], Color.red, 5f, false);
            }
        }


        //function that, given a node, converts its abstact coordinates into actual game-world coordinates
        private Vector3 FromNodeToPosition(GNode node)
        {
            Vector3 offsetVector = new Vector3(_offsetX, _offsetY);
            return _myTilemap.CellToWorld(new Vector3Int((int)node.x, (int)node.y, 0)) + offsetVector;
        }





        public List<Vector3> GetPathFromPointToPoint(Vector3 start, Vector3 end, CircleCollider2D circleCollider)
        {
            //to get the corresponding points on the graph, I find the closest points in the graph
            float minStartDistance = 100000f;
            float minEndDistance = 100000f;
            GNode nodeStart = null;
            GNode nodeEnd = null;
            foreach(GNode node in _lakeGraph.getNodes())
            {
                Vector3 translatedNode = FromNodeToPosition(node);
                float dStart = _lakeGraph.distance(start.x, start.y, translatedNode.x, translatedNode.y);
                if (nodeStart == null || dStart <= minStartDistance)
                {
                    nodeStart = node;
                    minStartDistance = dStart;
                }

                float dEnd = _lakeGraph.distance(end.x, end.y, translatedNode.x, translatedNode.y);
                if (nodeEnd == null || dEnd <= minEndDistance)
                {
                    nodeEnd = node;
                    minEndDistance = dEnd;
                }
            }

            //after we have found the starting node and the final node, we can find out the path from the first to the second
            GEdge[] path = AStarSolver.Solve(_lakeGraph, nodeStart, nodeEnd, AStarSolver.myHeuristics[(int)AStarSolver.Heuristics.Euclidean]);

            if(path == null) { return new List<Vector3>() { end }; }    //just go to the end, which is extremely near to you

            List<Vector3> allPoints = new List<Vector3>() { start };
            foreach(GEdge edge in path)
            {
                allPoints.Add(FromNodeToPosition(edge.from));
            }
            allPoints.Add(FromNodeToPosition(path[path.Length - 1].to));
            allPoints.Add(end);

            DrawPath(allPoints);


            //then, we need to smooth the path based on the collider of the entity that wants to move in that direction
            List<Vector3> necessaryPoints = new List<Vector3>();// { start };   //the starting point shouldn't be necessary: the caller is already there
            //so in the list of points it needs to traverse (that's what this function returns) there shouldn't be the start
            Vector3 currentPoint = start;
            Vector3 nextPoint;
            for(int i = 1; i < allPoints.Count; i++)
            {
                nextPoint = allPoints[i];

                float radius = circleCollider.radius;

                //set up the CircleCast for the next point
                Vector2 direction = new Vector2(nextPoint.x, nextPoint.y) - new Vector2(currentPoint.x, currentPoint.y);
                direction.Normalize();
                float distance = Vector2.Distance(new Vector2(nextPoint.x, nextPoint.y), new Vector2(currentPoint.x, currentPoint.y));
                RaycastHit2D hit = Physics2D.CircleCast(currentPoint, radius, direction, distance, LayerMask.GetMask("TerrainLayer"));

                if(hit.collider != null)
                {
                    //Debug.Log("TROVATO OSTACOLO");
                    necessaryPoints.Add(allPoints[i - 1]);
                    currentPoint = allPoints[i-1];

                    //do I see the final destination? If so, we can finish before
                    direction = new Vector2(end.x, end.y) - new Vector2(currentPoint.x, currentPoint.y);
                    direction.Normalize();
                    distance = Vector2.Distance(new Vector2(end.x, end.y), new Vector2(currentPoint.x, currentPoint.y));
                    hit = Physics2D.CircleCast(currentPoint, radius, direction, distance, LayerMask.GetMask("TerrainLayer"));
                    if (hit.collider == null) break;
                }
            }
            necessaryPoints.Add(end);

            List<Vector3> debugList = new List<Vector3>() { start };
            debugList.AddRange(necessaryPoints);

            DrawPath(debugList);

            return necessaryPoints;

        }






        public float GetOffsetX()
        {
            return _offsetX;
        }

        public float GetOffsetY()
        {
            return _offsetY;
        }


        /*
        public Vector3 SSSstart;
        public Vector3 SSSend;

        public Vector3 trueStart;
        public Vector3 trueEnd;
        


        void Update()
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                GEdge[] path = AStarSolver.Solve(_lakeGraph, _lakeGraph.getNodeAtCoordinates(SSSstart.x, SSSstart.y), _lakeGraph.getNodeAtCoordinates(SSSend.x, SSSend.y), AStarSolver.myHeuristics[(int)AStarSolver.Heuristics.Euclidean]);
                foreach (GEdge e in path)
                {
                    DrawRayPointToPoint(e);
                }
            }

            if (Input.GetKeyDown(KeyCode.G))
            {
                //trueStart = SSSstart * 3 + new Vector3(_offsetX, _offsetY, 0);
                //trueEnd = SSSend * 3 + new Vector3(_offsetX, _offsetY, 0);
                List<Vector3> p = GetPathFromPointToPoint(trueStart, trueEnd, GetComponent<CircleCollider2D>());
                Debug.Log("DONE");
                DrawPath(p);
            }

        }

        */


    }
}