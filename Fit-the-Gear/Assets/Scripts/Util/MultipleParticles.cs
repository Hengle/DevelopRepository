using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Util {
	public class MultipleParticles : MonoBehaviour {
		ParticleSystem[] particles;


		void Awake () {
			particles = transform.GetComponentsInChildren<ParticleSystem>();
		}

		public void Play () {
			foreach (var particle in particles) {
				particle.Play();
			}
		}

		public void Stop (ParticleSystemStopBehavior behavior) {
			foreach (var particle in particles) {
				particle.Stop(true, behavior);
			}
		}
	}
}
