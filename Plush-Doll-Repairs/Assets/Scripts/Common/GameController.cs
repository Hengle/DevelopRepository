using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class GameController : SingletonMonoBehaviour<GameController> {
	[SerializeField] List<GameObject> managersPrefab;

	public const bool IsDebugMode = false;


	protected override void Awake () {
		base.Awake();

		if (isCreated) return;

		DontDestroyOnLoad(gameObject);

		InitializeSetting();
		InitializeManagers();
		CreateSharedObject();
	}

	void InitializeSetting () {
		SetResolution(1280); // 反映はフレーム更新後
		QualitySettings.vSyncCount = 0;
		Application.targetFrameRate = 60;
		Input.multiTouchEnabled = false;
		DG.Tweening.DOTween.Init();
	}

	void InitializeManagers () {
		foreach (var prefab in managersPrefab) {
			var manager = Instantiate(prefab);
			manager.name = prefab.name;
			manager.transform.SetParent(transform, false);
		}
	}

	void CreateSharedObject () {
		if (FindObjectOfType<EventSystem>() == null) {
			var es = new GameObject("EventSystem", typeof(EventSystem));
			es.AddComponent<StandaloneInputModule>();
			es.transform.SetParent(transform, false);
		} else {
			Debug.LogWarning("EventSystemはコントローラで生成. シーン内に置かないこと");
		}
	}

	void SetResolution (float baseResolution) {
		float screenRate = baseResolution / Screen.height;
		if (screenRate > 1) screenRate = 1;
		int width = (int)(Screen.width * screenRate);
		int height = (int)(Screen.height * screenRate);

		Screen.SetResolution(width, height, true);
	}

	// ゲーム開始前に呼び出す
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	static void SwitchLogEnabled () {
#if UNITY_EDITOR
		var str = IsDebugMode ? "<color=green>有効</color>" : "<color=red>無効</color>";
		Debug.Log("ログ出力は" + str + "です");
#endif

		Debug.unityLogger.logEnabled = IsDebugMode;
	}

}
