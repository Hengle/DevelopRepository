using System;
using System.Linq;
using System.Collections.Generic;

public static class ListExtensions {
	/// <summary>
	/// 空かNullならTRUEを返す
	/// </summary>
	public static bool IsNullOrEmpty<T> (this List<T> list) {
		if (list == null)
			return true;

		if (list.Count == 0)
			return true;
			
		return false;
	}

	/// <summary>
	/// ランダムに一つ抽出
	/// </summary>
	public static T Random<T> (this List<T> list) {
		return list.Shuffle().First();
	}
	
	/// <summary>
	/// 順番をランダムに入れ替え
	/// </summary>
	public static List<T> Shuffle<T> (this List<T> list) {
		return list.OrderBy(i => Guid.NewGuid()).ToList();
	}
}
