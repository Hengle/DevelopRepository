using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PostEffect {
	[RequireComponent(typeof(Camera))]
	public class CameraBlur : PostEffectBase {
		public void SetSamplingDistance (float samplingDistance) {
			MyMaterial.SetFloat("_SamplingDistance", samplingDistance);
		}
	}
}
