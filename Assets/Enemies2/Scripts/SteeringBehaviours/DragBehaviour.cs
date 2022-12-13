using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SteeringBehaviourNamespace
{
	public class DragBehaviour : MovementBehaviour
	{

		// how long does it take to stop moving by dragging
		public float linearDrag = 0f;
		public float angularDrag = 0f;

		public override Vector3 GetAcceleration(MovementStatus status)
		{
			return Vector3.zero;

			Vector3 res = -(status.movementDirection.normalized * status.linearSpeed / linearDrag)
				   - ((Quaternion.Euler(0f, 90f, 0f) * status.movementDirection.normalized) * status.angularSpeed / angularDrag);
			//if (Mathf.Abs(res.magnitude) <= 0.1f) return Vector3.zero;
			return res;
		}
	}
}