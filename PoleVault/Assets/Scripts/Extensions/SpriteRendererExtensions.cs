using UnityEngine;

public static class SpriteRendererExtensions {
	/// <summary>
	/// 画像の左端からratioパーセントの位置をワールド座標で返す
	/// </summary>
	public static Vector3 PositionRatioFromLeft (this SpriteRenderer self, float ratio) {
		Debug.Assert(ratio >= 0.0f && ratio <= 1.0f, "比率は0 - 1の値で設定してください");

		return self.transform.TransformPoint(new Vector3(self.sprite.bounds.size.x * ratio, 0, 0f));
	}
}
