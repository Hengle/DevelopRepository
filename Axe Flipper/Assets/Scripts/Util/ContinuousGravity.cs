using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Util {
	[RequireComponent(typeof(Rigidbody))]
	public class ContinuousGravity : MonoBehaviour {
		[SerializeField] Vector3 force;
		[SerializeField] bool isAutoExecute = true;

		Rigidbody MyRigidbody { get; set; }
		bool IsActive { get; set; }

		void Start () {
			MyRigidbody = GetComponent<Rigidbody>();
			MyRigidbody.useGravity = false;

			IsActive = isAutoExecute;
		}

		void FixedUpdate () {
			if (!IsActive) return;

			MyRigidbody.AddForce(force, ForceMode.Force);
		}

		public void Apply () {
			if (isAutoExecute) return;

			IsActive = true;
		}
	}
}
