using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Util {
	public static class VectorUtility {
		public static Vector2 Parallel (Vector2 forward, Vector2 normal) {
			var dot = Vector2.Dot(-forward, normal);
			return forward + dot * normal;
		}

		public static Vector2 Reflect (Vector2 forward, Vector2 normal) {
			var dot = Vector2.Dot(-forward, normal) * 2;
			return forward + dot * normal;
		}

		// 3回判定
		public static void ReflectContinuous (Vector2 forward, Vector2 normal) {
			var hit = Physics2D.Raycast(forward, normal, Mathf.Infinity);

#if UNITY_EDITOR
			Debug.DrawRay(forward, normal * 100, Color.yellow);
#endif
			if (hit.collider != null) {
				var inDirection = Reflect(normal, hit.normal);

#if UNITY_EDITOR
				Debug.DrawRay(hit.point, inDirection * 100, Color.red);
#endif
				hit = Physics2D.Raycast(hit.point + hit.normal * 0.01f, inDirection, Mathf.Infinity);
				if (hit.collider != null) {
					inDirection = Reflect(inDirection, hit.normal);
#if UNITY_EDITOR
					Debug.DrawRay(hit.point, inDirection * 100, Color.green);
#endif
				}
			}

		}
	}
}
