using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GraphLakeNamespace {
	//A node of the graph
	public class GNode
	{

		//the coordinates (in the actual labyrinth) of this node
		public float x;
		public float y;

		public GNode(float x, float y)
		{
			this.x = x;
			this.y = y;
		}
	}
}