using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Util {
	public class BillBoardRotator : MonoBehaviour {
		[SerializeField] Camera targetCamera;

		Vector3 FirstPosition { get; set; }


		void Awake () {
			if (targetCamera == null) {
				targetCamera = Camera.main;
			}
		}

		void Update () {
			var dir = transform.position - targetCamera.transform.position;
			var rot = Quaternion.LookRotation(dir, transform.up);
			rot.y = 0;
			rot.z = 0;
			transform.rotation = rot;
		}
	}
}
