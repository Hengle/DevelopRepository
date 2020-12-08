using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MainGame {
	[RequireComponent(typeof(Collider2D))]
	[RequireComponent(typeof(Rigidbody2D))]
	public class BreakableWall : MonoBehaviour, IBreakable {
		[SerializeField] ParticleSystem efChipPrefab;

		Collider2D MyCollider { get; set; }
		Rigidbody2D MyRigidbody { get; set; }


		void Awake () {
			MyCollider = GetComponent<Collider2D>();
			MyRigidbody = GetComponent<Rigidbody2D>();
		}

		public void Break () {
			var effect = Instantiate(efChipPrefab);
			effect.gameObject.transform.position = transform.position;
			effect.gameObject.transform.localScale = efChipPrefab.gameObject.transform.localScale;
			Destroy(gameObject);
		}

		public void ChangeEnabled (bool isEnabled) {
			MyCollider.enabled = isEnabled;
			MyRigidbody.simulated = isEnabled;
		}
	}
}
