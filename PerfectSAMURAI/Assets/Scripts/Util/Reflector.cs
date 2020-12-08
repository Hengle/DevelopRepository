using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Util {
	[RequireComponent(typeof(Collider2D))]
	[RequireComponent(typeof(Rigidbody2D))]
	public class Reflector : MonoBehaviour {
		Collider2D MyCollider { get; set; }
		Rigidbody2D MyRigidbody { get; set; }

		void Awake () {
			MyCollider = GetComponent<Collider2D>();
			MyRigidbody = GetComponent<Rigidbody2D>();
		}

		void OnCollisionEnter2D (Collision2D collision) {
			MyRigidbody.velocity = Vector2.zero;

			var collisionNormal = collision.contacts[0].normal;
			var dot = Vector2.Dot(-transform.up, collisionNormal) * 2;
			var reflectVector = (Vector2)transform.up + dot * collisionNormal;

			transform.up = reflectVector;
		}
	}
}
