using UnityEngine;
using System.Collections.Generic;

public static class GameObjectExtensions {
	/// <summary>
	/// 指定されたインターフェイスを実装したコンポーネントを取得
	/// </summary>
	public static T FindComponentWithInterface<T> () where T : class {
		foreach (var c in GameObject.FindObjectsOfType<Component>()) {
			if (c is T component) {
				return component;
			}
		}
		return null;
	}

	/// <summary>
	/// 指定されたインターフェイスを実装したコンポーネントを持つオブジェクトを全取得
	/// </summary>
	public static List<GameObject> FindObjectsWithInterface<T> () where T : class {
		var list = new List<GameObject>();
		foreach (var c in GameObject.FindObjectsOfType<Component>()) {
			if (c is T component) {
				list.Add(c.gameObject);
			}
		}
		return list;
	}

	/// <summary>
	/// 指定されたインターフェイスを実装したコンポーネントを全取得
	/// </summary>
	public static List<T> FindComponentsWithInterface<T> () where T : class {
		var list = new List<T>();
		foreach (var c in GameObject.FindObjectsOfType<Component>()) {
			if (c is T component) {
				list.Add(component);
			}
		}
		return list;
	}

}