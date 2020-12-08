
using UnityEngine;

public class MasterDataManager : Singleton<MasterDataManager>
{
    MasterData Cache { get; set; }

    /// <summary>
    /// マスター取得 - 一度取得したものはキャッシュする
    /// </summary>
    public T Load<T>(string path, bool useCache = true) where T : ScriptableObject
    {
        if (useCache)
        {
            T cache = FindCache<T>();
            if (cache != null)
            {
                return cache;
            }
        }

        var loadedMaster = Object.Instantiate(Resources.Load<T>(path));
        CacheInstance(loadedMaster);
        return loadedMaster;
    }

    T FindCache<T>() where T : ScriptableObject
    {
        var type = typeof(T);
        if (type == typeof(MasterData) && Cache != null) {
        	return Cache as T;
        }
        return null;
    }

    void CacheInstance<T>(T target) where T : ScriptableObject
    {
        if (target.GetType() == typeof(MasterData)) {
        	Cache = target as MasterData;
        }
    }
}