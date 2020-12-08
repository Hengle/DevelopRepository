using System;
using UnityEngine;

namespace Util {
	[RequireComponent(typeof(Collider))]
	public class Collision3DHandler : MonoBehaviour {
		public event Action<Collision> OnCollisionEnterListener;
		public event Action<Collision> OnCollisionStayListener;
		public event Action<Collision> OnCollisionExitListener;
		public event Action<Collider> OnTriggerEnterListener;
		public event Action<Collider> OnTriggerStayListener;
		public event Action<Collider> OnTriggerExitListener;

		Collider MyCollider { get; set; }


		void Awake () {
			MyCollider = GetComponent<Collider>();
		}

		public void SetEnabled (bool isEnabled) {
			MyCollider.enabled = isEnabled;
		}

		void OnCollisionEnter (Collision collision) {
			OnCollisionEnterListener?.Invoke(collision);
		}

		void OnCollisionStay (Collision collision) {
			OnCollisionStayListener?.Invoke(collision);
		}

		void OnCollisionExit (Collision collision) {
			OnCollisionExitListener?.Invoke(collision);
		}

		void OnTriggerEnter (Collider collision) {
			OnTriggerEnterListener?.Invoke(collision);
		}

		void OnTriggerStay (Collider collision) {
			OnTriggerStayListener?.Invoke(collision);
		}

		void OnTriggerExit (Collider collision) {
			OnTriggerExitListener?.Invoke(collision);
		}
	}
}
