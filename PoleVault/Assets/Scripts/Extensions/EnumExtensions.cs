using System;
using System.Linq;
using System.Collections.Generic;

public static class EnumExtensions  {
	static readonly Random rand = new Random();
	
	/// <summary>
	/// 定義された列挙型の値をランダムで１つ取得
	/// </summary>
	public static T Random <T> () {
		return Enum.GetValues(typeof(T))
			.Cast<T>()
			.OrderBy(c => rand.Next())
			.FirstOrDefault();
	}
	
	/// <summary>
	/// 定義された列挙型の値をランダムに並び替えて取得
	/// </summary>
	public static List<T> RandomList<T> () {
		return Enum.GetValues(typeof(T))
			.Cast<T>()
			.OrderBy(c => rand.Next())
			.ToList();
	}
	
	/// <summary>
	/// 列挙型の名前を文字列で取得
	/// </summary>
	public static string Name <T> (this T type) {
		return Enum.GetName(typeof(T), type);
	}

	/// <summary>
	/// int型のindexから列挙型に変換
	/// </summary>
	public static T CastFromInt <T> (int index) {
		return (T)Enum.ToObject(typeof(T), index);
	}
}
