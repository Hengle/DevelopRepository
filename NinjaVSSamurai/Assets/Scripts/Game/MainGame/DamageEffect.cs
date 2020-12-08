using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MainGame {
	public class DamageEffect : MonoBehaviour {
		[SerializeField] ParticleSystem efBlood;
		[SerializeField] Util.MultipleParticles efCriticalBlood;


		public void Play (Vector2 position, bool isCritical) {
			transform.position = position;
			if (isCritical) {
				efCriticalBlood.Play();
			} else {
				efBlood.Play();
			}
		}
	}
}
