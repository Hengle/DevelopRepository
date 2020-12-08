using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LinqExtensions {
	static System.Random Ramdom = new System.Random();

	/// <summary>
	/// ランダムに一つ抽出
	/// </summary>
	public static T Random<T> (this IEnumerable<T> ie) {
		return ie.ElementAt(Ramdom.Next(ie.Count()));
	}
}
