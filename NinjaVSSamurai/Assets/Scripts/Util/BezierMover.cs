using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Util {
	public class BezierMover : MonoBehaviour {
		[SerializeField] Transform target;
		[SerializeField] Transform controlPoint1;
		[SerializeField] Transform controlPoint2;
		[SerializeField] Transform destinationPoint;
		[SerializeField] float duration = 1.0f;
		[SerializeField] AnimationCurve ease = AnimationCurve.Linear(0,0,1,1);

		public event Action OnMoveEndListener;
		bool IsMovable { get; set; }
		Vector3 DefaultPos { get; set; }
		float StartTime { get; set; }


		void Update () {
			if (!IsMovable) return;

			var diff = Time.time - StartTime;
			if (diff > duration) {
				target.position = destinationPoint.position;
				IsMovable = false;
				OnMoveEndListener?.Invoke();
			}

			var rate = diff / duration;
			var easePos = ease.Evaluate(rate);
			target.position = GetPoint(DefaultPos, controlPoint1.position, controlPoint2.position, destinationPoint.position, easePos);
		}

		public void StartMove (Vector3? destPos = null) {
			if (destPos != null) {
				destinationPoint.position = destPos.Value;
			}

			DefaultPos = target.position;
			IsMovable = true;
			StartTime = Time.time;
		}

		Vector3 GetPoint (Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t) {
			var oneMinusT = 1f - t;

			return oneMinusT * oneMinusT * oneMinusT * p0 +
				   3f * oneMinusT * oneMinusT * t * p1 +
				   3f * oneMinusT * t * t * p2 +
				   t * t * t * p3;
		}


	}
}
