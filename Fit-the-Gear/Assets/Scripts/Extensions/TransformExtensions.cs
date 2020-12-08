using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TransformExtensions {

	public static void KillChild (this Transform tf, int index) {
		if (tf.childCount < 1) return;

		var child = tf.GetChild(0);
		Object.Destroy(child.gameObject);
		tf.DetachChildren();
	}
}
