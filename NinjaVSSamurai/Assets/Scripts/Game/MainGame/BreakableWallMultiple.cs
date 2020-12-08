using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MainGame {
	[RequireComponent(typeof(Collider2D))]
	[RequireComponent(typeof(Rigidbody2D))]
	public class BreakableWallMultiple : MonoBehaviour, IBreakable {

		List<BreakableWall> BreakableObjects { get; set; } = new List<BreakableWall>();
		Collider2D MyCollider { get; set; }
		Rigidbody2D MyRigidbody { get; set; }


		void Awake () {
			MyCollider = GetComponent<Collider2D>();
			MyRigidbody = GetComponent<Rigidbody2D>();
		}

		void Start () {
			foreach (Transform child in GetComponentInChildren<Transform>()) {
				var wall = child.GetComponent<BreakableWall>();
				wall.ChangeEnabled(false);
				BreakableObjects.Add(wall);
			}
		}

		public void Break () {
			foreach (var obj in BreakableObjects) {
				obj.Break();
			}
			Destroy(gameObject);
		}

		public void ChangeEnabled (bool isEnabled) {
			MyCollider.enabled = isEnabled;
			MyRigidbody.simulated = isEnabled;
		}
	}
}
