using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GraphLakeNamespace
{
	//Transition between two nodes
	public class GEdge
	{

		public GNode from;
		public GNode to;
		public float weight;

		// Default weight could also be 0, but 1 will give a better animation effect
		public GEdge(GNode from, GNode to, float weight = 1f)
		{
			this.from = from;
			this.to = to;
			this.weight = weight;
		}

	}
}