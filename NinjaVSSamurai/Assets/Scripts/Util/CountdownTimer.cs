using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Util {
	public class CountdownTimer : Timer {
		[SerializeField] TextMeshProUGUI timeLabel;

		const int IntervalSeconds = 1;

		TimeSpan MyTimeSpan { get; set; }
		TimeSpan IntervalTimeSpan { get; set; } = new TimeSpan(0, 0, 1);
		Action OnTimeoutEvent { get; set; }


		public void BeginCountdown (DateTime baseTime, DateTime targetTime, Action onTimeout = null) {
			float needSeconds = (float)(targetTime - baseTime).TotalSeconds;
			MyTimeSpan = new TimeSpan(0, 0, (int)needSeconds);
			SetTimeText();
			OnTimeoutEvent = onTimeout;

			Show();
			Begin(IntervalSeconds, needSeconds, null, OnInterval, OnTimeout);
		}

		public override void Stop () {
			base.Stop();
			Hide();
		}

		void OnInterval () {
			MyTimeSpan -= IntervalTimeSpan;
			SetTimeText();
		}

		void OnTimeout () {
			Hide();
			OnTimeoutEvent?.Invoke();
		}

		void Show () {
			timeLabel.enabled = true;
		}

		void Hide () {
			timeLabel.enabled = false;
		}

		void SetTimeText () {
			int hourWithDay = 24 * MyTimeSpan.Days + MyTimeSpan.Hours;
			timeLabel.text = $"{hourWithDay:D2}:{MyTimeSpan.Minutes:D2}:{MyTimeSpan.Seconds:D2}";
		}
	}
}
