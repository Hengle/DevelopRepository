using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MainGame {
	[RequireComponent(typeof(SpriteRenderer))]
	[RequireComponent(typeof(Collider2D))]
	[RequireComponent(typeof(Rigidbody2D))]
	public class Arrow : GimmickBase {
		[SerializeField] float defaultForce = 20;
		[SerializeField] float delayTime;
		[SerializeField] DamageEffect efDamagePrefab;

		public override event Action<bool> OnActEnd;

		SpriteRenderer MyRenderer { get; set; }
		Collider2D MyCollider { get; set; }
		Rigidbody2D MyRigidbody { get; set; }
		DamageEffect MyDamageEffect { get; set; }
		bool IsSendDamaged { get; set; }
		bool IsCollisionEntered { get; set; }

		const int DefaultDamage = 2;


		protected override void Awake () {
			base.Awake();

			MyCollider = GetComponent<Collider2D>();
			MyRigidbody = GetComponent<Rigidbody2D>();
			MyRenderer = GetComponent<SpriteRenderer>();

			MyDamageEffect = Instantiate(efDamagePrefab);
			MyDamageEffect.transform.localScale = efDamagePrefab.transform.localScale;
		}

		void Start () {
			AudioManager.Instance.LoadSe(AudioManager.SeName.Throw);
		}

		public override void Activate () {
			OnActEnd(false);
			IsMoveStart = true;

			StartCoroutine(DelayInvoke(delayTime, () => {
				AudioManager.Instance.PlaySe(AudioManager.SeName.Throw);
				MyRigidbody.AddForce(transform.up * defaultForce, ForceMode2D.Impulse);
			}));
		}

		public override void Dispose () {
			base.Dispose();
		}

		public override void FindRegisterComponent () {
			base.FindRegisterComponent();
		}

		public override void SetFirstAct () {
			OnActEnd(true);
		}

		void OnCollisionEnter2D (Collision2D collision) {
			if (IsCollisionEntered) return;

			if (collision.gameObject.tag == "PlayerBone") {
				IsCollisionEntered = true;
				transform.SetParent(collision.gameObject.transform);
				MyRigidbody.simulated = false;
				MyRigidbody.velocity = Vector2.zero;
				OnActEnd?.Invoke(true);
			} else if (collision.gameObject.tag == "PhysicsStopper") {
				IsCollisionEntered = true;
				MyRigidbody.simulated = false;
				MyRigidbody.velocity = Vector2.zero;
				StartCoroutine(DelayInvoke(1.0f, () => OnActEnd?.Invoke(true)));
			}
		}

		void OnTriggerEnter2D (Collider2D collision) {
			if (IsSendDamaged) return;

			var iReceiveDamagable = collision.gameObject.GetComponent<IReceiveDamagable>();
			if (iReceiveDamagable != null) {
				iReceiveDamagable.ReceiveDamage(DefaultDamage);
				MyDamageEffect.Play(efDamagePrefab.transform.position, iReceiveDamagable.IsCritical());
				IsSendDamaged = true;
			}
		}

		IEnumerator DelayInvoke (float time, Action onInvoke) {
			yield return new WaitForSeconds(time);
			onInvoke.Invoke();
		}

	}
}
