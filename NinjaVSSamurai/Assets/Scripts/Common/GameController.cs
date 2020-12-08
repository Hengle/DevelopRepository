﻿﻿using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class GameController : SingletonMonoBehaviour<GameController> {
	[SerializeField] List<GameObject> managersPrefab;

	const int UIDragThreshold = 40; // 全体のドラッグに影響するので注意
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
		QualitySettings.vSyncCount = 1;
		Application.targetFrameRate = 60;
		Input.multiTouchEnabled = true;
		DG.Tweening.DOTween.Init();
	}

	void InitializeManagers () {
		foreach (var prefab in managersPrefab) {
			var manager = Instantiate(prefab);
			manager.name = prefab.name;
			manager.transform.SetParent(transform, false);
		}
		DG.Tweening.DOTween.Init();
	}

	void CreateSharedObject () {
		if (FindObjectOfType<EventSystem>() == null) {
			var es = new GameObject("EventSystem", typeof(EventSystem));
			es.GetComponent<EventSystem>().pixelDragThreshold = UIDragThreshold;
			es.AddComponent<StandaloneInputModule>();
			es.transform.SetParent(transform, false);
		} else {
			Debug.LogWarning("EventSystemはコントローラで生成. シーン内に置かないこと");
		}
	}

	// ゲーム開始前に呼び出す
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	static void SwitchLogEnabled () {
#if UNITY_EDITOR
		var str = IsDebugMode ? "有効" : "無効";
		Debug.Log("ログ出力は"+str+"です");
#endif

		Debug.unityLogger.logEnabled = IsDebugMode;
	}

}
