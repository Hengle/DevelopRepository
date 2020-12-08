using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Util {
	[RequireComponent(typeof(Collision2D))]
	[RequireComponent(typeof(Rigidbody2D))]
	public class AreaTrigger2DHandler : MonoBehaviour {
		[SerializeField] List<Collider2D> ignoreColliders;

		Dictionary<GameObject, bool> IsTriggeredObjects = new Dictionary<GameObject, bool>();
		Collider2D MyCollider { get; set; }

		public event Action<bool> OnTrigger;


		void Awake () {
			MyCollider = GetComponent<Collider2D>();
		}

		void OnTriggerEnter2D (Collider2D collision) {
			if (ignoreColliders.Contains(collision)) return;

			OnTrigger?.Invoke(true);
			if (IsTriggeredObjects.ContainsKey(collision.gameObject)) {
				IsTriggeredObjects[collision.gameObject] = true;
			} else {
				IsTriggeredObjects.Add(collision.gameObject, true);
			}
		}

		void OnTriggerExit2D (Collider2D collision) {
			if (IsTriggeredObjects.ContainsKey(collision.gameObject)) {
				IsTriggeredObjects[collision.gameObject] = false;
			} else {
				IsTriggeredObjects.Add(collision.gameObject, false);
			}

			if (!IsTriggeredObjects.ContainsValue(true)) {
				OnTrigger?.Invoke(false);
			}
		}

		public void SetEnabled (bool isEnabled) {
			MyCollider.enabled = isEnabled;
		}
	}
}
