using UnityEngine;

public static class VectorExtensions {
	/// <summary>
	/// 各成分のみに値を設定し返却. 返却された値は元の変数に代入すること
	/// </summary>
	public static Vector3 SetX (this Vector3 self, float x) {
		self.x = x;
		return self;
	}
	public static Vector3 SetY (this Vector3 self, float y) {
		self.y = y;
		return self;
	}
	public static Vector3 SetZ (this Vector3 self, float z) {
		self.z = z;
		return self;
	}
	public static Vector3 AddX (this Vector3 self, float x) {
		self.x += x;
		return self;
	}
	public static Vector3 AddY (this Vector3 self, float y) {
		self.y += y;
		return self;
	}
	public static Vector3 AddZ (this Vector3 self, float z) {
		self.z += z;
		return self;
	}
}
