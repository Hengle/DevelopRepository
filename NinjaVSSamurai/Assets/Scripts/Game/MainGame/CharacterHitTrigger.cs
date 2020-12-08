using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MainGame {
	[RequireComponent(typeof(Collider2D))]
	public class CharacterHitTrigger : MonoBehaviour, IReceiveDamagable {
		[SerializeField] int baseDamageRatio = 1;
		[SerializeField] bool isHead;

		public event Action<int> OnDamageListener; // damage, isHeadShot

		public void ReceiveDamage (int damage) {
			OnDamageListener?.Invoke(damage * baseDamageRatio);
		}

		public bool IsCritical () {
			return isHead;
		}
	}
}
