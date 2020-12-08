using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Util {
	public class RagdollUtility : MonoBehaviour {
		[SerializeField] string targetTagName;
		[SerializeField] float maxAngularVelocity = 150;
		[SerializeField] Vector3 gravity = new Vector3(0, -1, 0);

		List<Collider> TargetColliders = new List<Collider>();
		List<Rigidbody> TargetRigidbodies = new List<Rigidbody>();
		bool IsActive { get; set; }


		void FixedUpdate () {
			if (!IsActive) return;

			foreach (var rigid in TargetRigidbodies) {
				rigid.AddForce(gravity, ForceMode.Force);
			}
		}

		public void Setup () {
			TargetColliders.Clear();
			TargetRigidbodies.Clear();

			var childColliders = GetComponentsInChildren<Collider>();
			foreach (var c in childColliders) {
				if (c.gameObject == gameObject) continue;

				c.isTrigger = false;
				TargetColliders.Add(c);
			}

			var childRigidbodies = GetComponentsInChildren<Rigidbody>();
			foreach (var r in childRigidbodies) {
				if (r.tag == targetTagName) {
					if (r.gameObject == gameObject) continue;

					r.maxAngularVelocity = maxAngularVelocity;
					TargetRigidbodies.Add(r);
				}
				//r.useGravity = false;
			}
		}

		public void Activate () {
			foreach (var col in TargetColliders) {
				col.isTrigger = false;
			}

			foreach (var rigid in TargetRigidbodies) {
				rigid.isKinematic = false;
			}
			IsActive = true;
		}

		public void Deactivate () {
			foreach (var col in TargetColliders) {
				col.isTrigger = true;
			}

			foreach (var rigid in TargetRigidbodies) {
				rigid.isKinematic = true;
			}
			IsActive = false;
		}

		public void AddForce (Vector3 direction, Vector3 torque) {
			foreach (var rigid in TargetRigidbodies) {
				rigid.AddForce(direction, ForceMode.Impulse);
				rigid.AddTorque(torque, ForceMode.Impulse);
			}
		}
	}
}
