using System;
using UnityEngine;

namespace Util {
	[RequireComponent(typeof(Collider2D))]
	public class Collision2DHandler : MonoBehaviour {
		public event Action<Collision2D> OnCollisionEnterListener;
		public event Action<Collision2D> OnCollisionStayListener;
		public event Action<Collision2D> OnCollisionExitListener;
		public event Action<Collider2D> OnTriggerEnterListener;
		public event Action<Collider2D> OnTriggerStayListener;
		public event Action<Collider2D> OnTriggerExitListener;

		Collider2D MyCollider { get; set; }


		void Awake () {
			MyCollider = GetComponent<Collider2D>();
		}

		public void SetEnabled (bool isEnabled) {
			MyCollider.enabled = isEnabled;
		}

		void OnCollisionEnter2D (Collision2D collision) {
			OnCollisionEnterListener?.Invoke(collision);
		}

		void OnCollisionStay2D (Collision2D collision) {
			OnCollisionStayListener?.Invoke(collision);
		}

		void OnCollisionExit2D (Collision2D collision) {
			OnCollisionExitListener?.Invoke(collision);
		}

		void OnTriggerEnter2D (Collider2D collision) {
			OnTriggerEnterListener?.Invoke(collision);
		}

		void OnTriggerStay2D (Collider2D collision) {
			OnTriggerStayListener?.Invoke(collision);
		}

		void OnTriggerExit2D (Collider2D collision) {
			OnTriggerExitListener?.Invoke(collision);
		}
	}
}
