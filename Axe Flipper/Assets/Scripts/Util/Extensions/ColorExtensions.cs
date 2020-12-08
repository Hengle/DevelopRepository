using UnityEngine;

public static class ColorExtensions  {
	/// <summary>
	/// 透過度のみ変更
	/// </summary>
	public static Color SetAlpha (this Color self, float alpha) {
		return new Color(self.r, self.g, self.b, alpha);
	}
}
