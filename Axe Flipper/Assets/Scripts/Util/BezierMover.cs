using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Util {
	public class BezierMover : MonoBehaviour {
		[SerializeField] TrailRenderer trailRenderer;
		[SerializeField] AnimationCurve easing = AnimationCurve.Linear(0, 0, 1, 1);
		[SerializeField] float duration = 1.0f;
		[SerializeField, Range(0.0f, 1.0f)] float controlPointRatio1 = 0.25f;
		[SerializeField, Range(0.0f, 1.0f)] float controlPointRatio2 = 0.75f;
		[SerializeField] float amplitudeMin = 1.0f;
		[SerializeField] float amplitudeMax = 10.0f;

		Vector3 StartPos2D { get; set; }
		Vector3 EndPos2D { get; set; }
		Vector3 ControlPoint1 { get; set; }
		Vector3 ControlPoint2 { get; set; }

		Vector2 RandomAmplitude { get; set; }
		Vector2 RandomDirection { get; set; }
		bool IsActive { get; set; }


		void Awake () {
			var randAmp1 = UnityEngine.Random.Range(amplitudeMin, amplitudeMax);
			var randAmp2 = UnityEngine.Random.Range(amplitudeMin, amplitudeMax);

			RandomAmplitude = RandomAmplitude.SetX(randAmp1);
			RandomAmplitude = RandomAmplitude.SetY(randAmp2);

			var randDir1 = UnityEngine.Random.Range(0, 2) % 2 == 0 ? 1 : -1;
			var randDir2 = UnityEngine.Random.Range(0, 2) % 2 == 0 ? 1 : -1;

			RandomDirection = RandomDirection.SetX(randDir1);
			RandomDirection = RandomDirection.SetY(randDir2);
		}

		/// <summary>
		/// 3D空間上の位置から2DのRectTransformへ
		/// </summary>
		public void SetTarget (Camera camera3D, Camera camera2D, Vector3 startWorldPos3D, RectTransform targetRectTF) {
			// 2DカメラのUI座標から2Dカメラのワールド座標へ
			var screenPos = RectTransformUtility.WorldToScreenPoint(camera2D, targetRectTF.position);
			var endPos2d = Vector3.zero;
			RectTransformUtility.ScreenPointToWorldPointInRectangle(targetRectTF, screenPos, camera2D, out endPos2d);
			EndPos2D = endPos2d;

			// 3Dカメラのビューポート座標から2Dカメラのワールド座標へ
			StartPos2D = camera2D.ViewportToWorldPoint(camera3D.WorldToViewportPoint(startWorldPos3D));
			StartPos2D = StartPos2D.SetZ(EndPos2D.z);

			// ベジェ曲線の制御点を設定
			ControlPoint1 = Vector3.Lerp(StartPos2D, EndPos2D, controlPointRatio1);
			ControlPoint2 = Vector3.Lerp(StartPos2D, EndPos2D, controlPointRatio2);

			IsActive = true;
		}

		/// <summary>
		/// 3D空間上の位置から同空間の別位置へ
		/// </summary>
		public void SetTarget (Vector3 startWorldPos3D, Vector3 endWorldPos3D) {
			EndPos2D = endWorldPos3D;
			StartPos2D = startWorldPos3D;

			// ベジェ曲線の制御点を設定
			ControlPoint1 = Vector3.Lerp(StartPos2D, EndPos2D, controlPointRatio1);
			ControlPoint2 = Vector3.Lerp(StartPos2D, EndPos2D, controlPointRatio2);

			IsActive = true;
		}

		public void Shot (Action onComplete = null) {
			transform.position = StartPos2D;
			if (trailRenderer != null) {
				trailRenderer.Clear();
			}

			var amp1 = new Vector3(RandomAmplitude.x * Mathf.Sin(Time.time * 2), 1, 1) * RandomDirection.x;
			var amp2 = new Vector3(RandomAmplitude.y * Mathf.Cos(Time.time * 2), 1, 1) * RandomDirection.y;

			var cp1 = ControlPoint1 + amp1;
			var cp2 = ControlPoint2 + amp2;

			float t = 0;
			DOTween.To(() => t, time => t = time, 1.0f, duration).SetEase(easing)
				.OnUpdate(() => transform.position = CalcBezier(StartPos2D, cp1, cp2, EndPos2D, t))
				.OnComplete(() => onComplete?.Invoke());
		}

		// a = 1 - time;
		// b = time;
		// a(3) + 3a(2)b + 3ab(2) + b(3)
		Vector3 CalcBezier (Vector3 start, Vector3 control1, Vector3 control2, Vector3 end, float time) {
			var a = 1 - time;
			var b = time;

			return a * a * a * start +
					3f * a * a * b * control1 +
					3f * a * b * b * control2 +
					b * b * b * end;
		}
	}
}
