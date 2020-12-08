using System;
using UnityEngine;

namespace Util {
	public class Timer : MonoBehaviour {
		protected float TimeoutProgressPercent { get; private set; }
		protected float IntervalProgressPercent { get; private set; }

		float TimeoverCount { get; set; }
		float IntervalCount { get; set; }
		float EndTime { get; set; }
		float Interval { get; set; }
		bool IsBegining { get; set; }

		Action OnUpdateEvent { get; set; }
		Action IntervalEvent { get; set; }
		Action TimeoverEvent { get; set; }


		void Update () {
			if (IsBegining) {
				TimeoverCount += Time.deltaTime;
				IntervalCount += Time.deltaTime;

				TimeoutProgressPercent = TimeoverCount / EndTime;
				IntervalProgressPercent = IntervalCount / Interval;

				OnUpdateEvent?.Invoke();

				if (TimeoverCount >= EndTime) {
					Stop();
					TimeoverEvent?.Invoke();
					return;
				}

				if (IntervalCount >= Interval) {
					IntervalEvent?.Invoke();
					IntervalCount = 0.0f;
				}
			}
		}

		public void Begin (float interval = 1, float endTime = 5f, Action onUpdateEvent = null, Action intervalEvent = null, Action timeOverEvent = null) {
			if (IsBegining) return;

			Interval = interval;
			EndTime = endTime;
			OnUpdateEvent = onUpdateEvent;
			IntervalEvent = intervalEvent;
			TimeoverEvent = timeOverEvent;
			IsBegining = true;
		}

		public void Pause () {
			IsBegining = false;
		}

		public virtual void Stop () {
			IsBegining = false;
			TimeoutProgressPercent = 1f;
			TimeoverCount = 0f;
			IntervalProgressPercent = 1f;
			IntervalCount = 0f;
		}
	}
}
