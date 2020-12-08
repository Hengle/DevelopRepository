using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Util {
	public class AnimationEventHandler : MonoBehaviour {
		Dictionary<string, Action> WatchEvents { get; set; } = new Dictionary<string, Action>();


		public void SetEventReceiver (string eventName, Action onComplete) {
			if (!WatchEvents.ContainsKey(eventName)) {
				WatchEvents.Add(eventName, onComplete);
			}
		}

		void RemoveEventReceiver (string eventName) {
			WatchEvents.Remove(eventName);
		}

		void OnAnimationEvent (string eventName) {
			if (WatchEvents.ContainsKey(eventName)) {
				var ev = WatchEvents[eventName];
				//RemoveEventReceiver(eventName);
				ev();
			}
		}
	}
}
