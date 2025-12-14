using UnityEngine;

namespace SimpleFPS
{
	/// <summary>
	/// Simple script used to snap left hand to weapon (rifle) during Idle animation.
	/// </summary>
	public class HandSnap : MonoBehaviour
	{
		public float MaxSnapDistance = 0.1f;
		public Transform Handle;

		public void LateUpdate()
		{
			if (Handle == null)
				return;

			float sqrDistance = Vector3.SqrMagnitude(Handle.position - transform.position);
			if (sqrDistance > MaxSnapDistance * MaxSnapDistance)
				return; // Too far

			// Snap hand to handle
			transform.position = Handle.position;
		}
	}
}
