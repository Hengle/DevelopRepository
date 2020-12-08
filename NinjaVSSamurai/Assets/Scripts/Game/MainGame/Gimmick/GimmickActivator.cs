using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MainGame {
	[RequireComponent(typeof(SpriteRenderer))]
	[RequireComponent(typeof(Collider2D))]
	public class GimmickActivator : MonoBehaviour {
		[SerializeField] Sprite spGimmickOFF;
		[SerializeField] Sprite spGimmickON;
		[SerializeField] List<GimmickBase> targetGimmicks = new List<GimmickBase>();

		SpriteRenderer MyRenderer { get; set; }
		Collider2D MyCollider { get; set; }


		void Awake () {
			MyRenderer = GetComponent<SpriteRenderer>();
			MyCollider = GetComponent<Collider2D>();

			MyRenderer.sprite = spGimmickOFF;
		}

		void OnCollisionEnter2D (Collision2D collision) {
			if (collision.gameObject.tag != "Weapon") return;

			MyRenderer.sprite = spGimmickON;
			MyCollider.enabled = false;

			foreach (var gimmick in targetGimmicks) {
				gimmick.Activate();
			}
		}
	}
}
