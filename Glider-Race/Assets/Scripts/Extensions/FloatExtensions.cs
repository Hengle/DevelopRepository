using UnityEngine;

public static class FloatExtensions  {
	/// <summary>
	/// floatとfloatの比較. 閾値以内なら同値とみなす
	/// </summary>
	public static bool SafeEquals (this float self, float obj, float threshold = 0.001f) {
		return Mathf.Abs(self - obj) <= threshold;
	}

	/// <summary>
	/// 指定桁数で丸める
	/// </summary>
	public static float RoundDigit (this float self, int disit) {
		Debug.Assert(disit <= 7, "指定の桁数では丸められません");
		return float.Parse(self.ToString("F"+disit));
	}
}
