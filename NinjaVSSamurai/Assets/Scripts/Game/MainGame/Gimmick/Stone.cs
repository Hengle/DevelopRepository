using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MainGame {
	[RequireComponent(typeof(Collider2D))]
	[RequireComponent(typeof(Rigidbody2D))]
	[RequireComponent(typeof(SpriteRenderer))]
	public class Stone : GimmickBase, IActionMonitorable {
		[SerializeField] GameObject efBreakPrefab;
		[SerializeField] GameObject efBreakStrongPrefab;

		public override event Action<bool> OnActEnd;

		Collider2D MyCollider { get; set; }
		Rigidbody2D MyRigidbody { get; set; }
		SpriteRenderer MyRenderer { get; set; }
		bool IsHitCritical { get; set; }
		bool IsSendDamaged { get; set; }

		const int DefaultDamage = 4;


		protected override void Awake () {
			base.Awake();

			MyCollider = GetComponent<Collider2D>();
			MyRigidbody = GetComponent<Rigidbody2D>();
			MyRenderer = GetComponent<SpriteRenderer>();
		}

		void Start () {
			AudioManager.Instance.LoadSe(AudioManager.SeName.Break);
		}

		void Update () {
			if (!IsMoveStart && MyRigidbody.velocity.magnitude > 0.5) {
				Activate();
			}
		}

		public override void Activate () {
			MyRigidbody.gravityScale = 1.5f;
			OnActEnd(false);
			IsMoveStart = true;
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

		public void ChangeEnabled (bool isEnabled) {
			MyCollider.enabled = isEnabled;
			MyRigidbody.simulated = isEnabled;
		}

		void OnCollisionEnter2D (Collision2D collision) {

			if (collision.gameObject.tag == "PlayerBone") {
				Break();
			}

			if (collision.gameObject.tag == "PhysicsStopper") {
				Break();
			}
		}

		void OnTriggerEnter2D (Collider2D collision) {
			if (IsSendDamaged) return;

			var iReceiveDamagable = collision.gameObject.GetComponent<IReceiveDamagable>();
			if (iReceiveDamagable != null) {
				iReceiveDamagable.ReceiveDamage(DefaultDamage);
				IsHitCritical = iReceiveDamagable.IsCritical();
				IsSendDamaged = true;
			}
		}

		void Break () {
			if (!MyRigidbody.simulated) return;

			MyRenderer.enabled = false;
			MyCollider.enabled = false;
			MyRigidbody.simulated = false;

			var prefab = IsHitCritical ? efBreakStrongPrefab : efBreakPrefab;
			var efbreak = Instantiate(prefab);
			efbreak.gameObject.transform.position = transform.position;
			efbreak.gameObject.transform.localScale = efBreakPrefab.gameObject.transform.localScale;

			AudioManager.Instance.PlaySe(AudioManager.SeName.Break);
			StartCoroutine(DelayInvoke(1.0f, () => OnActEnd(true)));
		}

		IEnumerator DelayInvoke (float time, Action onInvoke) {
			yield return new WaitForSeconds(time);
			onInvoke.Invoke();
		}
	}
}
