using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SteeringBehaviourNamespace
{

	public class DDelegatedSteering : MonoBehaviour
	{
		public float minLinearSpeed = 0f;
		public float maxLinearSpeed = 5f;
		public float maxAngularSpeed = 75f;

		private MovementStatus status;

		private void Start()
		{
			status = new MovementStatus();
		}

		public MovementStatus GetMovementStatus()
        {
			return status;
        }

		void FixedUpdate()
		{

			status.movementDirection = transform.right;	//transform.right and not transform.forward since we are in 2D

			// Contact all behaviours and build a list of directions
			List<Vector3> components = new List<Vector3>();
			foreach (MovementBehaviour mb in GetComponents<MovementBehaviour>())
				components.Add(mb.GetAcceleration(status));

			// Blend the list to obtain a single acceleration to apply
			Vector3 blendedAcceleration = Blender.Blend(components);

			// if we have an acceleration, apply it
			if (blendedAcceleration.magnitude != 0f)
			{
				Driver.Steer(GetComponent<Rigidbody2D>(), status, blendedAcceleration,
							  minLinearSpeed, maxLinearSpeed, maxAngularSpeed);
			}
		}

		private void OnDrawGizmos()
		{
			if (status != null)
			{
				UnityEditor.Handles.Label(transform.position + 2f * transform.up, status.linearSpeed.ToString() + "\n" + status.angularSpeed.ToString());
			}
		}
	}
}