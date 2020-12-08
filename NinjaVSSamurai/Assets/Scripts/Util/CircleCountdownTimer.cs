using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Util {
	[RequireComponent(typeof(Image))]
	public class CircleCountdownTimer : Timer {
		[SerializeField] TextMeshProUGUI timeLabel;
		[SerializeField] int setCount = 3;
		[SerializeField] int setInterval = 1;

		Image TimeGauge { get; set; }
		int LocalTime { get; set; }
		Action OnTimeoutEvent { get; set; }


		void Awake () {
			TimeGauge = GetComponent<Image>();
			if (TimeGauge.type != Image.Type.Filled) { Debug.LogError("画像タイプをFilledにしてください"); return; }
		}

		public void BeginCountdown (Action onTimeout = null) {
			LocalTime = setCount;
			timeLabel.text = LocalTime.ToString();
			OnTimeoutEvent = onTimeout;

			Show();
			Begin(setInterval, LocalTime, OnUpdate, OnInterval, OnTimeout);
		}

		public override void Stop () {
			base.Stop();
			Hide();
		}

		void OnInterval () {
			LocalTime--;
			timeLabel.text = LocalTime.ToString();
		}

		void OnUpdate () {
			var per = IntervalProgressPercent;
			if (per >= 1.0f) per = 1.0f;

			TimeGauge.fillAmount = per;
		}

		void OnTimeout () {
			Hide();
			OnTimeoutEvent?.Invoke();
		}

		void Show () {
			TimeGauge.enabled = true;
			timeLabel.enabled = true;
		}

		void Hide () {
			TimeGauge.enabled = false;
			timeLabel.enabled = false;
		}
	}
}
