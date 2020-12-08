using UnityEngine;

/// <summary>
/// 特定のクラスに依存しないものだけを定義
/// </summary>
public static class Statics {
	public static WaitForSeconds WaitOneSeconds = new WaitForSeconds(1.0f);
	public static WaitForSeconds WaitHalfSeconds = new WaitForSeconds(0.5f);
	public static WaitForSeconds WaitQuarterSeconds = new WaitForSeconds(0.25f);
	public static Color32 AlphaZero = new Color32(255, 255, 255, 0);
	public static Color32 AlphaOne = new Color32(255, 255, 255, 255);
}
