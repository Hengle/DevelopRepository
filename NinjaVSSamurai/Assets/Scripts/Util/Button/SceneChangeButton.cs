using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Util {
	[RequireComponent(typeof(Button))]
	public class SceneChangeButton : MonoBehaviour {
		[SerializeField] SceneManager.SceneNames sceneName;
		[SerializeField] float delayTime = 0.0f;
		[SerializeField] bool isOneShot = true;

		bool IsOneShotEnd { get; set; }

		public event Action OnClickListener;


		void Awake () {
			GetComponent<Button>().onClick.AddListener(() => {
				if (IsOneShotEnd) return;

				IsOneShotEnd |= isOneShot;
				OnClickListener?.Invoke();
				SceneManager.Instance.ChangeSceneFade(sceneName, delayTime);
			});
		}
	}
}
