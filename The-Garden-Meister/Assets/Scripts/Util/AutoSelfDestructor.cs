using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Util {
	public class AutoSelfDestructor : MonoBehaviour {
		[SerializeField] float time = 2.0f;

		IEnumerator Start () {
			yield return new WaitForSeconds(time);

			Destroy(gameObject);
		}
	}
}
