using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MainGame {
	[RequireComponent(typeof(Collider2D))]
	[RequireComponent(typeof(Rigidbody2D))]
	public class CharacterBone : MonoBehaviour {
		Collider2D MyCollider { get; set; }
		Rigidbody2D MyRigidbody { get; set; }
		Joint2D MyJoint { get; set; }


		void Awake () {
			MyCollider = GetComponent<Collider2D>();
			MyRigidbody = GetComponent<Rigidbody2D>();
			MyJoint = GetComponent<Joint2D>();
		}

		public void ActivatePhysics () {
			MyCollider.enabled = true;
			MyRigidbody.simulated = true;
			MyJoint.enabled = true;
		}

		public void DeactivatePhysics () {
			MyCollider.enabled = false;
			MyRigidbody.simulated = false;
			MyJoint.enabled = false;
		}

		public void DetachJoint () {
			MyJoint.enabled = false;
		}
	}
}
