using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VisualEffects {
	public class Dissolve : VisualEffectBase {

		public void Play (bool isOut, float duration, float delay = 0.0f, Action onComplete = null) {
			float from = isOut ? 0 : 1;
			float to = isOut ? 1 : 0;

			SetValue("_Threshold", from, to, duration, delay, onComplete);
		}
	}
}
