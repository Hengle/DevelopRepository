using UnityEngine;

public class RuntimeInitializer {
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void InitializeBeforeSceneLoad() {
        // ゲーム中に常に存在するオブジェクトを生成、およびシーンの変更時にも破棄されないようにする。
        var dataManager = new GameObject("DataManager", typeof(DataManager));
        GameObject.DontDestroyOnLoad(dataManager);

        var adManager = new GameObject("TenjinManager", typeof(TenjinManager));
        GameObject.DontDestroyOnLoad(adManager);


        //var adManagerPrefab = Resources.Load("Prefabs/TenjinManager");
        //var adManager = GameObject.Instantiate(adManagerPrefab);
        //adManager.name = adManagerPrefab.name;
        //GameObject.DontDestroyOnLoad(adManager);

    }

} // class RuntimeInitializer