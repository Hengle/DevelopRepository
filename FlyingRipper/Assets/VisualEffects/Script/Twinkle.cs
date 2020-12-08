using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VisualEffects {
	public class Twinkle : VisualEffectBase {
		public void Play (bool toRightUp, float duration, float delay = 0.0f, Action onComplete = null) {
			float from = toRightUp ? 1 : -1;
			float to = toRightUp ? -1 : 1;

			SetValue("_EffectPos", from, to, duration, delay, onComplete);
		}
	}
}
