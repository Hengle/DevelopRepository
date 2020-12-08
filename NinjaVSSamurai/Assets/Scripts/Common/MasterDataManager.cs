using System.Collections.Generic;
using Data.Master;
using UnityEngine;

public class MasterDataManager : Singleton<MasterDataManager> {
	List<MasterBase> Cache { get; set; } = new List<MasterBase>();


	/// <summary>
	/// マスター取得
	/// </summary>
	public T Load<T> (string path, bool useCache = true) where T : MasterBase {
		if (useCache) {
			T cache = FindCache<T>();
			if (cache != null) {
				return cache;
			}
		}

		var loadedMaster = Object.Instantiate(Resources.Load<T>(path));
		if (useCache) Cache.Add(loadedMaster);
		return loadedMaster;
	}

	T FindCache<T> () where T : MasterBase {
		var type = typeof(T);

		foreach (var c in Cache) {
			if (c.GetType() == type) {
				return c as T;
			}
		}
		return null;
	}

}
