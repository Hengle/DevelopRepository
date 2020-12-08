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

		public static float GetAngle360 (Vector3 origin, Vector3 target, bool isClockwise = true) {
			var dir = target - origin;
			var deg = Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg;
			if (deg < 0) deg += 360.0f;

			return isClockwise ? -deg : deg;
		}

        public static Vector3 Reflect(Vector3 forward, Vector3 normal)
        {
            var dot = Vector3.Dot(-forward, normal) * 2;
            return forward + dot * normal;
        }

        //三次
        public static Vector3 CalcBezier(Vector3 start, Vector3 end, Vector3 control1, Vector3 control2, float t)
        {
            Vector3 M0 = Vector3.Lerp(start, control1, t);
            Vector3 M1 = Vector3.Lerp(control1, control2, t);
            Vector3 M2 = Vector3.Lerp(control2, end, t);
            Vector3 B0 = Vector3.Lerp(M0, M1, t);
            Vector3 B1 = Vector3.Lerp(M1, M2, t);
            return Vector3.Lerp(B0, B1, t);
        }

        //二次
        public static Vector3 CalcBezier(Vector3 start, Vector3 end, Vector3 control1, float t)
        {
            Vector3 M0 = Vector3.Lerp(start, control1, t);
            Vector3 M1 = Vector3.Lerp(control1, end, t);
            Vector3 B = Vector3.Lerp(M0, M1, t);
            return B;
        }
    }
}
