using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GraphLakeNamespace
{


	public delegate float HeuristicFunction(GNode from, GNode to);

	//CREDITS: Dario Maggiorini & Davide Gadia, University of Milan (Italy), Artificial Intelligence for Videogames
	public class AStarSolver
	{

		// two set of nodes (1)

		public static List<GNode> visited;
		public static List<GNode> unvisited;

		// data structures to extend nodes (2)

		protected struct GNodeExtension
		{
			public float distance;
			public float estimate;
			public GEdge predecessor;
		}

		protected static Dictionary<GNode, GNodeExtension> status;

		public static GEdge[] Solve(Graph g, GNode start, GNode goal, HeuristicFunction heuristic)
		{
			//null always means, in this implementation: there is no path from start to goal (either they coincide, so the caller
			//is already at its destination, or the graph is partitioned)
			if (start == goal) return null;

			// setup sets (1)
			visited = new List<GNode>();
			unvisited = new List<GNode>(g.getNodes());

			// set all node tentative distance (2)
			status = new Dictionary<GNode, GNodeExtension>();
			foreach (GNode n in unvisited)
			{
				GNodeExtension ne = new GNodeExtension();
				ne.distance = (n == start ? 0f : float.MaxValue); // infinite
				ne.estimate = (n == start ? heuristic(start, goal) : float.MaxValue);
				status[n] = ne;
			}

			// iterate until goal is reached with an optimal path (6)
			while (!CheckSearchComplete(goal, unvisited))
			{
				// select the current node (3)
				GNode current = GetNextNode();

				if (status[current].distance == float.MaxValue) break; // graph is partitioned

				// assign weight and predecessor to all neighbors (4)
				foreach (GEdge e in g.getConnections(current))
				{
					if (status[current].distance + e.weight < status[e.to].distance)
					{
						GNodeExtension ne = new GNodeExtension();
						ne.distance = status[current].distance + e.weight;
						ne.estimate = ne.distance + heuristic(e.to, goal);
						ne.predecessor = e;
						status[e.to] = ne;
						// unlike Dijkstra's, we can now discover better paths
						if (visited.Contains(e.to))
						{
							unvisited.Add(e.to);
							visited.Remove(e.to);
						}
					}
				}
				// mark current node as visited (5)
				visited.Add(current);
				unvisited.Remove(current);
			}

			if (status[goal].distance == float.MaxValue) return null; // goal is unreachable

			// walk back and build the shortest path (7)
			List<GEdge> result = new List<GEdge>();
			GNode walker = goal;

			while (walker != start)
			{
				result.Add(status[walker].predecessor);
				walker = status[walker].predecessor.from;
			}
			result.Reverse();
			return result.ToArray();
		}

		// iterate on the unvisited set and get the lowest weight
		protected static GNode GetNextNode()
		{
			GNode candidate = null;
			float cDistance = float.MaxValue;
			foreach (GNode n in unvisited)
			{
				if (candidate == null || cDistance > status[n].estimate)
				{
					candidate = n;
					cDistance = status[n].estimate;
				}
			}
			return candidate;
		}

		// chek if the goal has been reached in a satisfactory way
		protected static bool CheckSearchComplete(GNode goal, List<GNode> nodeList)
		{
			// check if we reached the goal
			if (status[goal].distance == float.MaxValue) return false;
			// check if the first hit is ok 
			if (stopAtFirstHit) return true;
			// check if all nodes in list have loger or same paths 
			foreach (GNode n in nodeList)
			{
				if (status[n].distance < status[goal].distance) return false;
			}
			return true;
		}






		//used by AStarSolver
		public static bool stopAtFirstHit = false;
		public bool _stopAtFirstHit = false;

		public enum Heuristics { Euclidean, Manhattan, Bisector, FullBisector, Zero };
		public static HeuristicFunction[] myHeuristics = { EuclideanEstimator, ManhattanEstimator, BisectorEstimator,
												 FullBisectorEstimator, ZeroEstimator };
		public Heuristics heuristicToUse = Heuristics.Euclidean;

		protected static float EuclideanEstimator(GNode from, GNode to)
		{
			return (new Vector3(from.x, from.y, 0) - new Vector3(to.x, to.y, 0)).magnitude;
		}

		protected static float ManhattanEstimator(GNode from, GNode to)
		{
			return (
					Mathf.Abs(from.x - to.x) +
					Mathf.Abs(from.y - to.y)
				);
		}

		protected static float BisectorEstimator(GNode from, GNode to)
		{
			Ray r = new Ray(Vector3.zero, new Vector3(to.x, to.y, 0));
			return Vector3.Cross(r.direction, new Vector3(from.x, from.y, 0) - r.origin).magnitude;
		}

		protected static float FullBisectorEstimator(GNode from, GNode to)
		{
			Ray r = new Ray(Vector3.zero, new Vector3(to.x, to.y, 0));
			Vector3 toBisector = Vector3.Cross(r.direction, new Vector3(from.x, from.y, 0) - r.origin);
			return toBisector.magnitude + (new Vector3(to.x, to.y, 0) - (new Vector3(from.x, from.y, 0) + toBisector)).magnitude;
		}

		protected static float ZeroEstimator(GNode from, GNode to) { return 0f; }



	}

}