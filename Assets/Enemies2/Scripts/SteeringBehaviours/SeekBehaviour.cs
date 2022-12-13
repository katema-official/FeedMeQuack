using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SteeringBehaviourNamespace
{
	public class SeekBehaviour : MovementBehaviour
	{

		public Vector3 Destination;
		public bool IsDestinationValid = false;

		public float Acceleration = 0f;
		public float Steer = 0f;
		public float Deceleration = 0f;

		public float BrakeAt = 0f;
		public float StopAt = 0f;

		public override Vector3 GetAcceleration(MovementStatus status)
		{


			if (IsDestinationValid)
			{
				Vector3 toDestination = (Destination - transform.position);

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