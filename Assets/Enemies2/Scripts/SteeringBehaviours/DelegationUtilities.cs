using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SteeringBehaviourNamespace
{


	public class MovementStatus
	{
		public Vector3 movementDirection;
		public float linearSpeed;
		public float angularSpeed;
	}

	// To be extended by all movement behaviours
	public abstract class MovementBehaviour : MonoBehaviour
	{
		public abstract Vector3 GetAcceleration(MovementStatus status);
	}

	public class Blender
	{
		public static Vector3 Blend(List<Vector3> vl)
		{
			Vector3 result = Vector3.zero;
			foreach (Vector3 v in vl) result += v;
			return result;
		}
	}

	// The steer function is the same as the FixedUpdate of DGripSteering
	public class Driver
	{

		public static void Steer(Rigidbody2D body, MovementStatus status, Vector3 acceleration,
											float minV, float maxV, float maxSigma)
		{


			Vector3 tangentComponent = Vector3.Project(acceleration, status.movementDirection);
			Vector3 normalComponent = acceleration - tangentComponent;

			float tangentAcc = tangentComponent.magnitude * Vector3.Dot(tangentComponent.normalized, status.movementDirection);
			Vector3 right = Quaternion.Euler(0f, 0f, 90f) * status.movementDirection.normalized;
			float rotationAcc = normalComponent.magnitude * Vector3.Dot(normalComponent.normalized, right) * 360f;

			float t = Time.deltaTime;

			float tangentDelta = status.linearSpeed * t + 0.5f * tangentAcc * t * t;
			float rotationDelta = status.angularSpeed * t + 0.5f * rotationAcc * t * t;

			status.linearSpeed += tangentAcc * t;
			status.angularSpeed += rotationAcc * t;

			status.linearSpeed = Mathf.Clamp(status.linearSpeed, minV, maxV);
			status.angularSpeed = Mathf.Clamp(status.angularSpeed, -maxSigma, maxSigma);

			//OK THIS STOPS IT, but fine-tune the values
			//float dist = Mathf.Abs((body.gameObject.transform.position - body.gameObject.GetComponent<SeekBehaviour>().destination.position).magnitude);
			//if (dist <= body.gameObject.GetComponent<SeekBehaviour>().stopAt) return;
			//body.gameObject.GetComponent<SeekBehaviour>().brakeAt = Mathf.Pow(status.linearSpeed, 2) / (2*body.gameObject.GetComponent<SeekBehaviour>().brake);

			if (Mathf.Abs(status.linearSpeed) >= 0.1f)
			{

				body.MovePosition(body.position + new Vector2(status.movementDirection.x, status.movementDirection.y) * tangentDelta);
			}
			//body.MoveRotation(body.rotation * Quaternion.Euler(0f, 0f, rotationDelta));
			//body.MoveRotation(Quaternion.AngleAxis(rotationDelta, Vector3.forward));
			body.MoveRotation(body.rotation + rotationDelta);
		}
	}

}