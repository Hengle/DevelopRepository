using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Util {
	[RequireComponent(typeof(Camera))]
	public class TouchEventHandler : MonoBehaviour {
		[SerializeField] bool isUGUIHittable = true;

		Camera MyCamera { get; set; }
		bool IsTouchable { get; set; }
		bool IsPressing { get; set; }

		public event Action<Vector3> OnTouchStartListener;
		public event Action<Vector3> OnTouchKeepListener;
		public event Action<Vector3> OnTouchReleaseListener;


		void Awake () {
			MyCamera = GetComponent<Camera>();
			Activate();
		}

		void Update () {
			if (!IsTouchable) return;

			if (Input.GetMouseButtonDown(0) && !IsPressing) {
				if (isUGUIHittable && IsUGUIHit(Input.mousePosition)) return;

				OnTouchStartListener?.Invoke(Input.mousePosition);
				IsPressing = true;
			}

			if (Input.GetMouseButton(0) && IsPressing) {
				OnTouchKeepListener?.Invoke(Input.mousePosition);
			}

			if (Input.GetMouseButtonUp(0) && IsPressing) {
				IsPressing = false;
				OnTouchReleaseListener?.Invoke(Input.mousePosition);
			}
		}

		public void Activate () {
			IsTouchable = true;
			IsPressing = false;
		}

		public void Deactivate () {
			IsTouchable = false;
			IsPressing = false;
			OnTouchReleaseListener?.Invoke(Input.mousePosition);
		}

		public Vector3 ConvertScreenToWorldPos3D (Vector3 pos) {
			var convertedPos = MyCamera.ScreenToWorldPoint(pos);
			convertedPos = new Vector3(convertedPos.x, 0, convertedPos.y);
			return convertedPos;
		}

		public Vector3 ConvertFromScreenToWorldPos2D (Vector3 pos) {
			var convertedPos = MyCamera.ScreenToWorldPoint(pos);
			convertedPos = new Vector3(convertedPos.x, convertedPos.y, 0);
			return convertedPos;
		}

		public GameObject Raycast (Vector3 pos) {
			var ray = MyCamera.ScreenPointToRay(pos);

			if (Physics.Raycast(ray, out RaycastHit hit)) {
				return hit.collider.gameObject;
			}

			return null;
		}

		bool IsUGUIHit (Vector3 mousePosition) {
			var pointer = new PointerEventData(EventSystem.current);
			pointer.position = mousePosition;
			List<RaycastResult> result = new List<RaycastResult>();
			EventSystem.current.RaycastAll(pointer, result);
			return result.Count > 0;
		}
	}
}
