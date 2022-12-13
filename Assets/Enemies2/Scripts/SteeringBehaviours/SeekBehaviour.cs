using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SteeringBehaviourNamespace
{
	public class SeekBehaviour : MovementBehaviour
	{

		public Transform Destination;

		public float Acceleration = 3f;
		public float Steer = 30f;
		public float Deceleration = 20f;

		public float BrakeAt = 5f;
		public float StopAt = 0.01f;

		public override Vector3 GetAcceleration(MovementStatus status)
		{


			if (Destination != null)
			{
				Vector3 toDestination = (Destination.position - transform.position);

				if (toDestination.magnitude > StopAt)
				{
					Vector3 tangentComponent = Vector3.Project(toDestination.normalized, status.movementDirection);
					Vector3 normalComponent = (toDestination.normalized - tangentComponent);
					return (tangentComponent * (toDestination.magnitude > BrakeAt ? Acceleration : -Deceleration)) + (normalComponent * Steer);
				}
				else
				{
					return Vector3.zero;
				}
			}
			else
			{
				return Vector3.zero;
			}
		}
	}

}