using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PostEffect {
	public class CameraMonoTone : PostEffectBase {
		public void SetThreshold (float threshold) {
			MyMaterial.SetFloat("_Threshold", threshold);
		}
	}
}
